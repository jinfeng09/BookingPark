using System;
using System.IO;

namespace Booking1._0.BLL
{
   public class CamearHelper
    {
        //获取根目录
        public void getsystempath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;//设置目录
            string path2 = path.Substring(0, path.LastIndexOf("\\"));
            string path3 = path2.Substring(0, path2.LastIndexOf("\\"));
            Model.DicValue.Rootpath = path3.Substring(0, path2.LastIndexOf("\\"));
        }
        public bool getMysqlConfig()
        {
           string sqlpath= AppDomain.CurrentDomain.BaseDirectory + "share_pref"+"\\";
           if (File.Exists(sqlpath + "database.json"))
           {
               return true;
           }
           else { return false; }
        }

    }
}
