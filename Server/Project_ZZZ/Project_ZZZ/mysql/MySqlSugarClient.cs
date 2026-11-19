using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Project_ZZZ.mysql
{
    public class MySqlSugarClient: Singleton<MySqlSugarClient>
    {
        public SqlSugarClient m_sqlSugarClient;
        public void Init()
        {
            m_sqlSugarClient = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = "server=127.0.0.1;uid=root;pwd=YOUR_DB_PASSWORD;database=clinenttest;SslMode=None;",
                    DbType = DbType.MySql,//设置数据库类型
                    IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                    InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息

                });
            Debug.LogInfo("数据库连接成功");
        }
    }
}
