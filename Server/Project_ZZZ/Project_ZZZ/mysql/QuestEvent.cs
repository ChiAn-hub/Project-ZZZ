using System;

namespace Project_ZZZ.mysql
{
    internal static class QuestEvent
    {
        public static event Action<int, QuestTarget, double> ProgressChanged;
        public static void RaiseProgress(int userId, QuestTarget target, double amount)
        {
            if (userId <= 0 || target == QuestTarget.None || amount <= 0)
            {
                return;
            }
            ProgressChanged?.Invoke(userId, target, amount);
        }
    }
}
