using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("player_quest")]
    public class PlayerQuest
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int UserId { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int QuestId { get; set; }

        public QuestState State { get; set; }

        public double CurNum { get; set; }
    }
}