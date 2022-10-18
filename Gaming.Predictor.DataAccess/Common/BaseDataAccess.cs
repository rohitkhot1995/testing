using Gaming.Predictor.Interfaces.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.DataAccess.Common
{
    public class BaseDataAccess
    {
        protected readonly String _ConnectionString;
        protected readonly String _ConnectionStringMOL;
        protected readonly String _Schema;

        public BaseDataAccess(IPostgre postgre)
        {
            _ConnectionString = postgre.ConnectionString;
            _ConnectionStringMOL = postgre.ConnectionStringMOL;
            _Schema = postgre.Schema;
        }
    }
}
