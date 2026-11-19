using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("player_character")]
    public class PlayerCharacter
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int UserId { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int CharacterId { get; set; }

        public int Level { get; set; }
        public long Exp { get; set; }

        public int CurHp { get; set; }
        public int BaseMaxHp { get; set; }
        public int BaseAttack { get; set; }
        public int BaseImpact { get; set; }
        public int BaseDefense { get; set; }

        public int CriticalRate { get; set; }
        public int CriticalDamage { get; set; }
    }
}