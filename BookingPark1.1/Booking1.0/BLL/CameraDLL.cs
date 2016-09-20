using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Booking1._0.BLL
{
    public class CameraDLL
    {
        public class CameraConfig
        {
            private WTY.KHTSDK.WTYConnectCallback ConnectCallback = null;
            //JPEG流
            private WTY.KHTSDK.WTYJpegCallback JpegExCallback = null;
            //识别结果
            private WTY.KHTSDK.WTYDataExCallback DataExCallbcak = null;
            // 定义显示车牌的委托
            public unsafe delegate void delShowPlate(String strPlate, String strColor, System.Windows.Controls.TextBox tbx);

            public System.Windows.Threading.Dispatcher Dispatcher;
            public System.Windows.Forms.PictureBox picbox1;
            public System.Windows.Forms.PictureBox picbox2;
            public System.Windows.Forms.PictureBox picbox3;
            public System.Windows.Forms.PictureBox picbox4;
            public System.Windows.Controls.TextBox tbx1;
            public System.Windows.Controls.TextBox tbx2;
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
            public void CallbackFuntion(System.Windows.Controls.ListBox listBox1)
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
                WTY.KHTSDK.WTY_SetSavePath(Model.DicValue.Rootpath + "\\" + "photos");

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
                    Thread thread = new Thread(DeviceSelect(picbox1,picbox2,picbox3,picbox4));
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
                    string pathname = fileNameTime + ".jpg";//获得图片路径
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
                    tbx.Text = "无牌车";
                }
                else
                {
                   //listBox1.Items.Add("");
                    string str = "识别结果为：" + strColor.TrimEnd('\0') + "," + strPlate.TrimEnd('\0');
                   // listBox1.Items.Add(str);
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
                    tbx.Text = "无牌车";
                }
                else
                {
                    //listBox1.Items.Add("");
                    string str = "识别结果为：" + strColor.TrimEnd('\0') + "," + strPlate.TrimEnd('\0');
                    //listBox1.Items.Add(str);
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
            public unsafe void JpegCallback(IntPtr JpegInfo ,System.Windows.Forms.PictureBox pic1,System.Windows.Forms.PictureBox pic2) //视频流
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
                            pic1.Image = System.Drawing.Image.FromStream(new MemoryStream(chJpegStream));
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
                            pic2.Image = System.Drawing.Image.FromStream(new MemoryStream(chJpegStream));
                        }
                    }
                }
            }

            public void DeviceSelect(System.Windows.Forms.PictureBox pic1,System.Windows.Forms.PictureBox pic2,System.Windows.Forms.PictureBox pic3,System.Windows.Forms.PictureBox pic4,System.Windows.Controls.TextBox tbx1,System.Windows.Controls.TextBox tbx2)
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
                            pateShowInto(recRes1, strFullFile1, strPlateFile1,pic1, pic3, tbx1);
                        }
                        nCallbackTrigger1 = false;
                    }
                    if (nCallbackTrigger2 == true)
                    {
                        sIp = new string(recRes2.chWTYIP);
                        if (String.Compare(sIp, txtbox2.Trim(), true) == 0)
                        {
                            // 显示IP地址2的识别数据
                            pateShowOut(recRes2, strFullFile2, strPlateFile2,pic2, pic4, tbx2);
                        }
                        nCallbackTrigger2 = false;
                    }
                }
            }

        }
    }
}
