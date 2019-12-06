using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration
{
    public interface IRawSqlLockStatements
    {
        string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga;
    }
}
