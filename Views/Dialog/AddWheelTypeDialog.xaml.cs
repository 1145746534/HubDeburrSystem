using CommonServiceLocator;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using HubDeburrSystem.ViewModel;
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

namespace HubDeburrSystem.Views.Dialog
{
    /// <summary>
    /// AddWheelTypeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddWheelTypeDialog : Window
    {
        public AddWheelTypeDialog()
        {
            InitializeComponent();
            this.Loaded += AddWheelTypeDialog_Loaded;
        }

        private void AddWheelTypeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var sDB = new SqlAccess().SystemDataAccess;
            var data = sDB.Queryable<TemplateDataModel>().Max(it => it.Index);
            Index_tbx.Text = (data + 1).ToString();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            if(WheelType_tbx.Text == "" || SpokeQuantity_tbx.Text == "")
            {
                UMessageBox.Show("输入项有空值，请检查输入项！", MessageType.Default);
                return;
            }
            if(WheelType_tbx.Text.Length < 8)
            {
                UMessageBox.Show("轮型长度错误，请重新输入！", MessageType.Default);
                return;
            }
            try
            {
                var sDB = new SqlAccess().SystemDataAccess;
                var typeList = sDB.Queryable<TemplateDataModel>().Select(it => it.WheelType).ToArray();
                var result = Array.IndexOf(typeList, WheelType_tbx.Text.Trim(' '));
                if (result >= 0)
                {
                    UMessageBox.Show("轮型重复，请重新输入！", MessageType.Default);
                    return;
                }
                TemplateDataModel model = new TemplateDataModel
                {
                    Index = int.Parse(Index_tbx.Text),
                    WheelType = WheelType_tbx.Text.Trim(' '),
                    SpokeQuantity = int.Parse(SpokeQuantity_tbx.Text),
                    UnusedDays = 0,
                    ProcessingEnable = false,
                    CenterRow = 0,
                    CenterColumn = 0,
                    DarkMaxThreshold = 0,
                    LightMinThreshold = 0,
                    InnerCircleCaliperLength = 0,
                    InnerCircleRadius = 0,
                    AngularCompensation = 0,
                    LocusScale = 0,
                    OutPointPoseId = "0,0,0,0,0,0,0,0,0,0",
                    ProcessingPressure = 0.1f
                };
                //向模板数据库中添加新数据
                sDB.Insertable(model).ExecuteCommand();
                //更新保存在内存中的模板数据
                ServiceLocator.Current.GetInstance<TemplatePageViewModel>().UpdateWheelDatas(WheelType_tbx.Text.Trim(' '));
                this.Close();
            }
            catch (Exception ex)
            {
                UMessageBox.Show(ex.Message, MessageType.Error);
            }
        }

        private void WheelType_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-z_]");//只允许数字、小写字母和下划线
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void SpokeQuantity_tbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]"); // 只允许数字
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true; 
            }
        }
    }
}
