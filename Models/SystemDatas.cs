using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    /// <summary>
    /// 系统数据
    /// </summary>
    public class SystemDatas
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string DataName { get; set; }
        public string DataValue { get; set; }
    }
}
