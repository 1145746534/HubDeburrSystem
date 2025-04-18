using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 识别结果数据模型
    /// </summary>
    public class IdentifyDataModel
    {
        /// <summary>
        /// 轮型列表
        /// </summary>
        public List<string> WheelTypes { get; set; } = new List<string>();
        /// <summary>
        /// 相似度列表
        /// </summary>
        public List<double> Similaritys { get; set; } = new List<double>();
        /// <summary>
        /// 识别的轮型
        /// </summary>
        public string IdentifyWheelType { get; set; } = null;
        /// <summary>
        /// 识别的相似度
        /// </summary>
        public double Similarity { get; set; } = new double();
        /// <summary>
        /// 识别的弧度
        /// </summary>
        public HTuple Radian { get; set; } = new HTuple();
        /// <summary>
        /// 识别的弧度列表
        /// </summary>
        public List<HTuple> Radians { get; set; } = new List<HTuple>();
        /// <summary>
        /// 所有识别相似度超过系统设定相似度的轮型
        /// </summary>
        public List<string> IdentifyWheels { get; set; } = new List<string>();
    }
}
