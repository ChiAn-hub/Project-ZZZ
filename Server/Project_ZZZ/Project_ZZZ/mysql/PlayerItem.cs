using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("player_item")]
    public class PlayerItem
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int UserId { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int ItemId { get; set; }

        public int Count { get; set; }
    }
}
