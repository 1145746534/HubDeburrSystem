using HubDeburrSystem.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.DataAccess
{
    public class SqlAccess
    {
        /// <summary>
        /// 系统数据访问
        /// </summary>
        public SqlSugarClient SystemDataAccess;
        /// <summary>
        /// 生产数据访问
        /// </summary>
        public SqlSugarClient ProductionDataAccess;
        /// <summary>
        /// 源轨迹数据访问
        /// </summary>
        public SqlSugarClient SourceLocusDataAccess;
        /// <summary>
        /// 加工轨迹数据访问
        /// </summary>
        public SqlSugarClient ProcessingLocusDataAccess;
        /// <summary>
        /// 创建数据库
        /// </summary>
        public SqlAccess()
        {
            //用于存储系统用户数据、配置数据、LOG等的数据库
            SystemDataAccess = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = @"Data Source=" + Environment.CurrentDirectory + @"\SystemData.db",
                
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            SystemDataAccess.DbMaintenance.CreateDatabase();

            //用于存储历史生产数据的数据库
            ProductionDataAccess = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = @"Data Source=" + Environment.CurrentDirectory + @"\ProductionData.db",
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            ProductionDataAccess.DbMaintenance.CreateDatabase();

            //用于存储由模板制作时生成的原始轨迹数据的数据库
            SourceLocusDataAccess = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = @"Data Source=" + Environment.CurrentDirectory + @"\LocusData.db",
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            SourceLocusDataAccess.DbMaintenance.CreateDatabase();

            //用于存储由位置管理时生成的加工轨迹数据的数据库
            ProcessingLocusDataAccess = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = @"Data Source=" + Environment.CurrentDirectory + @"\ProcessingLocusData.db",
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            ProcessingLocusDataAccess.DbMaintenance.CreateDatabase();
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        public void InitializeTable()
        {
            SystemDataAccess.CodeFirst.InitTables(typeof(UserDataModel));
            SystemDataAccess.CodeFirst.InitTables(typeof(TemplateDataModel));
            SystemDataAccess.CodeFirst.InitTables(typeof(SystemDatas));
            ProductionDataAccess.CodeFirst.InitTables(typeof(ProductionDatas));
        }
    }
}
