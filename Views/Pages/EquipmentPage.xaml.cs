using HubDeburrSystem.Public;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HubDeburrSystem.Views.Pages
{
    /// <summary>
    /// EquipmentPage.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentPage : UserControl
    {
        public EquipmentPage()
        {
            InitializeComponent();
        }

        private string downStr = string.Empty;
        private DispatcherTimer _timer;

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Name == "Text_JogSpeed")
            {
                string numStr = textBox.Text + e.Text;
                bool isDou = double.TryParse(numStr, out double doubleVal);               
                if (!isDou)
                {
                    e.Handled = true; 
                }
            }else
            {
                string numStr = textBox.Text + e.Text;
                bool isDou = double.TryParse(numStr, out double doubleVal);
                if (!isDou)
                {
                    e.Handled = true;
                }
            }

           
        }

        private void My_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            downStr = (sender as Button).Content.ToString();
            // 初始化定时器
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50); // 设置定时器间隔，比如每100毫秒触发一次
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 在这里执行你想要持续进行的功能
            
            SetPLCAction(true);
        }

        private void SetPLCAction(bool  BValue)
        {
            if (PlcHelper.PlcCilent.Connected)
            {
                switch (downStr)
                {
                    case "复位伺服报警":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 4, BValue);
                        break;
                    case "轴向下点动":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 5, BValue);
                        break;
                    case "轴向上点动":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 6, BValue);
                        break;
                    case "轴暂停":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 7, BValue);
                        break;
                    case "手动一键回零":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 7, 0, BValue);
                        break;
                    case "复位加工完成":
                        S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 1, 1, false);
                         break;
                }
            }
        }

        private void My_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            downStr = (sender as Button).Content.ToString();
            // 停止定时器
            if (_timer != null && _timer.IsEnabled)
            {
               
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
                _timer = null; // 可选：释放定时器资源（实际上在GC回收时会自动处理，但显式设置为null可以避免悬挂引用）
                SetPLCAction(false);
            }
        }

        private void PLCAction1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btu = (sender as Button);
            Console.WriteLine(btu.Content);
            if (btu.Content.ToString().Equals("手动使能开"))
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 3, true);
            }
            else if (btu.Content.ToString().Equals("手动使能关"))
            {
                S7.SetBitAt(PlcHelper.PlcWriteDataBuffer, 6, 3, false);

            }
        }
    }
}
