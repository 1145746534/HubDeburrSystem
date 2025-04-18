using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class PointModel
    {
        /// <summary>
        /// X轴坐标（在像素中为列坐标）
        /// </summary>
        public double Column { get; set; }
        /// <summary>
        /// Y轴坐标（在像素中为行坐标）
        /// </summary>
        public double Row { get; set; }
        /// <summary>
        /// Z轴坐标
        /// </summary>
        public double Z { get; set; }
    }
}
