using HubDeburrSystem.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace HubDeburrSystem.DataAccess
{
    public class LocalDataAccess:ILocalDataAccess
    {

        public UserModel Login(string username, string password)
        {
            if(username == null || password == null) throw new Exception("用户名或密码不能为空!");
            UserModel userModel = new UserModel();
            SqlSugarClient sDA = new SqlAccess().SystemDataAccess;
            List<UserDataModel> users = sDA.Queryable<UserDataModel>().Where(i => i.UserName == username && i.Password == password).ToList();
            if (users.Count() == 0) throw new Exception("用户名或密码错误!");
            userModel.UserName = users[0].UserName;
            userModel.Password = users[0].Password;
            userModel.UserType = users[0].UserType;
            return userModel;
        }
    }
}
