using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public class ProductionRecord
    {

        [SugarColumn(IsIdentity = true, IsPrimaryKey = true,IsNullable = false)]
        public int Id { get; set; }

        /// <summary>
        /// 轮毂型号
        /// </summary>
        [SugarColumn(IsNullable = false ,Length = 40)]
        public string WheelType { get; set; }

        /// <summary>
        /// 相机拍照时间
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 10)]
        public string CameraPhotoTime { get; set; }


        /// <summary>
        /// 软件识别轮型时间
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 10)]
        public string SoftwareRecognitionTime { get; set; }

        /// <summary>
        /// 轮毂进入时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public DateTime WheelEntryTime { get; set; }


        /// <summary>
        /// 轮毂流出时间
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public DateTime WheelExitTime { get; set; }

        /// <summary>
        /// CT
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 10)]
        public float CT { get; set; }

        
        /// <summary>
        /// 匹配度
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 10)]
        public float MatchingDegree { get; set; }

        
        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 10)]
        public int Deleted { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "datetime")]
        public int CreateTime { get; set; }
    }
}
