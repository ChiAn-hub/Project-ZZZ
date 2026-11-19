using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectItem
{
    private readonly ParticleSystem[] particles;//用于保存和管理每个特效自身及子物体身上的组件
    private readonly List<ParticleSystem> playParticles = new List<ParticleSystem>();//用于保存所有需要主动播放的特效

    public GameObject EffectObject { get; private set; }//

    public PlayerEffectItem(GameObject effectObject)
    {
        EffectObject = effectObject;
        particles = effectObject.GetComponentsInChildren<ParticleSystem>(true);

        HashSet<ParticleSystem> subParticles = new HashSet<ParticleSystem>();//保存所有子发射器

        foreach (ParticleSystem particle in particles)//遍历特效上的所有粒子系统
        {
            ParticleSystem.MainModule main = particle.main;//获取粒子系统组件的主要模块
            main.loop = false;//关闭循环
            main.playOnAwake = false;//关闭自动播放
            main.stopAction = ParticleSystemStopAction.None;//关闭组件停止时的行为
            main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;//设置为总是播放,不管摄像机看没看

            ParticleSystem.SubEmittersModule subEmitters = particle.subEmitters;//获取该组件上的子发射器
            if (subEmitters.enabled)//子发射器是否启用
            {
                for (int i = 0; i < subEmitters.subEmittersCount; i++)//启用则将全部子发射器保存进集合里
                {
                    subParticles.Add(subEmitters.GetSubEmitterSystem(i));
                }
            }
        }
        foreach (ParticleSystem particle1 in particles)
        {
            if (!subParticles.Contains(particle1))//如果子发射器集合中不包含该粒子系统,则保存进该数组里去
            {
                playParticles.Add(particle1);
            }
        }
    }

    public void Play()//播放
    {
        EffectObject.SetActive(true);//显示该特效
        foreach (ParticleSystem particle in particles)//从对象池取出来重播时，先清空上次的粒子
        {
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);//重置每个粒子系统
        }
        foreach (ParticleSystem particle in playParticles)//播放整个特效
        {
            if (particle.gameObject.activeInHierarchy)//如果这个粒子系统所在的物体处于激活状态
            {
                particle.Play(false);
            }
        }
    }

    public bool IsAlive()//判断特效是否存活
    {
        foreach (ParticleSystem particle in particles)
        {
            if (particle.gameObject.activeInHierarchy && particle.IsAlive(false))//如果这个粒子系统所在的物体处于激活状态，并且这个粒子系统里还有活着的粒子
            {
                return true;
            }
        }
        return false;
    }
}
