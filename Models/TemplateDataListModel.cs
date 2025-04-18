using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class TemplateDataListModel
    {
        /// <summary>
        /// 存储活跃轮型列表
        /// </summary>
        public List<string> ActiveWheelTypeList { get; set; } = new List<string>();
        /// <summary>
        /// 存储不活跃轮型列表
        /// </summary>
        public List<string> NotActiveWheelTypeList { get; set; } = new List<string>();
        /// <summary>
        /// 存储活跃模板
        /// </summary>
        public List<HTuple> ActiveTemplateList { get; set; } = new List<HTuple>();
        /// <summary>
        /// 存储不活跃模板
        /// </summary>
        public List<HTuple> NotActiveTemplateList { get; set; } = new List<HTuple>();
    }
}
