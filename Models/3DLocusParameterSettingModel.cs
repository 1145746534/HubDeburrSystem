using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class _3DLocusParameterSettingModel
    {
        /// <summary>
        /// 生成终点时终点相对于起点偏移的距离
        /// </summary>
        //public static double EndPointOffsetDistance { get; set; }
        /// <summary>
        /// 进刀点X轴偏移距离（像素）
        /// </summary>
        public static double EntryPointXAxisOffsetDistance {  get; set; }
        /// <summary>
        /// 进刀点Y轴偏移距离（像素）
        /// </summary>
        public static double EntryPointYAxisOffsetDistance { get; set; }
        /// <summary>
        /// 出刀点X轴偏移距离（像素）
        /// </summary>
        public static double ExitPointXAxisOffsetDistance { get; set; }
        /// <summary>
        /// 出刀点Y轴偏移距离（像素）
        /// </summary>
        public static double ExitPointYAxisOffsetDistance { get; set; }
        /// <summary>
        /// 生成进出刀点时相对于起点和终点的偏移高度（mm）
        /// </summary>
        public static double EntryExitPointOffsetHeight { get; set; }
        /// <summary>
        /// 允许的总轨迹点数
        /// </summary>
        public static int TotalTrajectoryPointsAllowed { get; set; }
        /// <summary>
        /// 增加点相对于选中点偏移距离（像素）
        /// </summary>
        public static double IncreasePointOffsetDistance { get; set; }
    }
}
