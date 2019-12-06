using System;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Messages
{
    public class SimpleSagaMessageBase :
        CorrelatedBy<Guid>
    {
        public SimpleSagaMessageBase()
        {
        }

        public SimpleSagaMessageBase(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }

}