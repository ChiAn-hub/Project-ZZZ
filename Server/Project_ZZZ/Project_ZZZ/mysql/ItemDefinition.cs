using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("item_definition")]
    public class ItemDefinition
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ItemId { get; set; }

        public string Name { get; set; }
        public int ItemType { get; set; }
        public string Quality { get; set; }
        public int MaxStack { get; set; }
        public string IconName { get; set; }
        public long RecycleGold { get; set; }
        public int MaxHpBonus { get; set; }
        public int AttackBonus { get; set; }
        public int ImpactBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int CriticalRateBonus { get; set; }
        public int CriticalDamageBonus { get; set; }
    }
}