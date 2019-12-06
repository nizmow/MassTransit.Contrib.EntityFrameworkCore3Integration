﻿// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
// Modifications copyright 2019 Neil Houghton.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Context;
using MassTransit.Saga;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Saga
{
    public class EntityFrameworkSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IsolationLevel _isolationLevel;
        readonly IRawSqlLockStatements _rawSqlLockStatements;
        readonly ISagaDbContextFactory _sagaDbContextFactory;

        public EntityFrameworkSagaRepository(ISagaDbContextFactory sagaDbContextFactory,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            IRawSqlLockStatements rawSqlLockStatements = null)
        {
            _sagaDbContextFactory = sagaDbContextFactory;
            _isolationLevel = isolationLevel;
            _rawSqlLockStatements = rawSqlLockStatements;
        }

        public static EntityFrameworkSagaRepository<TSaga> CreateOptimistic(ISagaDbContextFactory sagaDbContextFactory)
        {
            return new EntityFrameworkSagaRepository<TSaga>(sagaDbContextFactory, IsolationLevel.ReadCommitted);
        }

        public static EntityFrameworkSagaRepository<TSaga> CreateOptimistic(Func<DbContext> sagaDbContextFactory)
        {
            return CreateOptimistic(new DelegateSagaDbContextFactory(sagaDbContextFactory));
        }

        public static EntityFrameworkSagaRepository<TSaga> CreatePessimistic(ISagaDbContextFactory sagaDbContextFactory,
            IRawSqlLockStatements rawSqlLockStatements = null)
        {
            return new EntityFrameworkSagaRepository<TSaga>(sagaDbContextFactory, IsolationLevel.Serializable,
                rawSqlLockStatements ?? new MsSqlLockStatements());
        }

        public static EntityFrameworkSagaRepository<TSaga> CreatePessimistic(Func<DbContext> sagaDbContextFactory,
            IRawSqlLockStatements rawSqlLockStatements = null)
        {
            return CreatePessimistic(new DelegateSagaDbContextFactory(sagaDbContextFactory), rawSqlLockStatements);
        }

        async Task<IEnumerable<Guid>> IQuerySagaRepository<TSaga>.Find(ISagaQuery<TSaga> query)
        {
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                return await dbContext.Set<TSaga>()
                    .Where(query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .ToListAsync().ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            var dbContext = _sagaDbContextFactory.Create();
            try
            {
                scope.Set(new
                {
                    Persistence = "entityFramework",
                    Entities = dbContext.Model.GetEntityTypes().Select(type => type.Name)
                });
            }
            finally
            {
                _sagaDbContextFactory.Release(dbContext);
            }
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var dbContext = _sagaDbContextFactory.CreateScoped(context);
            try
            {
                var execStrategy = dbContext.Database.CreateExecutionStrategy();
                if (execStrategy is SqlServerRetryingExecutionStrategy)
                {
                    await execStrategy.Execute(async () =>
                    {
                        using (var transaction =
                            await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false))
                        {
                            await SendLogic(transaction, dbContext, context, policy, next).ConfigureAwait(false);
                        }
                    });
                }
                else
                {
                    using (var transaction = await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false))
                    {
                        await SendLogic(transaction, dbContext, context, policy, next).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _sagaDbContextFactory.Release(dbContext);
            }
        }

        async Task SendLogic<T>(IDbContextTransaction transaction, DbContext dbContext, ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var sagaId = context.CorrelationId.Value;

            if (policy.PreInsertInstance(context, out var instance))
            {
                var inserted = await PreInsertSagaInstance(dbContext, context, instance).ConfigureAwait(false);
                if (!inserted)
                    instance = null; // Reset this back to null if the insert failed. We will use the MissingPipe to create instead
            }

            try
            {
                if (instance == null)
                {
                    // Query with a row Lock instead using FromSql. Still a single trip to the DB (unlike EF6, which has to make one dummy call to row lock)
                    var rowLockQuery = _rawSqlLockStatements?.GetRowLockStatement<TSaga>(dbContext);
                    if (rowLockQuery != null)
                        instance= await dbContext.Set<TSaga>().FromSqlRaw(rowLockQuery, new object[] {sagaId}).SingleOrDefaultAsync(context.CancellationToken)
                            .ConfigureAwait(false);
                    else
                        instance = await dbContext.Set<TSaga>().SingleOrDefaultAsync(x => x.CorrelationId == sagaId, context.CancellationToken).ConfigureAwait(false);
                }

                if (instance == null)
                {
                    var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                    await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                }
                else
                {
                    var sagaConsumeContext = new EntityFrameworkSagaConsumeContext<TSaga, T>(dbContext, context, instance);

                    sagaConsumeContext.LogUsed();

                    await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
                }

                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }

                throw;
            }
            catch (DbUpdateException ex)
            {
                if (IsDeadlockException(ex))
                {
                    // deadlock, no need to rollback
                }
                else
                {
                    context.LogFault(this, ex, instance?.CorrelationId);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception innerException)
                    {
                        LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                context.LogFault(this, ex, instance?.CorrelationId);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }

                throw;
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var dbContext = _sagaDbContextFactory.CreateScoped(context);
            try
            {
                List<Guid> nonTrackedInstances = null;

                // Only perform this additional DB Call for pessimistic concurrency
                if (_rawSqlLockStatements != null)
                {
                    // We just get the correlation ids related to our Filter.
                    // We do this outside of the transaction to make sure we don't create a range lock.
                    nonTrackedInstances = await dbContext.Set<TSaga>()
                        .AsNoTracking()
                        .Where(context.Query.FilterExpression)
                        .Select(x => x.CorrelationId)
                        .ToListAsync(context.CancellationToken)
                        .ConfigureAwait(false);
                }

                var execStrategy = dbContext.Database.CreateExecutionStrategy();
                if (execStrategy is SqlServerRetryingExecutionStrategy)
                {
                    await execStrategy.Execute(async () =>
                    {
                        using (var transaction =
                            await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false))
                        {
                            await SendQueryLogic(nonTrackedInstances, transaction, dbContext, context, policy, next);
                        }
                    });
                }
                else
                {
                    using (var transaction = await dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken).ConfigureAwait(false))
                    {
                        await SendQueryLogic(nonTrackedInstances, transaction, dbContext, context, policy, next);
                    }
                }
            }
            finally
            {
                _sagaDbContextFactory.Release(dbContext);
            }
        }

        async Task SendQueryLogic<T>(IList<Guid> nonTrackedInstances, IDbContextTransaction transaction, DbContext dbContext,
            SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                // Simple path for Optimistic Concurrency
                if (_rawSqlLockStatements == null)
                {
                    var instances = await dbContext.Set<TSaga>()
                        .Where(context.Query.FilterExpression)
                        .ToListAsync(context.CancellationToken)
                        .ConfigureAwait(false);

                    if (!instances.Any())
                    {
                        var missingSagaPipe = new MissingPipe<T>(dbContext, next);
                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                    else
                        await Task.WhenAll(instances.Select(instance => SendToInstance(context, dbContext, policy, instance, next))).ConfigureAwait(false);
                }
                // Pessimistic Concurrency
                else
                {
                    // Hack for locking row for the duration of the transaction.
                    // We only lock one at a time, since we don't want an accidental range lock.
                    var rowLockQuery = _rawSqlLockStatements.GetRowLockStatement<TSaga>(dbContext);

                    var missingCorrelationIds = new List<Guid>();
                    if (nonTrackedInstances?.Any() == true)
                    {
                        var foundInstances = new List<Task>();

                        foreach (var nonTrackedInstance in nonTrackedInstances)
                        {
                            // Query with a row Lock instead using FromSql. Still a single trip to the DB (unlike EF6, which has to make one dummy call to row lock)
                            var instance = await dbContext.Set<TSaga>()
                                .FromSqlRaw(rowLockQuery, new object[] {nonTrackedInstance})
                                .SingleOrDefaultAsync(context.CancellationToken)
                                .ConfigureAwait(false);

                            if (instance != null)
                                foundInstances.Add(SendToInstance(context, dbContext, policy, instance, next));
                            else
                                missingCorrelationIds.Add(nonTrackedInstance);
                        }

                        if (foundInstances.Any())
                            await Task.WhenAll(foundInstances).ConfigureAwait(false);
                    }

                    // If no sagas are found or all are missing
                    if (nonTrackedInstances.Count == missingCorrelationIds.Count)
                    {
                        var missingSagaPipe = new MissingPipe<T>(dbContext, next);

                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                }

                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }

                throw;
            }
            catch (DbUpdateException ex)
            {
                if (IsDeadlockException(ex))
                {
                    // deadlock, no need to rollback
                }
                else
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception innerException)
                    {
                        LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                    }
                }

                throw;
            }
            catch (SagaException sex)
            {
                context.LogFault(sex);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }

                throw;
            }
            catch (Exception ex)
            {
                context.LogFault(ex);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception innerException)
                {
                    LogContext.Warning?.Log(innerException, "Transaction rollback failed");
                }

                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
            }
        }

        static bool IsDeadlockException(Exception exception)
        {
            return exception.GetBaseException() is SqlException baseException && baseException.Number == 1205;
        }

        async Task<bool> PreInsertSagaInstance<T>(DbContext dbContext, ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            EntityEntry<TSaga> entry = null;
            try
            {
                entry = dbContext.Set<TSaga>().Add(instance);
                await dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                context.LogInsert(this, instance.CorrelationId);

                return true;
            }
            catch (Exception ex)
            {
                // Because we will still be using the same dbContext, we need to reset the entry we just tried to pre-insert (likely a duplicate), so
                // on the next save changes (which is the update), it will pass.
                // see here for details: https://www.davideguida.com/how-to-reset-the-entities-state-on-a-entity-framework-db-context/
                entry.State = EntityState.Detached;

                context.LogInsertFault(this, ex, instance.CorrelationId);
            }

            return false;
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, DbContext dbContext, ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                var sagaConsumeContext = new EntityFrameworkSagaConsumeContext<TSaga, T>(dbContext, context, instance);

                sagaConsumeContext.LogUsed();

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
            }
            catch (SagaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }

        /// <summary>
        /// Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly DbContext _dbContext;
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;

            public MissingPipe(DbContext dbContext, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _dbContext = dbContext;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                var proxy = new EntityFrameworkSagaConsumeContext<TSaga, TMessage>(_dbContext, context, context.Saga, false);

                proxy.LogAdded();

                await _next.Send(proxy).ConfigureAwait(false);

                if (!proxy.IsCompleted)
                    _dbContext.Set<TSaga>().Add(context.Saga);

                await _dbContext.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
