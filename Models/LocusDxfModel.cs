using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 轨迹Dxf模型
    /// </summary>
    public class LocusDxfModel
    {
        /// <summary>
        /// 轨迹名称（轮型）
        /// </summary>
        public List<string> LocusName { get; set; } = new List<string>();
        /// <summary>
        /// 轨迹点
        /// </summary>
        public List<List<MachiningPathPosModel>> LocusPoints { get; set; } = new List<List<MachiningPathPosModel>>();
    }
}
