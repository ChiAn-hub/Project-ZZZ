using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project_ZZZ.mysql
{
    internal class PlayerManager : Singleton<PlayerManager>
    {
        private const int ManSp = 240;
        private const int RecoverMinutes = 6;

        public PlayerProfile GetPlayerProfile(int userId)
        {
            if(userId <= 0)
            {
                return null;
            }
            try
            {
                PlayerProfile profile = MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<PlayerProfile>().Where(p => p.UserId == userId).Single();//从服务器中获取玩家信息
                if (profile == null)//如果玩家信息为空,证明玩家第一次登录,需要创建玩家信息
                {
                    profile = new PlayerProfile();
                    profile.UserId = userId;
                    profile.Level = 1;
                    profile.Exp = 0;    
                    profile.Gold = 0;
                    profile.Money = 0;
                    profile.Sp = ManSp;
                    profile.SpRecoverTime = DateTime.UtcNow;

                    int result = MySqlSugarClient.Instance.m_sqlSugarClient.Insertable(profile).ExecuteCommand();//将玩家信息插入到数据库中
                    if(result != 1)
                    {
                        return null;
                    }
                }
                RecoverSp(profile);//恢复体力
                return profile;
            }
            catch(Exception e)
            {
                Debug.LogError("获取玩家数据失败" + e);
                return null;
            }
        }

        public void RecoverSp(PlayerProfile profile)
        {
            if(profile == null)
            {
                return;
            }
            DateTime now = DateTime.UtcNow;
            if(profile.SpRecoverTime == DateTime.MinValue || profile.SpRecoverTime > now)//如果恢复时间为空或者恢复时间大于当前时间,证明体力已经满了,不需要恢复
            {
                profile.SpRecoverTime = now;
                MySqlSugarClient.Instance.m_sqlSugarClient.Updateable(profile).ExecuteCommand();
                return;
            }
            if(profile.Sp >= ManSp)//如果体力已经满了,不需要恢复
            {
                profile.SpRecoverTime = now;
                profile.Sp = ManSp;
                MySqlSugarClient.Instance.m_sqlSugarClient.Updateable(profile).ExecuteCommand();
                return;
            }
            TimeSpan timeSpan = now - profile.SpRecoverTime;//计算时间差
            int recoverCount = (int)(timeSpan.TotalMinutes / RecoverMinutes);//计算恢复的体力数量
            if (recoverCount <= 0)
            {
                return;
            }
            profile.Sp = Math.Min(profile.Sp + recoverCount, ManSp);
            if(profile.Sp >= ManSp)//如果体力满了,将恢复时间设置为当前时间
            {
                profile.SpRecoverTime = now;
            }
            else//如果体力没有满
            {
                profile.SpRecoverTime = profile.SpRecoverTime.AddMinutes(recoverCount * RecoverMinutes);//将恢复时间设置为当前时间加上恢复的体力数量
            }
            MySqlSugarClient.Instance.m_sqlSugarClient.Updateable(profile).ExecuteCommand();
        }
    }
}
