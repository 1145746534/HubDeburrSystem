using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class StatisticsDataModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 轮毂型号
        /// </summary>
        public string WheelModel { get; set; }
        /// <summary>
        /// 加工完成
        /// </summary>
        public int ProcessingComplete { get; set; }
        /// <summary>
        /// 加工异常
        /// </summary>
        public int MachiningAnomalies { get; set; }
        /// <summary>
        /// 总计
        /// </summary>
        public int Total {  get; set; }
    }
}
