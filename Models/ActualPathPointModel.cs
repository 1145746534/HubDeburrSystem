using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 实际路径点
    /// </summary>
    internal class ActualPathPointModel
    {
        /// <summary>
        /// K的的下标
        /// </summary>
        public int IndexK { get; set; }
        /// <summary>
        /// 实际点X的绝对距离偏差
        /// </summary>
        public double ActualRowAbs { get; set; }
        /// <summary>
        /// 实际点Y的绝对距离偏差
        /// </summary>
        public double ActualColumnAbs { get; set; }
    }
}
