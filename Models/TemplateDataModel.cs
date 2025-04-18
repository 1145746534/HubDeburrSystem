using GalaSoft.MvvmLight;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 模板数据模型
    /// </summary>
    public class TemplateDataModel : ObservableObject
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Index { get; set; }

        /// <summary>
        /// 轮毂型号
        /// </summary>
        public string WheelType { get; set; }

        /// <summary>
        /// 轮辐数量
        /// </summary>
        public int SpokeQuantity { get; set; }

        /// <summary>
        /// 未用天数
        /// </summary>
        public int UnusedDays { get; set; }

        private bool _processingEnable;
        /// <summary>
        /// 加工使能
        /// </summary>
        public bool ProcessingEnable
        {
            get { return _processingEnable; }
            set {Set(ref _processingEnable, value); }
        }

        /// <summary>
        /// 模板中心行坐标（像素）
        /// </summary>
        public double CenterRow { get; set; }

        /// <summary>
        /// 模板中心列坐标（像素）
        /// </summary>
        public double CenterColumn { get; set; }

        /// <summary>
        /// 窗口暗部最大阈值
        /// </summary>
        public int DarkMaxThreshold {  get; set; }

        /// <summary>
        /// 窗口亮部最小阈值
        /// </summary>
        public int LightMinThreshold {  get; set; }

        /// <summary>
        /// 内圆卡尺长度
        /// </summary>
        public int InnerCircleCaliperLength { get; set; }

        /// <summary>
        /// 内圆半径
        /// </summary>
        public int InnerCircleRadius { get; set; }

        /// <summary>
        /// 角度补偿
        /// </summary>
        public double AngularCompensation { get; set; }

        /// <summary>
        /// 轨迹缩放
        /// </summary>
        public int LocusScale { get; set; }

        /// <summary>
        /// 出刀点PoseId
        /// </summary>
        public string OutPointPoseId { get; set; }

        /// <summary>
        /// 加工压力
        /// </summary>
        public float ProcessingPressure { get; set; }
    }
}
