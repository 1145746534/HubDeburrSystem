using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Public
{
    /// <summary>
    /// 弹窗管理器
    /// </summary>
    public class DialogManager
    {
        /// <summary>
        /// 
        /// </summary>
        static Dictionary<string, Delegate> actionMap = new Dictionary<string, Delegate>();

        public static void Register<T>(string key, Delegate d)
        {
            if (!actionMap.ContainsKey(key))
                actionMap.Add(key, d);
        }
        public static void Unregister(string key)
        {
            if (actionMap.ContainsKey(key))
                actionMap.Remove(key);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void Execute<T>(string key, T data)
        {
            if (actionMap.ContainsKey(key))
                actionMap[key].DynamicInvoke(data);
        }

        /// <summary>
        ///执行并返回状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ExecuteAndResult<T>(string key, T data)
        {
            if (actionMap.ContainsKey(key))
            {
                var action = actionMap[key] as Func<T, bool>;
                if (action == null)
                    return false;
                //执行窗口启动时注册的对应弹出窗口的委托方法
                return action.Invoke(data);
            }
            return false;
        }
    }
}
