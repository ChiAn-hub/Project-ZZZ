using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum  RegisterType//注册的结果
{
    Success,//注册成功
    Failed,//注册失败
    UserExist,//用户已存在
}
public enum LoadType//登录的结果
{
    Success,//登录成功
    Failed,//登录失败
    PasswordError,//密码错误
    NoUser,//没有该用户
}
public enum SetNameType//设置昵称的结果
{
    Success,//设置成功
    Failed,//设置失败
}
namespace Project_ZZZ.mysql
{
    public class UserManager:Singleton<UserManager>
    {
        public User GetUser(int userid)//获取用户数据
        {
            if(userid <= 0)
            {
                return null;
            }
            try
            {
                return MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<User>().Where(it => it.Id == userid).Single();
            }catch(Exception e)
            {
                Debug.LogError("获取用户数据失败：" + e);
                return null;
            }
        }
        public RegisterType Register(string account, string password,out int userId)//注册
        {
            userId = 0;
            try
            {
                int count = MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<User>().Where(it => it.Account == account).Count();
                if(count > 0)
                {
                    return RegisterType.UserExist;
                }
                else
                {
                    User user = new User();
                    user.Account = account;
                    user.Password = password;
                    user.LoadTime = DateTime.Now;
                    userId = MySqlSugarClient.Instance.m_sqlSugarClient.Insertable(user).ExecuteReturnIdentity();//更新数据库并获取到用户id
                    return userId > 0 ? RegisterType.Success: RegisterType.Failed;//根据用户id判断是否注册成功
                }
            }
            catch(Exception e)
            {
                Debug.LogError("注册失败" + e);
                return RegisterType.Failed;
            }
        }
        public LoadType Load(string account, string password,out int userId,out bool isSetName)//登录
        {
            userId = 0;
            isSetName = false;
            try
            {
                User user = null;
                user = MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<User>().Where(it => it.Account == account).Single();
                if (user == null)
                {
                    return LoadType.NoUser;
                }
                else
                {
                    if(user.Password != password)
                    {
                        return LoadType.PasswordError;
                    }
                    else
                    {
                        userId = user.Id;
                        isSetName = !string.IsNullOrWhiteSpace(user.Name);
                        user.LoadTime = DateTime.Now;
                        MySqlSugarClient.Instance.m_sqlSugarClient.Updateable(user).ExecuteCommand();
                        return LoadType.Success;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("登录失败" + e);
                return LoadType.Failed;
            }
        }

        public SetNameType SetName(int userId, string name)//设置昵称
        {
            name = name?.Trim();//处理空格
            if (userId <= 0 ||string.IsNullOrWhiteSpace(name) ||name.Length > 12)
            {
                return SetNameType.Failed;
            }
            try
            {
                User user = MySqlSugarClient.Instance.m_sqlSugarClient.Queryable<User>().Where(it => it.Id == userId).Single();
                if (user == null)
                {
                    return SetNameType.Failed;
                }
                user.Name = name;
                int result = MySqlSugarClient.Instance.m_sqlSugarClient.Updateable(user).ExecuteCommand();

                return result == 1 ? SetNameType.Success : SetNameType.Failed;
            }
            catch(Exception e)
            {
                Debug.LogError("设置昵称失败" + e);
                return SetNameType.Failed;
            }
        }
    }
}
