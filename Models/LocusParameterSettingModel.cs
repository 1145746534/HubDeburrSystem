using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 轨迹参数设置模型
    /// </summary>
    public class LocusParameterSettingModel
    {
        /// <summary>
        /// 内圆卡尺长度
        /// </summary>
        public static int InnerCaliperLength { get; set; }
        /// <summary>
        /// 内圆半径
        /// </summary>
        public static int InnerRadius { get; set; }
        /// <summary>
        /// 偏差倍数
        /// </summary>
        public static int CalipersDevExpand { get; set; }
        /// <summary>
        /// 测量卡尺长度
        /// </summary>
        public static int CalipersMeaLength { get; set; }
        /// <summary>
        /// 测量卡尺宽度
        /// </summary>
        public static int CalipersMeaWidth { get; set; }
        /// <summary>
        /// 分割阈值
        /// </summary>
        public static int CalipersAmpThreshold { get; set; }
        /// <summary>
        /// 平滑参数
        /// </summary>
        public static int CalipersSmooth { get; set; }
        /// <summary>
        /// 外圆最小阈值
        /// </summary>
        public static int OuterMinThreshold { get; set; }

        /// <summary>
        /// 模板匹配最小相似度
        /// </summary>
        public static double MinSimilarity { get; set; }

        /// <summary>
        /// 图像缩放（模板制作和模板匹配）
        /// </summary>
        public static double ImageScale { get; set; }
        /// <summary>
        /// 模板角度开始
        /// </summary>
        public static double TemplateAngleStart { get; set; }
        /// <summary>
        /// 模板角度范围
        /// </summary>
        public static double TemplateAngleExtent { get; set; }


        /// <summary>
        /// 窗口暗部最小面积
        /// </summary>
        public static double DarkMinArea { get; set; }

        /// <summary>
        /// 窗口亮部最小面积
        /// </summary>
        public static double BrightMinArea { get; set; }

        /// <summary>
        /// 窗口微调时的单轮廓膨胀值
        /// </summary>
        public static double SingleXldDilation { get; set; }

        /// <summary>
        /// 窗口暗部最大阈值
        /// </summary>
        public static int DarkMaxThreshold { get; set; }

        /// <summary>
        /// 窗口亮部最小阈值
        /// </summary>
        public static int BrightMinThreshold { get; set; }

        /// <summary>
        /// 亮暗区域合并后的膨胀腐蚀值
        /// </summary>
        public static double UnionDilationErosion { get; set; }

        /// <summary>
        /// 加工轨迹偏移（像素：1像素大约等于0.12mm）
        /// </summary>
        public static int MachiningLocusOffset { get; set; }

        /// <summary>
        /// 轮廓点投影的最大距离，超过这个距离取原始的点
        /// </summary>
        public static double MaxDistance { get; set; }

        /// <summary>
        /// Igs文件路径
        /// </summary>
        public static string IgsPath { get; set; }

        /// <summary>
        /// 模板数据窗口选择的轮型
        /// </summary>
        public static string SelectWheelModel { get; set; }

        #region======边缘算法参数======
        /// <summary>
        /// 边缘算法滤镜参数：值越小，平滑效果越强，细节越少
        /// </summary>
        public static double CannyAlpha { get; set;}
        /// <summary>
        /// 边缘算法低阈值
        /// </summary>
        public static int CannyLowThresold { get; set;}
        /// <summary>
        /// 边缘算法高阈值
        /// </summary>
        public static int CannyHighThresold { get; set;}
        /// <summary>
        /// 边缘算法轮廓最小长度
        /// </summary>
        public static int XldMinLength { get; set;}

        /// <summary>
        /// 对比度增强掩模的宽高
        /// </summary>
        public static int MaskWidthHeight { get; set;}

        /// <summary>
        /// 是否图像增强
        /// </summary>
        public static bool IsImageEnhancement { get; set; }
        #endregion
    }
}
