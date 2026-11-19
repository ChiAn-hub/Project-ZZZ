using Project_ZZZ.mysql;
using Project_ZZZ.Net;

namespace Project_ZZZ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MySqlSugarClient.Instance.Init();
            QuestManage.Instance.Init();
            ServerSocket.Instance.Init();
        }
    }
}
