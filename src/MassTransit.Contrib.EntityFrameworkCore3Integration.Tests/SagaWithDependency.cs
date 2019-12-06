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
using System.Threading.Tasks;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Messages;
using MassTransit.Saga;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    public class SagaWithDependency :
        InitiatedBy<InitiateSimpleSaga>,
        Orchestrates<UpdateSagaDependency>,
        ISaga
    {
        public bool Completed { get; private set; }
        public bool Initiated { get; private set; }
        public string Name { get; private set; }

        public SagaDependency Dependency { get; set; }

        public Task Consume(ConsumeContext<InitiateSimpleSaga> context)
        {
            CorrelationId = context.Message.CorrelationId;
            Initiated = true;
            Name = context.Message.Name;
            Dependency = new SagaDependency
            {
                SagaInnerDependency = new SagaInnerDependency()
            };

            return Task.CompletedTask;
        }

        public Guid CorrelationId { get; set; }

        public Task Consume(ConsumeContext<UpdateSagaDependency> context)
        {
            Dependency.SagaInnerDependency.Name = context.Message.Name;
            Completed = true;
            return Task.CompletedTask;
        }
    }
}
