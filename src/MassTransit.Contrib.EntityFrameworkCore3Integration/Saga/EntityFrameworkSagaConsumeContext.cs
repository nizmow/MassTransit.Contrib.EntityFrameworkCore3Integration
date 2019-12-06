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
using System.Threading.Tasks;
using MassTransit.Context;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Saga
{
    public class EntityFrameworkSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly DbContext _dbContext;
        readonly bool _existing;

        public EntityFrameworkSagaConsumeContext(DbContext dbContext, ConsumeContext<TMessage> context, TSaga instance, bool existing = true)
            : base(context)
        {
            Saga = instance;
            _dbContext = dbContext;
            _existing = existing;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            IsCompleted = true;
            if (_existing)
            {
                _dbContext.Set<TSaga>().Remove(Saga);

                this.LogRemoved();

                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}
