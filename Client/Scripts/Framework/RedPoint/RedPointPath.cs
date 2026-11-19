using UnityEngine;

public static class RedPointPath
{
    public const string Bag = "Bag";//背包名前缀
    public const string Quest = "Quest";//任务名前缀
    public static string GetBagItemPath(int itemId)
    {
        return Bag + "/" + itemId;
    }
    public static string GetQuestPath(int questId)
    {
        return Quest + "/" + questId;
    }
    public static string GetQuestNewPath(int questId)
    {
        return GetQuestPath(questId) + "/New";
    }

    public static string GetQuestCompletePath(int questId)
    {
        return GetQuestPath(questId) + "/Complete";
    }
}
