using System;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Mappings
{
    public interface ISagaClassMap
    {
        Type SagaType { get; }
        void Configure(ModelBuilder modelBuilder);
    }
}
