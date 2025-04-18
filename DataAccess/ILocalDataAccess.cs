using HubDeburrSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubDeburrSystem.DataAccess
{
    public interface ILocalDataAccess
    {
        UserModel Login(string username, string password);
    }
}
