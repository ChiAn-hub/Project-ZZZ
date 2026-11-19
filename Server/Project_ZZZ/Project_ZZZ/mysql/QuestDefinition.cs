using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("quest_definition")]
    internal class QuestDefinition
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int QuestId { get; set; }
        public string QuestType { get; set; }

        public string Target { get; set; }

        public double NeedCount { get; set; }

        public int CharacterExp { get; set; }

        public int WeaponExp { get; set; }

        public int PlayerExp { get; set; }

        public long PlayerGold { get; set; }
    }
}
