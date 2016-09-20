using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Booking1._0.Camera
{
    /// <summary>
    /// Camera.xaml 的交互逻辑
    /// </summary>
    public partial class Camera : Window
    {
         //连接状态
        private WTY.KHTSDK.WTYConnectCallback ConnectCallback = null;

        //JPEG流
        private WTY.KHTSDK.WTYJpegCallback JpegExCallback = null;

        //识别结果
        private WTY.KHTSDK.WTYDataExCallback DataExCallbcak = null;

        // 定义显示车牌的委托
        public unsafe delegate void delShowPlate(String strPlate, String strColor, Label lbl);

        // 定义显示识别时间的委托
        public unsafe delegate void delShowRecogniseTime(Int32 nYear,
                                        Int32 nMonth,
                                        Int32 nDay,
                                        Int32 nHour,
                                        Int32 nMinute,
                                        Int32 nSecond,
                                        Int32 nMillisecond);

        // 定义显示车牌坐标的委托
        public unsafe delegate void delShowPlateC(Int32 nLeft,
                                Int32 nTop,
                                Int32 nRight,
                                Int32 nBottom);

        //自定义消息ID   
        private const int WM_GETDATA1 = 0x0400 + 101;
        private const int WM_GETDATA2 = 0x0400 + 102;
        public readonly int BIGIMAGE_LEN = WTY.KHTSDK.BIG_PICSTREAM_SIZE;
        public readonly int SMALLIMAGE_LEN = WTY.KHTSDK.SMALL_PICSTREAM_SIZE;
        public readonly int PLATE_LEN = 20;
        public readonly int COLOR_LEN = 5;
        public readonly int IP_LEN = 5;

        bool g_bRecgoRuing = false;

        WTY.plate_result recRes1;
        WTY.plate_result recRes2;
        bool nCallbackTrigger1 = false;
        bool nCallbackTrigger2 = false;

        String sIp1;
        String sIp2;
        string txtbox1;
        string txtbox2;
        string rootpath3;
        public Camera()
        {
            InitializeComponent();
            textBox1.Text = "192.168.0.98";
            textBox2.Text = "";
            button2.IsEnabled = false;
            string path = AppDomain.CurrentDomain.BaseDirectory;//设置目录
            string rootpath = path.Substring(0, path.LastIndexOf("\\"));
            string rootpath2 = rootpath.Substring(0, rootpath.LastIndexOf("\\"));
             rootpath3 = rootpath2.Substring(0, rootpath2.LastIndexOf("\\"));
            Console.WriteLine(rootpath3);
        }

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

        //识别车牌
        public void pateShow(WTY.plate_result recResult, String fullImgFile, String plateImgFile, System.Windows.Forms.PictureBox FullImg, System.Windows.Forms.PictureBox PlateImg, Label lbl)
        {
            string fileNameTime = DateTime.Now.ToString("hh-mm-ss");
            recResult.shootTime.Year = Convert.ToInt32(DateTime.Now.Year.ToString());
            recResult.shootTime.Month = Convert.ToInt32(DateTime.Now.Month.ToString());
            recResult.shootTime.Day = Convert.ToInt32(DateTime.Now.Day.ToString());
            recResult.shootTime.Hour = Convert.ToInt32(DateTime.Now.Hour.ToString());
            recResult.shootTime.Minute = Convert.ToInt32(DateTime.Now.Minute.ToString());
            recResult.shootTime.Second = Convert.ToInt32(DateTime.Now.Second.ToString());
                              recResult.shootTime.Millisecond=2;
            //string fileNameTime = System.DateTime.Now.ToString();
            string directoryPath = recResult.chWTYIP.ToString();
            // 显示车牌
            string strLicesen = new string(recResult.chLicense);//车牌显示字符串
            string strColor = new string(recResult.chColor);
            object[] Dl = { 
                              strLicesen, 
                              strColor,
                              lbl
                          };
            // int handle = new WindowInteropHelper(this).Handle.ToInt32();
            Dispatcher.BeginInvoke(new delShowPlate(ShowPlate), Dl);

            Directory.CreateDirectory(recResult.chWTYIP.ToString());

            // 显示识别时间
            object[] D2 = { 
                              recResult.shootTime.Year, 
                              recResult.shootTime.Month, 
                              recResult.shootTime.Day, 
                              recResult.shootTime.Hour, 
                              recResult.shootTime.Minute, 
                              recResult.shootTime.Second, 
                              recResult.shootTime.Millisecond
                          };

            Dispatcher.BeginInvoke(new delShowRecogniseTime(ShowRecogniseTime), D2);


            // 显示识别坐标
            //object[] D3 = {  
            //                 recResult.pcLocation.Left, 
            //                 recResult.pcLocation.Top, 
            //                 recResult.pcLocation.Right, 
            //                 recResult.pcLocation.Bottom 
            //              };
            //Dispatcher.BeginInvoke(new delShowPlateC(ShowPlateC), D3);

            // 显示识别图像
            if (recResult.nFullLen > 0)
            {
                // 保存全景图
                FullImg.Load("111.jpg");
                System.IO.FileStream fs = new System.IO.FileStream(fullImgFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, FileShare.ReadWrite);
                //System.IO.FileStream fs = new System.IO.FileStream(directoryPath + fileNameTime + ".jpg", System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, FileShare.ReadWrite);
                try
                {
                    fs.Write(recResult.chFullImage, 0, recResult.nFullLen);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                // 将全景图显示在界面上
                FullImg.Image = null;
                FileInfo fi = new FileInfo(fullImgFile);
                if (fi.Exists)
                {
                    FullImg.Load(fullImgFile);
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

        // 显示识别时间
        public void ShowRecogniseTime(Int32 nYear,
                                        Int32 nMonth,
                                        Int32 nDay,
                                        Int32 nHour,
                                        Int32 nMinute,
                                        Int32 nSecond,
                                        Int32 nMillisecond)
        {
            string str = "识别时间为：" + nYear + "-"
                            + nMonth + "-"
                            + nDay + " "
                            + nHour + ":"
                            + nMinute + ":"
                            + nSecond + "."
                            + nMillisecond;
            listBox1.Items.Add(str);
        }

        //public void ShowPlateC(Int32 nLeft,
        //                        Int32 nTop,
        //                        Int32 nRight,
        //                        Int32 nBottom)
        //{
        //    if ((nRight != 0) || (nBottom != 0))
        //    {
        //        string str = "车牌坐标为："
        //                        + "left:" + nLeft + ", "
        //                        + "top:" + nTop + ", "
        //                        + "right:" + nRight + ", "
        //                        + "bottom:" + nBottom;
        //        listBox1.Items.Add(str);
        //    }
        //}
        // 显示车牌函数
        public void ShowPlate(String strPlate, String strColor, Label lbl)
        {
            if (strPlate.TrimEnd('\0').Length == 0)
            {
                listBox1.Items.Add("");
                listBox1.Items.Add("识别结果为：无牌车！");
                lbl.Content = "无牌车";
            }
            else
            {
                listBox1.Items.Add("");
                string str = "识别结果为：" + strColor.TrimEnd('\0') + "," + strPlate;
                listBox1.Items.Add(str);
                lbl.Content = strPlate;
                if (String.Compare("蓝", strColor) == 0)
                {
                    lbl.Foreground = Brushes.Blue;
                    lbl.Background = Brushes.White;
                    //lbl.Foreground = new SolidColorBrush(Colors.Black);
                }
                else if (String.Compare("白", strColor) == 0)
                {
                    lbl.Foreground = Brushes.White;
                    lbl.Background = Brushes.Black;
                    // lbl.Foreground = new SolidColorBrush(Colors.White);
                }
                else if (String.Compare("黄", strColor) == 0)
                {
                    lbl.Foreground = Brushes.Yellow;
                    lbl.Background = Brushes.Black;
                    //lbl.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else if (String.Compare("黑", strColor) == 0)
                {
                    lbl.Foreground = Brushes.Black;
                    lbl.Background = Brushes.White;
                    //lbl.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        // 回调方式接收识别结果
        public void CallbackFuntion()
        {
            sIp1 = textBox1.Text.ToString();
            sIp2 = textBox2.Text.ToString();
            txtbox1 = sIp1; txtbox2 = sIp2;  //新增代码
            int nNum = 0;
            int ret;
            rbMessage.IsEnabled = false;
            rbCallBack.IsChecked = true;

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
            //WTY.KHTSDK.WTY_SetSavePath("E:\\videos\\");
            WTY.KHTSDK.WTY_SetSavePath(rootpath3+"\\"+"photos");

            // IP地址1的设备初始化
            if (textBox1.Text != "")
            {
                pIP = Marshal.StringToHGlobalAnsi(textBox1.Text.Trim());
                // 链接设备
                ret = WTY.KHTSDK.WTY_InitSDK(8080, IntPtr.Zero, 0, pIP);
                if (ret != 0)
                    listBox1.Items.Add(textBox1.Text.ToString() + "初始化失败！");
                else
                {
                    nNum = 1;
                    button2.IsEnabled = true;
                    g_bRecgoRuing = true;
                    listBox1.Items.Add(textBox1.Text.ToString() + "初始化成功！");
                    button1.Content = "断开连接";
                }
            }

            // IP地址2的设备初始化
            if (textBox2.Text != "")
            {
                pIP = Marshal.StringToHGlobalAnsi(textBox2.Text.Trim());
                // 链接设备
                ret = WTY.KHTSDK.WTY_InitSDK(8080, IntPtr.Zero, 0, pIP);
                if (ret != 0)
                    listBox1.Items.Add(textBox2.Text.ToString() + "初始化失败！");
                else
                {
                    nNum = 2;
                    button2.IsEnabled = true;
                    g_bRecgoRuing = true;
                    listBox1.Items.Add(textBox2.Text.ToString() + "初始化成功！");
                    button1.Content = "断开连接";
                }
            }

            // 启用线程，用来显示识别结果到界面上
            if (nNum > 0)
            {
                Thread thread = new Thread(DeviceSelect);
                thread.Start();
            }
        }
        // 相机的连接状态
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
        public void DeviceSelect()
        {
            string sIp;
            String strFullFile1 = "FullImage1.jpg";
            String strPlateFile1 = "PlateImage1.jpg";
            String strFullFile2 = "FullImage2.jpg";
            String strPlateFile2 = "PlateImage2.jpg";
            
            while (g_bRecgoRuing == true)
            {
                nCallbackTrigger1 = true;
                if (nCallbackTrigger1 == true)
                {
                    sIp = new string(recRes1.chWTYIP);
                    if (String.Compare(sIp, txtbox1.Trim(), true) == 0)
                    {
                        // 显示IP地址1的识别数据
                        pateShow(recRes1, strFullFile1, strPlateFile1, this.pictureBox1, this.pictureBox2, PlateLabel1);
                    }
                    nCallbackTrigger1 = false;
                }
                //if (nCallbackTrigger2 == true)
                //{
                //    sIp = new string(recRes2.chWTYIP);
                //    if (String.Compare(sIp, textBox2.Text.ToString().Trim(), true) == 0)
                //    {
                //        // 显示IP地址2的识别数据
                //        pateShow(recRes2, strFullFile2, strPlateFile2, this.pictureBox2, this.pictureBox1, PlateLabel2);
                //    }
                //    nCallbackTrigger2 = false;
                //}
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (g_bRecgoRuing == true)
            {
                // 断开所有设备，并释放所有设备占用的资源
                WTY.KHTSDK.WTY_QuitSDK();
                rbMessage.IsEnabled = true;
                rbCallBack.IsEnabled = true;

                button2.IsEnabled = false;
                g_bRecgoRuing = false;
                button1.Content = "连接相机";
                listBox1.Items.Add("断开所有相机！");
            }
            else
            {
                // 回调方式获取识别结果
                if (rbCallBack.IsChecked==true)
                {
                    CallbackFuntion();
                }
                else if (rbMessage.IsChecked==true)// 消息方式获取识别结果
                {
                   // MessageFuntion();
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            int ret;
            string sIp;

            sIp = textBox1.Text.ToString();
            // IP地址1的设备发送模拟触发指令
            if (sIp.Length != 0)
            {
                ret = WTY.KHTSDK.WTY_SetTrigger(sIp, 8080);
                if (ret < 0)
                {
                    listBox1.Items.Add(sIp + "触发失败");
                }
            }

            //sIp = textBox2.Text.ToString();
            //// IP地址2的设备发送模拟触发指令
            //if (sIp.Length != 0)
            //{
            //    ret = WTY.KHTSDK.WTY_SetTrigger(sIp, 8080);
            //    if (ret < 0)
            //    {
            //        listBox1.Items.Add(sIp + "触发失败");
            //    }
            //}
        }

          //获取JPEG流的回调函数
        public unsafe void JpegCallback(IntPtr JpegInfo) //删除此条，无法在Pictrue上显示
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
    }
}