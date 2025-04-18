using HubDeburrSystem.Models;
using HubDeburrSystem.Public;
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
using System.Windows.Shapes;
using static HubDeburrSystem.Models._3DLocusParameterSettingModel;

namespace HubDeburrSystem.Views.Dialog
{
    /// <summary>
    /// _3DLocusParameterSettingsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class _3DLocusParameterSettingsDialog : Window
    {
        public _3DLocusParameterSettingsDialog()
        {
            InitializeComponent();
            this.Loaded += _3DLocusParameterSettingsDialog_Loaded;
        }

        private void _3DLocusParameterSettingsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //EndPointOffsetDistance_tbx.Text = EndPointOffsetDistance.ToString();
            EntryPointXAxisOffsetDistance_tbx.Text = EntryPointXAxisOffsetDistance.ToString();
            EntryPointYAxisOffsetDistance_tbx.Text = EntryPointYAxisOffsetDistance.ToString();
            ExitPointXAxisOffsetDistance_tbx.Text = ExitPointXAxisOffsetDistance.ToString();
            ExitPointYAxisOffsetDistance_tbx.Text = ExitPointYAxisOffsetDistance.ToString();
            EntryExitPointOffsetHeight_tbx.Text = EntryExitPointOffsetHeight.ToString();

        }

        private void Confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            if (EntryPointXAxisOffsetDistance_tbx.Text != EntryPointXAxisOffsetDistance.ToString())
            {
                try
                {
                    var value = double.Parse(EntryPointXAxisOffsetDistance_tbx.Text);
                    if (value >= -100 && value <= 100)
                    {
                        EntryPointXAxisOffsetDistance = value;
                        ConfigEdit.SetAppSettings("EntryPointXAxisOffsetDistance", EntryPointXAxisOffsetDistance_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("进刀点X轴偏移距离（相对于起点）必须大于等于-100或小于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("进刀点X轴偏移距离值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            if (EntryPointYAxisOffsetDistance_tbx.Text != EntryPointYAxisOffsetDistance.ToString())
            {
                try
                {
                    var value = double.Parse(EntryPointYAxisOffsetDistance_tbx.Text);
                    if (value >= -100 && value <= 100)
                    {
                        EntryPointYAxisOffsetDistance = value;
                        ConfigEdit.SetAppSettings("EntryPointYAxisOffsetDistance", EntryPointYAxisOffsetDistance_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("进刀点Y轴偏移距离（相对于起点）必须大于等于-100或小于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("进刀点Y轴偏移距离值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            if (ExitPointXAxisOffsetDistance_tbx.Text != ExitPointXAxisOffsetDistance.ToString())
            {
                try
                {
                    var value = double.Parse(ExitPointXAxisOffsetDistance_tbx.Text);
                    if (value >= -100 && value <= 100)
                    {
                        ExitPointXAxisOffsetDistance = value;
                        ConfigEdit.SetAppSettings("ExitPointXAxisOffsetDistance", ExitPointXAxisOffsetDistance_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("出刀点X轴偏移距离（相对于终点）必须大于等于-100或小于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("出刀点X轴偏移距离值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            if (ExitPointYAxisOffsetDistance_tbx.Text != ExitPointYAxisOffsetDistance.ToString())
            {
                try
                {
                    var value = double.Parse(ExitPointYAxisOffsetDistance_tbx.Text);
                    if (value >= -100 && value <= 100)
                    {
                        ExitPointYAxisOffsetDistance = value;
                        ConfigEdit.SetAppSettings("ExitPointYAxisOffsetDistance", ExitPointYAxisOffsetDistance_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("出刀点Y轴偏移距离（相对于终点）必须大于等于-100或小于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("出刀点Y轴偏移距离值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }

            if (EntryExitPointOffsetHeight_tbx.Text != EntryExitPointOffsetHeight.ToString())
            {
                try
                {
                    var value = double.Parse(EntryExitPointOffsetHeight_tbx.Text);
                    if (value >= 50 && value <= 100)
                    {
                        EntryExitPointOffsetHeight = value;
                        ConfigEdit.SetAppSettings("EntryExitPointOffsetHeight", EntryExitPointOffsetHeight_tbx.Text);
                    }
                    else
                    {
                        UMessageBox.Show("进出刀点偏移高度必须大于等于50或小于等于100，请重新输入！", MessageType.Error);
                        return;
                    }
                }
                catch
                {
                    UMessageBox.Show("进出刀点偏移高度值输入错误，请重新输入", MessageType.Error);
                    return;
                }
            }
            this.Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void EndPointOffsetDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void EntryExitPointOffsetHeight_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]"); // 只允许数字和小数点和-
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void EntryPointXAxisOffsetDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void EntryPointYAxisOffsetDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void ExitPointXAxisOffsetDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void ExitPointYAxisOffsetDistance_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]"); // 只允许数字和小数点
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }


    }
}
