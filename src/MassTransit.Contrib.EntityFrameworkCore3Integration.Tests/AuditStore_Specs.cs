// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GreenPipes.Util;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Audit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    [TestFixture]
    public class Saving_audit_records_to_the_audit_store
    {
        [Test]
        public async Task Should_have_consume_audit_records()
        {
            var consumed = _harness.Consumed;
            await Task.Delay(500);
            (await GetAuditRecords("Consume")).ShouldBe(consumed.Count());
        }

        [Test]
        public async Task Should_have_send_audit_record()
        {
            var sent = _harness.Sent;
            await Task.Delay(500);
            (await GetAuditRecords("Send")).ShouldBe(sent.Count());
        }

        [SetUp]
        public Task CleanAudit() => Task.CompletedTask;

        InMemoryTestHarness _harness;
        ConsumerTestHarness<TestConsumer> _consumer;
        EntityFrameworkAuditStore _store;

        [OneTimeSetUp]
        public async Task Send_message_to_test_consumer()
        {
            // add migrations by calling
            // dotnet ef migrations add --context auditdbcontext --output-dir Migrations\\Audit audit_init
            DbContextOptionsBuilder<AuditDbContext> optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>().
                UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                m =>
                    {
                        var executingAssembly = typeof(ContextFactory).GetTypeInfo().Assembly;

                        m.MigrationsAssembly(executingAssembly.GetName().Name);
                        m.MigrationsHistoryTable("__AuditEFMigrationHistoryAudit");
                    });

            _store = new EntityFrameworkAuditStore(optionsBuilder.Options, "EfCoreAudit");
            using (var dbContext = _store.AuditContext)
            {
                await dbContext.Database.MigrateAsync();
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE EfCoreAudit");
            }

            _harness = new InMemoryTestHarness();
            _harness.OnConnectObservers += bus =>
            {
                bus.ConnectSendAuditObservers(_store);
                bus.ConnectConsumeAuditObserver(_store);
            };
            _consumer = _harness.Consumer<TestConsumer>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        async Task<int> GetAuditRecords(string contextType)
        {
            using (var dbContext = _store.AuditContext)
                return await dbContext.Set<AuditRecord>()
                    .Where(x => x.ContextType == contextType)
                    .CountAsync();
        }


        class TestConsumer : IConsumer<A>,
            IConsumer<B>
        {
            public async Task Consume(ConsumeContext<A> context) =>
                await context.RespondAsync(new B());

            public Task Consume(ConsumeContext<B> context)
            {
                return TaskUtil.Completed;
            }
        }


        class A
        {
        }


        class B
        {
        }
    }
}
