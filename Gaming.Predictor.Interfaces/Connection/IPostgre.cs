using System;

namespace Gaming.Predictor.Interfaces.Connection
{
    public interface IPostgre
    {
        String Schema { get; }
        String ConnectionString { get; }
        String ConnectionStringMOL { get; }
    }
}