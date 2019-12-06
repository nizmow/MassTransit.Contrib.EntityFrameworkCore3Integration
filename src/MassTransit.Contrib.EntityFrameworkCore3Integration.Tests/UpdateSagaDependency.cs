using System;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Messages;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    [Serializable]
    public class UpdateSagaDependency :
        SimpleSagaMessageBase
    {
        public UpdateSagaDependency()
        {
        }

        public UpdateSagaDependency(Guid correlationId, string propertyValue)
            : base(correlationId)
        {
            this.Name = propertyValue;
        }

        public string Name { get; set; }
    }
}
