using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class ProductionDatas
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 轮毂型号
        /// </summary>
        public string WheelModel { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string ProcessingResult { get; set; }

        /// <summary>
        /// 处理用时
        /// </summary>
        public string ConsumingTime {  get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime ProcessingTime { get; set; }

    }
}
