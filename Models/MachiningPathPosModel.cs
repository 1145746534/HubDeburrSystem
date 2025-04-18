using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 加工路径点模型
    /// </summary>
    public class MachiningPathPosModel
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        /// 位姿ID，4-5位长度，第一个窗口起始ID为1000 ，第二个窗口起始ID为2000，以此类推。
        /// </summary>
        public int PoseId { get; set; }
        /// <summary>
        /// 图像行坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double Row { get; set; }
        /// <summary>
        /// 图像列坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double Column { get; set; }

        /// <summary>
        /// 行的真实边
        /// </summary>
        public double rowEdge;
        /// <summary>
        /// 列的真实边
        /// </summary>
        public double columnEdge;
        /// <summary>
        /// 行偏差绝对值
        /// </summary>
        public double rowDeviationAbs;
        /// <summary>
        /// 列偏差绝对值
        /// </summary>
        public double columnDeviationAbs;
        /// <summary>
        /// 是否符合条件
        /// </summary>
        public bool IsAccord = true;

        /// <summary>
        /// 物理X轴坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double X { get; set; }
        /// <summary>
        /// 物理Y轴坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double Y { get; set; }
        /// <summary>
        /// 物理Z轴坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double Z { get; set; }
        /// <summary>
        /// 欧拉角X
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double EX { get; set; }
        /// <summary>
        /// 欧拉角Y
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double EY { get; set; }
        /// <summary>
        /// 欧拉角Z
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double EZ { get; set; }
        /// <summary>
        /// 四元素Q1
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 5)]
        public double Q1 { get; set; }
        /// <summary>
        /// 四元素Q2
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 5)]
        public double Q2 { get; set; }
        /// <summary>
        /// 四元素Q3
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 5)]
        public double Q3 { get; set; }
        /// <summary>
        /// 四元素Q4
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 5)]
        public double Q4 { get; set; }
    }
}
