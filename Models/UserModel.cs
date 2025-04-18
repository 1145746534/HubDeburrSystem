using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 用户模型
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = "admin";
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "666666";
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; } = 0;
    }
}
