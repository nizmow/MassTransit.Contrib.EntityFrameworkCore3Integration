namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Messages
{
    using System;


    [Serializable]
    public class InitiateSimpleSaga :
        SimpleSagaMessageBase
    {
        public InitiateSimpleSaga()
        {
        }

        public InitiateSimpleSaga(Guid correlationId)
            :base(correlationId)
        {
        }

        public string Name { get; set; }
    }
}