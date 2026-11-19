using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("user")]
    public class User
    {
        //指定主键和自增列，当然数据库中也要设置主键和自增列才会有效
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Account { get; set; }
        public DateTime LoadTime { get; set; }
    }
}
