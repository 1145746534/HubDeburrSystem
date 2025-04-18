using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class NinePointCalibrationData
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
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
        /// 物理X坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double X { get; set; }
        /// <summary>
        /// 物理Y坐标
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 3)]
        public double Y { get; set; }
    }
}
