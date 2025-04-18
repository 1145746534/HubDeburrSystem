using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 坐标系路径点模型
    /// </summary>
    public class PathPointModel
    {
        /// <summary>
        /// 存储路径坐标轴原点3D模型
        /// </summary>
        public List<ModelUIElement3D> CModelUIElement3Ds { get; set; } = new List<ModelUIElement3D>();
        /// <summary>
        /// 存储路径X轴3D模型
        /// </summary>
        public List<ModelUIElement3D> XModelUIElement3Ds { get; set; } = new List<ModelUIElement3D>();
        /// <summary>
        /// 存储路径Y轴3D模型
        /// </summary>
        public List<ModelUIElement3D> YModelUIElement3Ds { get; set; } = new List<ModelUIElement3D>();
        /// <summary>
        /// 存储路径Z轴3D模型
        /// </summary>
        public List<ModelUIElement3D> ZModelUIElement3Ds { get; set; } = new List<ModelUIElement3D>();
    }
}
