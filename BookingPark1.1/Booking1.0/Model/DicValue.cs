using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking1._0.Model
{
    public class DicValue
    {
        private static string _rootpath; //根目录

        public static string Rootpath
        {
            get { return DicValue._rootpath; }
            set { DicValue._rootpath = value; }
        }
        private static string _Intopicpath="1";//进入图片路径

        public static string Intopicpath
        {
            get { return DicValue._Intopicpath; }
            set { DicValue._Intopicpath = value; }
        }

        private static string _Outpicpath="1";//出来图片路径

        public static string Outpicpath
        {
            get { return DicValue._Outpicpath; }
            set { DicValue._Outpicpath = value; }
        }
        private static string _intotime;//进入时间

        public static string Intotime
        {
            get { return DicValue._intotime; }
            set { DicValue._intotime = value; }
        }
        private static string _outtime;//离开时间

        public static string Outtime
        {
            get { return DicValue._outtime; }
            set { DicValue._outtime = value; }
        }

        private static string _MangerName; //存储管理员昵称

        public static string MangerName
        {
            get { return DicValue._MangerName; }
            set { DicValue._MangerName = value; }
        }

        private static int _MangerID; //存储管理员ID

        public static int MangerID
        {
            get { return DicValue._MangerID; }
            set { DicValue._MangerID = value; }
        }

        private static int _MangerParkID=-1;//管理员所在停车场ID

        public static int MangerParkID
        {
            get { return DicValue._MangerParkID; }
            set { DicValue._MangerParkID = value; }
        }

        private static string _IsManOrDoor; //判断是门卫还是管理员

        public static string IsManOrDoor
        {
            get { return DicValue._IsManOrDoor; }
            set { DicValue._IsManOrDoor = value; }
        }

        private static int _PayParkingID=0; //支付ID

        public static int PayParkingID
        {
            get { return DicValue._PayParkingID; }
            set { DicValue._PayParkingID = value; }
        }

        public  static SmartParkDatabase.Model.Entity.ParkingPayInfoEntity current; //使用优惠券的data
        public static List<Dictionary<string, int>> typelist = null;  //存储会员类型ID
        public static List<Dictionary<string, int>> TicketTypeList = null;//存储优惠券类型ID
    }
}
