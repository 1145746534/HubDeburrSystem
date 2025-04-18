using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class MenuModel
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 菜单文本
        /// </summary>
        public string MenuHeader { get; set; }
        /// <summary>
        /// 切换的页面
        /// </summary>
        public string TargetView { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIcon { get; set; }
    }
}
