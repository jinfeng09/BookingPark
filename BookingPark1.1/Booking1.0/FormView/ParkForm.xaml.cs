using SmartParkDatabase.Control;
using SmartParkDatabase.Model.Entity;
using SmartParkDatabase.Model.View;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Booking1._0.FormView
{
    /// <summary>
    /// ParkForm.xaml 的交互逻辑
    /// </summary>
    public partial class ParkForm : Window
    {
        public BLL.CamearHelper ch;
        public UserParkingControl upc;
        public ParkTicketControl ptc;
        private int TicketTypeId;
        System.Windows.Threading.DispatcherTimer timer;//动态显示当前时间
        public ParkForm()
        {
            InitializeComponent();
            upc = new UserParkingControl();//实例化车牌数据库函数
            ptc = new ParkTicketControl();
            ch = new BLL.CamearHelper();
            Model.DicValue.TicketTypeList = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, int>>();
            ch.getsystempath();//获得根目录
            Window w = new Window();
            startcamera(w);
            CallbackFuntion();
            GetMangerInfo();
            w.Close();
        }
        private WTY.KHTSDK.WTYConnectCallback ConnectCallback = null;
        //JPEG流
        private WTY.KHTSDK.WTYJpegCallback JpegExCallback = null;
        //识别结果
        private WTY.KHTSDK.WTYDataExCallback DataExCallbcak = null;
        // 定义显示车牌的委托
        public unsafe delegate void delShowPlate(String strPlate, String strColor, System.Windows.Controls.TextBox tbx);

        // 定义显示车牌坐标的委托
        public unsafe delegate void delShowPlateC(Int32 nLeft,
                                Int32 nTop,
                                Int32 nRight,
                                Int32 nBottom);


        bool g_bRecgoRuing = true;//修改过后

        WTY.plate_result recRes1;
        WTY.plate_result recRes2;
        bool nCallbackTrigger1 = false;
        bool nCallbackTrigger2 = false;

        String sIp1;
        String sIp2;
        string txtbox1;
        string txtbox2;

        public StructType ConverBytesToStructure<StructType>(byte[] bytesBuffer)
        {
            // 检查长度
            if (bytesBuffer.Length != Marshal.SizeOf(typeof(StructType)))
            {
                throw new ArgumentException("bytesBuffer参数和structObject参数字节长度不一致。");
            }
            IntPtr bufferHandler = Marshal.AllocHGlobal(bytesBuffer.Length);
            try
            {
                for (int index = 0; index < bytesBuffer.Length; index++)
                {
                    Marshal.WriteByte(bufferHandler, index, bytesBuffer[index]);
                }
                StructType structObject = (StructType)Marshal.PtrToStructure(bufferHandler, typeof(StructType));
                return structObject;
            }
            finally { Marshal.FreeHGlobal(bufferHandler); }
        }

        // 回调方式接收识别结果
        public void CallbackFuntion()
        {
            sIp1 = Model.IPConfig.IP1;
            sIp2 = Model.IPConfig.IP2;
            txtbox1 = sIp1; txtbox2 = sIp2;  //新增代码
            int nNum = 0;
            int ret;

            IntPtr pIP = IntPtr.Zero;

            this.ConnectCallback = new WTY.KHTSDK.WTYConnectCallback(this.ConnectStatue);
            this.JpegExCallback = new WTY.KHTSDK.WTYJpegCallback(this.JpegCallback);
            this.DataExCallbcak = new WTY.KHTSDK.WTYDataExCallback(this.WTYDataExCallback);


            // 注册通讯状态的回调函数（必选）
            WTY.KHTSDK.WTY_RegWTYConnEvent(this.ConnectCallback);

            // 注册获取JPEG流的回调函数(可选)
            WTY.KHTSDK.WTY_RegJpegEvent(this.JpegExCallback);

            // 注册获取识别结果的回调函数（必选）
            WTY.KHTSDK.WTY_RegDataExEvent(this.DataExCallbcak);

            // 设置图片保存的路径。（根据需求来调用此函数）
            WTY.KHTSDK.WTY_SetSavePath(Model.DicValue.Rootpath+"\\"+"photos");
            
            // IP地址1的设备初始化
            if (sIp1 != "")
            {
                pIP = Marshal.StringToHGlobalAnsi(sIp1.Trim());
                // 链接设备
                ret = WTY.KHTSDK.WTY_InitSDK(8080, IntPtr.Zero, 0, pIP);
                if (ret != 0)
                    listBox1.Items.Add(sIp1.ToString() + "初始化失败！");
                else
                {
                    nNum = 2;
                    listBox1.Items.Add(sIp2.ToString() + "初始化成功！");
                }
            }

            // IP地址2的设备初始化
            if (sIp2 != "")
            {
                pIP = Marshal.StringToHGlobalAnsi(sIp2.Trim());
                // 链接设备
                ret = WTY.KHTSDK.WTY_InitSDK(8080, IntPtr.Zero, 0, pIP);
                if (ret != 0)
                    listBox1.Items.Add(sIp2.ToString() + "初始化失败！");
                else
                {
                    nNum = 2;
                    listBox1.Items.Add(sIp2.ToString() + "初始化成功！");
                }
            }

            // 启用线程，用来显示识别结果到界面上
            if (nNum > 0)
            {
                Thread thread = new Thread(DeviceSelect);
                thread.Start();
            }
        }

        //识别车牌
        public void pateShowInto(WTY.plate_result recResult, String fullImgFile, String plateImgFile, System.Windows.Forms.PictureBox FullImg, System.Windows.Forms.PictureBox PlateImg, System.Windows.Controls.TextBox tbx)
        {
            string fileNameTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");//文件当前时间
            //string directoryPath = recResult.chWTYIP.ToString();//文件路径
            string directoryPath = Model.DicValue.Rootpath + "\\" + "Into";
            string strLicesen = new string(recResult.chLicense);//车牌显示字符串
            string strColor = new string(recResult.chColor);
            object[] Dl = { 
                              strLicesen, 
                              strColor,
                              tbx
                          };
            Dispatcher.BeginInvoke(new delShowPlate(ShowPlateInto), Dl);//显示识别结果
            Directory.CreateDirectory(recResult.chWTYIP.ToString());
             // 显示识别图像
            if (recResult.nFullLen > 0)
            {
                // 保存全景图
                FullImg.Load("111.jpg");
                //System.IO.FileStream fs = new System.IO.FileStream(fullImgFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, FileShare.ReadWrite);
                System.IO.FileStream fs = new System.IO.FileStream(directoryPath + fileNameTime + ".jpg", System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite);
                string pathname =  fileNameTime + ".jpg";//获得图片路径
                Model.DicValue.Intopicpath = pathname.Replace("\\", "/");
                try
                {
                    fs.Write(recResult.chFullImage, 0, recResult.nFullLen);
                    fs.Close();
                }
                 catch (Exception ex)
                 {
                     Console.WriteLine(ex.Message);
                 }
                 FullImg.Image = null;
                 FileInfo fi = new FileInfo(directoryPath + fileNameTime + ".jpg");
                 if (fi.Exists)
                 {
                     FullImg.Load(directoryPath + fileNameTime + ".jpg");
                 }
            }
            if (recResult.nPlateLen > 0)
         {
             // 保存车牌小图
             PlateImg.Load("112.jpg");
             System.IO.FileStream fs = new System.IO.FileStream(plateImgFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, FileShare.ReadWrite);
             try
             {
                 fs.Write(recResult.chPlateImage, 0, recResult.nPlateLen);
                 fs.Close();
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
             }

             // 将车牌小图显示在界面上
             PlateImg.Image = null;
             FileInfo fi = new FileInfo(plateImgFile);
             if (fi.Exists)
             {
                 PlateImg.Load(plateImgFile);
             }
         }
        }
        public void pateShowOut(WTY.plate_result recResult, String fullImgFile, String plateImgFile, System.Windows.Forms.PictureBox FullImg, System.Windows.Forms.PictureBox PlateImg, System.Windows.Controls.TextBox tbx)
        {
            string fileNameTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");//文件当前时间
            //string directoryPath = recResult.chWTYIP.ToString();//文件路径
            string directoryPath = Model.DicValue.Rootpath + "\\" + "Out";
            string strLicesen = new string(recResult.chLicense);//车牌显示字符串
            string strColor = new string(recResult.chColor);
            object[] Dl = { 
                              strLicesen, 
                              strColor,
                              tbx
                          };
            Dispatcher.BeginInvoke(new delShowPlate(ShowPlateOut), Dl);//显示识别结果
            Directory.CreateDirectory(recResult.chWTYIP.ToString());
            // 显示识别图像
            if (recResult.nFullLen > 0)
            {
                // 保存全景图
                FullImg.Load("113.jpg");
                //System.IO.FileStream fs = new System.IO.FileStream(fullImgFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, FileShare.ReadWrite);
                System.IO.FileStream fs = new System.IO.FileStream(directoryPath + fileNameTime + ".jpg", System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite);
                string pathname = fileNameTime + ".jpg";//获得图片路径
                Model.DicValue.Outpicpath = pathname.Replace("\\", "/");
                try
                {
                    fs.Write(recResult.chFullImage, 0, recResult.nFullLen);
                    fs.Close(); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                FullImg.Image.Dispose();
                FullImg.Image = null; 
                FileInfo fi = new FileInfo(directoryPath + fileNameTime + ".jpg");
                if (fi.Exists)
                {
                    FullImg.Load(directoryPath + fileNameTime + ".jpg");
                }
            }
            if (recResult.nPlateLen > 0)
            {
                // 保存车牌小图
                PlateImg.Load("114.jpg");
                System.IO.FileStream fs = new System.IO.FileStream(plateImgFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, FileShare.ReadWrite);
                try
                {
                    fs.Write(recResult.chPlateImage, 0, recResult.nPlateLen);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // 将车牌小图显示在界面上
                PlateImg.Image = null;
                FileInfo fi = new FileInfo(plateImgFile);
                if (fi.Exists)
                {
                    PlateImg.Load(plateImgFile);
                }
            }


        }
        //显示识别结果
        public void ShowPlateInto(String strPlate, String strColor, System.Windows.Controls.TextBox tbx)
        {
            if (strPlate.TrimEnd('\0').Length == 0)
            {
                intoCar.Text = "无牌车";
            }
            else
            {
                listBox1.Items.Add("");
                string str = "识别结果为：" + strColor.TrimEnd('\0') + "," + strPlate.TrimEnd('\0');
                listBox1.Items.Add(str);
                tbx.Text = strPlate.TrimEnd('\0');
                if (String.Compare("蓝", strColor) == 0)
                {
                    tbx.Foreground = Brushes.Blue;
                    tbx.Background = Brushes.White;
                    //lbl.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (String.Compare("白", strColor) == 0)
                {
                    tbx.Foreground = Brushes.White;
                    tbx.Background = Brushes.Black;
                    // lbl.Foreground = new SolidColorBrush(Colors.White);
                }
                else if (String.Compare("黄", strColor) == 0)
                {
                    tbx.Foreground = Brushes.Yellow;
                    tbx.Background = Brushes.Black;
                    //lbl.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else if (String.Compare("黑", strColor) == 0)
                {
                    tbx.Foreground = Brushes.Black;
                    tbx.Background = Brushes.White;
                    //lbl.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        public void ShowPlateOut(String strPlate, String strColor, System.Windows.Controls.TextBox tbx)
        {
            if (strPlate.TrimEnd('\0').Length == 0)
            {
                outCar.Text = "无牌车";
            }
            else
            {
                listBox1.Items.Add("");
                string str = "识别结果为：" + strColor.TrimEnd('\0') + "," + strPlate.TrimEnd('\0');
                listBox1.Items.Add(str);
                tbx.Text = strPlate.TrimEnd('\0');
                if (String.Compare("蓝", strColor) == 0)
                {
                    tbx.Foreground = Brushes.Blue;
                    tbx.Background = Brushes.White;
                    //lbl.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (String.Compare("白", strColor) == 0)
                {
                    tbx.Foreground = Brushes.White;
                    tbx.Background = Brushes.Black;
                    // lbl.Foreground = new SolidColorBrush(Colors.White);
                }
                else if (String.Compare("黄", strColor) == 0)
                {
                    tbx.Foreground = Brushes.Yellow;
                    tbx.Background = Brushes.Black;
                    //lbl.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else if (String.Compare("黑", strColor) == 0)
                {
                    tbx.Foreground = Brushes.Black;
                    tbx.Background = Brushes.White;
                    //lbl.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }
        public void ConnectStatue(StringBuilder chWTYIP, UInt32 Status)
        {
            if (Status == 0)
            {
                // listBox1.Items.Add(chWTYIP + "连接失败！");
            }
            else
            {
                //listBox1.Items.Add("连接成功！");
            }
        }
        // 获取识别结果的回调函数 （新的使用方式）
        public unsafe void WTYDataExCallback(IntPtr recResult)
        {
            WTY.plate_result recRes;
            string sIp;
            /*
              注：
                客户在挂接的时候，不要将自己的事物处理放到此回调函数中。
                否则，可能会影响DLL的正常工作。
                将识别数据拷贝全局缓冲区，去处理。
            */
            WTY.plate_result recResult_ss = new WTY.plate_result();
            int size = Marshal.SizeOf(recResult_ss);
            byte[] bytes = new byte[size];
            Marshal.Copy(recResult, bytes, 0, size);
            recRes = ConverBytesToStructure<WTY.plate_result>(bytes);

            sIp = new string(recRes.chWTYIP);
            if (String.Compare(sIp, txtbox1.Trim(), true) == 0)
            {
                // 将识别结果拷贝全局缓冲区
                recRes1 = ConverBytesToStructure<WTY.plate_result>(bytes);
                // 通知显示线程去显示识别结果
                nCallbackTrigger1 = true;
            }
            if (String.Compare(sIp, txtbox2.Trim(), true) == 0)
            {
                // 将识别结果拷贝全局缓冲区
                recRes2 = ConverBytesToStructure<WTY.plate_result>(bytes);
                // 通知显示线程去显示识别结果
                nCallbackTrigger2 = true;
            }
        }

        //获取JPEG流的回调函数
        public unsafe void JpegCallback(IntPtr JpegInfo) //视频流
        {
            WTY.DevData_info jpegResult_ss = new WTY.DevData_info();
            int size = Marshal.SizeOf(jpegResult_ss);
            byte[] bytes = new byte[size];
            Marshal.Copy(JpegInfo, bytes, 0, size);
            WTY.DevData_info jpegResult_s = ConverBytesToStructure<WTY.DevData_info>(bytes);

            byte[] chJpegStream = new byte[WTY.KHTSDK.BIG_PICSTREAM_SIZE + 312];

            string devIP = new string(jpegResult_s.chIp);
            devIP = devIP.Split('\0')[0];

            //string sIp1 = textBox1.Text.ToString();
            //string sIp2 = textBox2.Text.ToString();
            if (String.Compare(sIp1, devIP, true) == 0) //连接多台相机时，通过IP地址判断是哪台相机返回的数据
            {
                if (jpegResult_s.nStatus == 0)
                {
                    if ((jpegResult_s.nLen > 0) && (jpegResult_s.pchBuf != null))
                    {
                        //把图像数据拷贝到指定内存
                        Int32 nJpegStream = jpegResult_s.nLen;
                        Array.Clear(chJpegStream, 0, chJpegStream.Length);
                        Marshal.Copy(jpegResult_s.pchBuf, chJpegStream, 0, nJpegStream);

                        //显示JPEG流
                        pictureBox1.Image = System.Drawing.Image.FromStream(new MemoryStream(chJpegStream));
                    }
                }
            }
            else if (String.Compare(sIp2, devIP, true) == 0)
            {
                if (jpegResult_s.nStatus == 0)
                {
                    if ((jpegResult_s.nLen > 0) && (jpegResult_s.pchBuf != null))
                    {
                        //把图像数据拷贝到指定内存
                        Int32 nJpegStream = jpegResult_s.nLen;
                        Array.Clear(chJpegStream, 0, chJpegStream.Length);
                        Marshal.Copy(jpegResult_s.pchBuf, chJpegStream, 0, nJpegStream);

                        //显示JPEG流
                        pictureBox2.Image = System.Drawing.Image.FromStream(new MemoryStream(chJpegStream));
                    }
                }
            }
        }

        public void DeviceSelect()
        {
            string sIp;
            String strFullFile1 = "FullImage1.jpg";
            String strPlateFile1 = "PlateImage1.jpg";
            String strFullFile2 = "FullImage2.jpg";
            String strPlateFile2 = "PlateImage2.jpg";

            while (g_bRecgoRuing == true)
            {
                if (nCallbackTrigger1 == true)
                {
                    sIp = new string(recRes1.chWTYIP);
                    if (String.Compare(sIp, txtbox1.Trim(), true) == 0)
                    {
                        // 显示IP地址1的识别数据
                        pateShowInto(recRes1, strFullFile1, strPlateFile1, this.pictureBox1, this.pictureBox3, this.intoCar);
                    }
                   nCallbackTrigger1 = false;
                }
                if (nCallbackTrigger2 == true)
                {
                    sIp = new string(recRes2.chWTYIP);
                    if (String.Compare(sIp, txtbox2.Trim(), true) == 0)
                    {
                        // 显示IP地址2的识别数据
                        pateShowOut(recRes2, strFullFile2, strPlateFile2, this.pictureBox2, this.pictureBox4, this.outCar);
                    }
                    nCallbackTrigger2 = false;
                }
            }
        }

        private void changeLicInto_Click(object sender, RoutedEventArgs e) //改变入车车牌
        {
            intoCar.IsEnabled = true;
        }
        private void changLicOut_Click(object sender, RoutedEventArgs e) //改变出车车牌
        {
            outCar.IsEnabled = true;
        }

        private void Retakes1_Click(object sender, RoutedEventArgs e)
        {
            int ret;
            string sIp;
            sIp = Model.IPConfig.IP1;
            // IP地址1的设备发送模拟触发指令
            if (sIp.Length != 0)
            {
                ret = WTY.KHTSDK.WTY_SetTrigger(sIp, 8080);
                if (ret < 0)
                {
                    listBox1.Items.Add(sIp + "触发失败");
                }
            }
        }
        //确认放行(入口)
        private void intoYes_Click(object sender, RoutedEventArgs e)
        {
            if (intoCar.Text != "")
            {
                if (Model.DicValue.Intopicpath != null)
                {
                  int res=  upc.UserParkingIn(intoCar.Text, Model.DicValue.Intopicpath, Model.DicValue.MangerParkID);
                  if (res > 0)
                  {
                      WTY.KHTSDK.WTY_SetRelayClose(Model.IPConfig.IP1, 8080);
                      intoCar.IsEnabled = false;
                      intoCar.Text = "";
                      pictureBox3.Image=null;
                      System.Windows.MessageBox.Show("已放行！");
                  }
                  else { System.Windows.MessageBox.Show("放行失败"); }
                    //Model.DicValue.Intopicpath = null;//模拟测试此条注释
                }
                else { System.Windows.MessageBox.Show("未检测到车牌识别！"); }
            }
            else { System.Windows.MessageBox.Show("未检测到车牌识别！"); }
        }
        //确认放行(出口)
        private void OutYes_Click(object sender, RoutedEventArgs e)
        {
            if (outCar.Text != "")
            {
                if (Model.DicValue.Outpicpath != null)
                {
                  int PayRes= upc.UserParkngOut(outCar.Text, Model.DicValue.Outpicpath, Model.DicValue.MangerParkID);
                  if (PayRes > 0)
                  {
                      ParkMemberControl pmc = new ParkMemberControl();
                      ViewMemberInfoEntity vmieRes = pmc.GetCurrentMemberInfo(outCar.Text, Model.DicValue.MangerParkID);
                      if (vmieRes != null)
                      {
                          MemberType.Content = "会员";
                      }
                      else { MemberType.Content = "普通用户"; }
                      ParkingPayInfoEntity PayInfo = upc.FiguringUserPayInfo(PayRes);
                      lblCarlicense.Content = outCar.Text;
                      lblPrice.Content = PayInfo.Price.ToString();
                      lblStopTime.Content = PayInfo.Hours.ToString();
                      Lostmoney.Text = PayInfo.Price.ToString();
                      outCar.Text = ""; outCar.IsEnabled = false;
                      Model.DicValue.PayParkingID = PayInfo.ParkingId;
                      Model.DicValue.current = PayInfo; 
                      pictureBox4.Image=null;
                      //Model.DicValue.Outpicpath = null;//模拟测试此条注释
                  }
                }
                else { System.Windows.MessageBox.Show("未检测到车牌识别！"); }
            }
            else { System.Windows.MessageBox.Show("未检测到车牌识别！"); }
        }
        //使用优惠券
        private void BTCoupon_Click(object sender, RoutedEventArgs e)
        {
            if (CbxTicket.IsEnabled == false)
            {
                CbxTicket.IsEnabled = true;
                BTCoupon.Content = "确认使用";
                CbxTicket.Items.Clear();
                Model.DicValue.TicketTypeList.Clear();
                GetAllTicketType();
            }
            else
            {
                if (Model.DicValue.current != null)
                {
                    BTCoupon.Content = "优惠券";
                    ParkingPayInfoEntity payinfo = upc.UseParkingTicket(Model.DicValue.current, TicketTypeId);
                    Lostmoney.Text = payinfo.Price.ToString();
                    Model.DicValue.PayParkingID = payinfo.ParkingId;
                }
                CbxTicket.IsEnabled = false;
            }
            
        }
        private void GetAllTicketType() //获取所有会员类型
        {
            ParkControl pc = new ParkControl();
            System.Collections.Generic.List<TicketTypeEntity> GetAllTicketType = ptc.GetAllParkingTicketType(int.Parse(pc.GetCurrentParkId().ToString()));
            System.Collections.Generic.Dictionary<string, int> tmp = new System.Collections.Generic.Dictionary<string, int>();
            for (int i = 0; i < GetAllTicketType.Count; i++)
            {
                CbxTicket.Items.Add(GetAllTicketType[i].Name);
                tmp.Add(GetAllTicketType[i].Name.ToString(), GetAllTicketType[i].Id);
                Model.DicValue.TicketTypeList.Add(tmp);
            }
            CbxTicket.SelectedIndex = 0;
        }

        private void Retakes2_Click(object sender, RoutedEventArgs e)
        {
            int ret;
            string sIp;
            sIp = Model.IPConfig.IP2;
            // IP地址1的设备发送模拟触发指令
            if (sIp.Length != 0)
            {
                ret = WTY.KHTSDK.WTY_SetTrigger(sIp, 8080);
                if (ret < 0)
                {
                    listBox1.Items.Add(sIp + "触发失败");
                }
            }
        }

        private void startcamera(Window w) //初始化相机动画
        {
            w.Width = 300;
            w.Height = 100;
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ResizeMode = ResizeMode.NoResize;
            w.WindowStyle = WindowStyle.None;
            var bc = new BrushConverter();
            w.Background = (Brush)bc.ConvertFrom("#8B5F65"); 
            System.Windows.Controls.Label BT = new System.Windows.Controls.Label();
            BT.Content = "正在初始化相机，请稍后.......................";
            BT.Foreground = Brushes.White;
            BT.FontSize = 15; BT.FontWeight = FontWeights.Bold;
            BT.Margin = new Thickness(0, 30, 0, 0);

            System.Windows.Controls.Grid panel = new System.Windows.Controls.Grid();
            System.Windows.Markup.IAddChild container = panel;
            container.AddChild(BT);
            container = w;
            container.AddChild(panel);
            w.Show();
        }

        public  void GetMangerInfo()//获得管理员信息
        {
            ParkManagerControl pmc = new ParkManagerControl();
            if (Model.DicValue.IsManOrDoor == "管理员")
            { 
            ParkManagerEntity GetParkManagerInfo = pmc.GetParkManager(Model.DicValue.MangerID);
            if (GetParkManagerInfo != null)
            {
                if (GetParkManagerInfo.Nickname != null)
                {  
                    this.WorkerName.Content = GetParkManagerInfo.Nickname;
                    Model.DicValue.MangerParkID = GetParkManagerInfo.ParkId;
                }
                else { System.Windows.MessageBox.Show("获取管理员昵称失败!"); }
            }
            else { System.Windows.MessageBox.Show("获取管理员信息失败！"); }
         }
            if (Model.DicValue.IsManOrDoor == "门卫")
            {
                ParkManagerEntity GetParkDoorInfo = pmc.GetParkDoorman(Model.DicValue.MangerID);
                if (GetParkDoorInfo != null)
                {
                    if (GetParkDoorInfo.Nickname != null)
                    {
                        this.WorkerName.Content = GetParkDoorInfo.Nickname;
                        Model.DicValue.MangerParkID = GetParkDoorInfo.ParkId;
                    }
                    else { System.Windows.MessageBox.Show("获取门卫昵称失败!"); }
                }
                else { System.Windows.MessageBox.Show("获取门卫信息失败！"); }
            }
        }
    
        private void ChangerWorker_Click(object sender, RoutedEventArgs e)//交班
        {
            if (WorkerName.Content == null)
            {
                ChangeWorkerLogin cwl = new ChangeWorkerLogin();
                cwl.Changemanger += cwl_Changemanger;
                cwl.ShowDialog();
            }
            else { System.Windows.MessageBox.Show("您尚未注销,请先注销！"); }
        }
        //委托获得用户信息
        void cwl_Changemanger(bool topmost)
        {
            GetMangerInfo();
        }

        private void LayoutWorker_Click(object sender, RoutedEventArgs e)//注销
        {
            ParkManagerControl pmc = new ParkManagerControl();
            if (Model.DicValue.IsManOrDoor == "管理员")
            {
                if (Model.DicValue.MangerParkID != -1)
                {
                    int res = pmc.ParkManagerLogout(Model.DicValue.MangerParkID);
                    if (res > 0)
                    {
                        WorkerName.Content = null;
                        System.Windows.MessageBox.Show("您已下线，你的下线时间为：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("下线失败!");
                    }
                }
            }
            if (Model.DicValue.IsManOrDoor == "门卫")
            {
                if (Model.DicValue.MangerParkID != -1)
                {
                    int res = pmc.DoormanLogout(Model.DicValue.MangerParkID);
                    if (res > 0)
                    {
                        WorkerName.Content = null;
                        System.Windows.MessageBox.Show("您已下线，你的下线时间为：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("下线失败!");
                    }
                }
            }
        }

        private void Daytime_Loaded(object sender, RoutedEventArgs e)//获取时间
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);   //间隔1秒
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Daytime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            SystemControl sc = new SystemControl();
            sc.Close();
            try { 
            System.Environment.Exit(0);
            System.Windows.Application.Current.Shutdown();
            }
            catch { Console.WriteLine("创建出错"); }
            
        }

        private void StartCamera_Click(object sender, RoutedEventArgs e)
        {
            // 断开所有设备，并释放所有设备占用的资源
            WTY.KHTSDK.WTY_QuitSDK();
            CallbackFuntion();
        }

        private void ChargeRelease_Click(object sender, RoutedEventArgs e)
        {
            UserParkingControl upc = new UserParkingControl();
            if (Model.DicValue.PayParkingID != 0)
            { 
             int res= upc.UserFinishPay(Model.DicValue.PayParkingID);
             if (res > 0)
             {
                 System.Windows.MessageBox.Show("支付成功！");
             }
             else
             {
                 System.Windows.MessageBox.Show("支付失败!");
             }
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme.IsOpen = true;
        }

        private void colorBlue1_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Background = new SolidColorBrush(Color.FromArgb(255,71,145,235)) ;
            Daytime.Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 145, 235));
            ChangeButton(StartGrid.Background);
            ChangeBorder(StartGrid.Background);
        }

        private void colorBlue2_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Background = new SolidColorBrush(Color.FromArgb(255, 198, 47, 47));
            Daytime.Foreground = new SolidColorBrush(Color.FromArgb(255, 198, 47, 47));
            ChangeButton(StartGrid.Background); ChangeBorder(StartGrid.Background);
        }

        private void colorOrange_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Background = new SolidColorBrush(Color.FromArgb(255, 255, 143, 87));
            Daytime.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 143, 87));
            ChangeButton(StartGrid.Background); ChangeBorder(StartGrid.Background);
        }

        private void colorGreen_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Background = new SolidColorBrush(Color.FromArgb(255,43,182,105));
            Daytime.Foreground = new SolidColorBrush(Color.FromArgb(255, 43, 182, 105));
            ChangeButton(StartGrid.Background); ChangeBorder(StartGrid.Background);
        }

        private void colorGray_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Background = new SolidColorBrush(Color.FromArgb(255,157,115,60));
            Daytime.Foreground = new SolidColorBrush(Color.FromArgb(255, 157, 115, 60));
            ChangeButton(StartGrid.Background); ChangeBorder(StartGrid.Background);
        }

        private void colorZi_Click(object sender, RoutedEventArgs e)
        {
            StartGrid.Background = new SolidColorBrush(Color.FromArgb(255,157, 60,94));
            Daytime.Foreground = new SolidColorBrush(Color.FromArgb(255, 157, 60, 94));
            ChangeButton(StartGrid.Background); ChangeBorder(StartGrid.Background);
        }
        private void ChangeButton(Brush c)//一键改变控件颜色
        {
            changLicInto.Background = c;
            changLicOut.Background = c;
            btnRefresh1.Background = c;
            btnRefresh2.Background=c;
            btnSure1.Background = c;
            btnSuer2.Background = c;
            ChangerWorker.Background = c;
            LayoutWorker.Background = c;
            StartCamera.Background = c;
            BTCoupon.Background = c;
            ChargeRelease.Background = c;
            BtnOK.Background = c;
        }
        private void ChangeBorder(Brush c) //一键改变边框
        {
            border1.BorderBrush = c;
            border2.BorderBrush = c;
            border3.BorderBrush = c;
        }

        private void WinClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tianjiahuiyuan_Click(object sender, RoutedEventArgs e)
        {
            AddMember am = new AddMember();
            am.Show();
        }

        private void CbxTicket_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CbxTicket.SelectedIndex < 0)
            {
                return;
            }
            int index = CbxTicket.SelectedIndex;
            System.Collections.Generic.Dictionary<string, int> data =Model.DicValue.TicketTypeList[index];
            foreach (System.Collections.Generic.KeyValuePair<string, int> qq in data)
            {
                if (qq.Key.Equals(CbxTicket.SelectedValue.ToString()))
                {
                    Console.WriteLine(qq.Value);
                    TicketTypeId = qq.Value;
                }
            }
        }

    }
}
