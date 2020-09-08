using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COREWEBSOCKET.sqlBase
{
    public abstract class SqlSugarBase
    {
        public SqlSugarClient DB => GetInstance();

        SqlSugarClient GetInstance()
        {
            string connectionString = "Server=.;Database=nxzjfx;Integrated Security=False;User ID=sa;Password=123456;";

            var db = new SqlSugarClient(
                new ConnectionConfig
                {
                    ConnectionString = connectionString,
                    DbType = DbType.SqlServer,
                    IsShardSameThread = true
                }
            );
            return db;
        }
    }
}
