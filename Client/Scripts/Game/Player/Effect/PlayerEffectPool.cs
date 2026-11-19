using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectPool : Singleton<PlayerEffectPool>
{
    private readonly Dictionary<PlayerEffectType, GameObject> prefabDic = new Dictionary<PlayerEffectType, GameObject>();
    private readonly Dictionary<PlayerEffectType, List<PlayerEffectItem>> effectDic = new Dictionary<PlayerEffectType, List<PlayerEffectItem>>();

    public PlayerEffectPool()
    {
        ReadEffectInfo();//读取特效信息
    }

    private void ReadEffectInfo()//读取配置文件信息
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Config/PlayerEffect");
        PlayerEffectList effectList = JsonUtility.FromJson<PlayerEffectList>(textAsset.text);
        foreach (PlayerEffectInfo info in effectList.effects)
        {
            if (prefabDic.ContainsKey(info.effectType))
            {
                continue;
            }
            GameObject prefab = Resources.Load<GameObject>(info.prefabPath);

            prefabDic.Add(info.effectType, prefab);
            effectDic.Add(info.effectType, new List<PlayerEffectItem>());
        }
    }

    public PlayerEffectItem GetEffect(PlayerEffectType effectType, Vector3 position, Quaternion rotation)//获取对应特效
    {
        if (!prefabDic.TryGetValue(effectType, out GameObject prefab))
        {
            return null;
        }
        Vector3 effectPosition = position + rotation * prefab.transform.localPosition;//计算出特效的位置
        Quaternion effectRotation = rotation * prefab.transform.localRotation;//计算出特效的角度
        List<PlayerEffectItem> effects = effectDic[effectType];//获取对应的特效组件列表

        for (int i = effects.Count - 1; i >= 0; i--)
        {
            PlayerEffectItem effect = effects[i];
            if (effect.EffectObject == null)
            {
                effects.RemoveAt(i);
            }
            if (!effect.EffectObject.activeSelf)
            {
                effect.EffectObject.transform.SetPositionAndRotation(effectPosition, effectRotation);//设置特效位置和旋转
                return effect;
            }
        }
        //没有找到可用的特效,则进行实例化创建
        GameObject effectObject = Object.Instantiate(prefab, effectPosition, effectRotation);//实例化特效
        effectObject.SetActive(false);

        PlayerEffectItem newEffect = new PlayerEffectItem(effectObject);
        effects.Add(newEffect);
        return newEffect;
    }

    public void ReleaseEffect(PlayerEffectItem effect)
    {
        if (effect != null && effect.EffectObject != null)
        {
            effect.EffectObject.SetActive(false);
        }
    }

}
