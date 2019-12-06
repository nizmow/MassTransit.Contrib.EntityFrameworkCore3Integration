using System;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Messages
{
    public class CompleteSimpleSaga :
        SimpleSagaMessageBase
    {
        public CompleteSimpleSaga()
        {
        }

        public CompleteSimpleSaga(Guid correlationId)
            :
            base(correlationId)
        {
        }
    }
}