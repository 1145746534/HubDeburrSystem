using CommonServiceLocator;
using HubDeburrSystem.Models;
using HubDeburrSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace HubDeburrSystem.Views.Pages
{
    /// <summary>
    /// ReportPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReportPage : UserControl
    {
        public ReportPage()
        {
            InitializeComponent();
        }

        private void ProductionDatasDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            if (dataGrid != null && dataGrid.Items.Count > 0 && dataGrid.CurrentItem != null)
            {
                //获取当前选择的轮型数据
                var data = (ProductionDatas)dataGrid.CurrentCell.Item;
                ServiceLocator.Current.GetInstance<ReportPageViewModel>().InquireWheelType = data.WheelModel;
            }
        }

        private void StatisticsDatasDataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            if (dataGrid != null && dataGrid.Items.Count > 0 && dataGrid.CurrentItem != null)
            {
                //获取当前选择的轮型数据
                var data = (StatisticsDataModel)dataGrid.CurrentCell.Item;
                ServiceLocator.Current.GetInstance<ReportPageViewModel>().InquireWheelType = data.WheelModel;
            }
        }
    }
}
