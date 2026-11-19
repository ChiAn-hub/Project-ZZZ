using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("player_equipment")]
    public class PlayerEquipment
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int UserId { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int CharacterId { get; set; }

        public int ItemId { get; set; }
    }
}