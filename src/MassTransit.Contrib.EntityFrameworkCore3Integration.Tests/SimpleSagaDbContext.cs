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

using System.Collections.Generic;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    public class SimpleSagaDbContext : SagaDbContext
    {
        public SimpleSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SimpleSagaMap(); }
        }
    }
}
