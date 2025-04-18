using Adapters;
using HubDeburrSystem.DataAccess;
using HubDeburrSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration = System.Configuration.Configuration;

namespace HubDeburrSystem.Public
{
    public class ConfigEdit
    {
        //读取XML，不能用中文，切记切记
        public static void ReadAppSettings(string key, out string value)
        {
            //获取Configuration对象
            //string value = "";
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            value = config.AppSettings.Settings[key].Value;
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }

        //写入XML
        public static void SetAppSettings(string key, string value)
        {
            //获取Configuration对象
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }

        //增加XML
        public static void AddAppSettings(string key, string value)
        {
            //获取Configuration对象
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //增加
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }

        //删除XML
        public static void RemoveAppSettings(string key)
        {
            //获取Configuration对象
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根据Key读取<add>元素的Value
            config.AppSettings.Settings.Remove(key);
            //一定要记得保存，写不带参数的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否则程序读取的还是之前的值（可能已装入内存）
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 系统数据写入
        /// </summary>
        /// <param name="dataName">数据名</param>
        /// <param name="dataValue">数据值</param>
        public static void SystemDatasWrite(string dataName, string dataValue)
        {
            var sDB = new SqlAccess().SystemDataAccess;
            SystemDatas data = new SystemDatas();
            data.DataName = dataName;
            data.DataValue = dataValue;
            sDB.Updateable(data).Where(x => x.DataName == dataName).ExecuteCommand();
        }

        public static void SaveListToCsv<T>(List<T> list, string filePath, params Func<T, string>[] propertySelectors)
        {
            var properties = propertySelectors.Select(ps => ps.Method.Name).ToList();

            using (var writer = new StreamWriter(filePath))
            {
                // Write the header
                writer.WriteLine(string.Join(",", properties));

                foreach (var item in list)
                {
                    var values = propertySelectors.Select(ps => ps(item)).ToList();
                    writer.WriteLine(string.Join(",", values));
                }
            }
        }
    }
}
