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
using GreenPipes.Internals.Extensions;
using GreenPipes.Internals.Reflection;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Mappings
{
    public abstract class SagaClassMap<TSaga> : ISagaClassMap
        where TSaga : class, ISaga
    {
        public Type SagaType => typeof(TSaga);

        public virtual void Configure(ModelBuilder modelBuilder)
        {
            if (!TypeCache<TSaga>.ReadWritePropertyCache.TryGetProperty("CorrelationId", out ReadWriteProperty<TSaga> _))
                throw new ConfigurationException("The CorrelationId property must be read/write for use with Entity Framework. Add a setter to the property.");
            EntityTypeBuilder<TSaga> cfg = modelBuilder.Entity<TSaga>();

            cfg.HasKey(t => t.CorrelationId);

            cfg.Property(t => t.CorrelationId)
                .ValueGeneratedNever();

            Configure(cfg, modelBuilder);
        }

        protected virtual void Configure(EntityTypeBuilder<TSaga> cfg, ModelBuilder modelBuilder)
        {
        }
    }
}
