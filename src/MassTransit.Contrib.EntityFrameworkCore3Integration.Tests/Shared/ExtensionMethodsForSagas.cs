// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenPipes.Util;
using MassTransit.Saga;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Shared
{
    public static class ExtensionMethodsForSagas
    {
        public static Task<Guid?> ShouldContainSaga<TSaga>(this InMemorySagaRepository<TSaga> repository, Guid sagaId,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            IQuerySagaRepository<TSaga> querySagaRepository = repository;

            return querySagaRepository.ShouldContainSaga(sagaId, timeout);
        }

        public static Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is IQuerySagaRepository<TSaga> querySagaRepository)
                return querySagaRepository.ShouldContainSaga(sagaId, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository",
                nameof(repository)));
        }

        public static async Task<Guid?> ShouldContainSaga<TSaga>(this IQuerySagaRepository<TSaga> repository,
            Guid sagaId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                Guid saga = (await repository.Where(x => x.CorrelationId == sagaId).ConfigureAwait(false))
                    .FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        public static Task<Guid?> ShouldNotContainSaga<TSaga>(this InMemorySagaRepository<TSaga> repository,
            Guid sagaId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            IQuerySagaRepository<TSaga> querySagaRepository = repository;

            return querySagaRepository.ShouldNotContainSaga(sagaId, timeout);
        }

        public static Task<Guid?> ShouldNotContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is IQuerySagaRepository<TSaga> querySagaRepository)
                return querySagaRepository.ShouldNotContainSaga(sagaId, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository",
                nameof(repository)));
        }

        public static async Task<Guid?> ShouldNotContainSaga<TSaga>(this IQuerySagaRepository<TSaga> repository,
            Guid sagaId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            Guid? saga = default;
            while (DateTime.Now < giveUpAt)
            {
                saga = (await repository.Where(x => x.CorrelationId == sagaId).ConfigureAwait(false)).FirstOrDefault();
                if (saga == Guid.Empty)
                    return default;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return saga;
        }

        public static Task<Guid?> ShouldContainSaga<TSaga>(this InMemorySagaRepository<TSaga> repository,
            Expression<Func<TSaga, bool>> filter,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            IQuerySagaRepository<TSaga> querySagaRepository = repository;

            return querySagaRepository.ShouldContainSaga(filter, timeout);
        }

        public static Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository,
            Expression<Func<TSaga, bool>> filter,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            if (repository is IQuerySagaRepository<TSaga> querySagaRepository)
                return querySagaRepository.ShouldContainSaga(filter, timeout);

            return TaskUtil.Faulted<Guid?>(new ArgumentException("Does not support IQuerySagaRepository",
                nameof(repository)));
        }

        public static async Task<Guid?> ShouldContainSaga<TSaga>(this IQuerySagaRepository<TSaga> repository,
            Expression<Func<TSaga, bool>> filter,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            var query = new SagaQuery<TSaga>(filter);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await repository.Where(query.FilterExpression).ConfigureAwait(false)).ToList();
                if (sagas.Count > 0)
                    return sagas.Single();

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }
    }
}