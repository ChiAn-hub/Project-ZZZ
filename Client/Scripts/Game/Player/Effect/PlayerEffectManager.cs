using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    private readonly List<PlayerEffectItem> playingEffects = new List<PlayerEffectItem>();//用于保存正在播放的特效

    public void PlayEffect(PlayerEffectType effectType, Vector3 position, Quaternion rotation)
    {
        if (!isActiveAndEnabled || effectType == PlayerEffectType.None)
        {
            return;
        }
        PlayerEffectItem effect = PlayerEffectPool.Instance.GetEffect(effectType, position, rotation);
        if (effect == null)
        {
            return;
        }
        effect.Play();
        playingEffects.Add(effect);
    }

    private void LateUpdate()
    {
        for (int i = playingEffects.Count - 1; i >= 0; i--)
        {
            PlayerEffectItem effect = playingEffects[i];
            if (effect.EffectObject != null && effect.IsAlive())//检测特效是否播放完毕
            {
                continue;
            }
            PlayerEffectPool.Instance.ReleaseEffect(effect);
            playingEffects.RemoveAt(i);
        }
    }

    public void StopAllEffects()
    {
        foreach (PlayerEffectItem effect in playingEffects)
        {
            PlayerEffectPool.Instance.ReleaseEffect(effect);
        }

        playingEffects.Clear();
    }

    private void OnDisable()
    {
        StopAllEffects();
    }

}
