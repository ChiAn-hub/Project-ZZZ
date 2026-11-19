using System;
using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("player_profile")]
    public class PlayerProfile
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int UserId { get; set; }

        public int Level { get; set; }
        public long Exp { get; set; }
        public long Gold { get; set; }
        public long Money { get; set; }
        public int Sp { get; set; }
        public int BagCapacity { get; set; }
        public DateTime SpRecoverTime { get; set; }
    }
}
