using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.Models
{
    public enum MessageType
    {
        [Description("提示")]
        Default,

        [Description("成功")]
        Success,

        [Description("警告")]
        Warning,

        [Description("错误")]
        Error
    }
}
