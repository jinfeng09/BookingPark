using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WTY
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NetCamera_info
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] cameraName; /* 当前相机的名称 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chIP; /* IP地址 	 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chGateway; /* 网关地址	 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chNetmask; /* 子网掩码 	 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chDNS; /* DNS服务器	 */
    }

    /* 相机时间 */
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct camera_time
    {
        public Int32 Year;           /* 年 */
        public Int32 Month;          /* 月 */
        public Int32 Day;            /* 日 */
        public Int32 Hour;           /* 时 */
        public Int32 Minute;         /* 分 */
        public Int32 Second;         /* 秒 */
        public Int32 Millisecond;    /* 微妙 */
    }

    /* 识别结果坐标 */
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct plate_location
    {
        public Int32 Left;	/* 左 */
        public Int32 Top;	/* 上 */
        public Int32 Right;	/* 右 */
        public Int32 Bottom;/* 下 */
    }

    /* 识别结果 */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct plate_result
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chWTYIP;                          /* 相机IP           */
        public Int32 nFullLen;                          /* 全景图像数据大小 */
        public Int32 nPlateLen;                         /* 车牌图像数据大小 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200000 - 312)]
        public byte[] chFullImage;                      /* 全景图像数据 	*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10000)]
        public byte[] chPlateImage;                     /* 全景图像数据 	*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] chColor;                          /* 车牌颜色 		*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chLicense;                        /* 车牌号码 		*/
        public plate_location pcLocation;	    	    /* 车牌在图像中的坐标 */
        public camera_time shootTime;		    		/* 识别出车牌的时间 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] reserved;                              
    }

    /* 车牌配置参数 */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PlateIDCfg
    {
	    public Int32 nMinPlateWidth;		/*  车牌最小宽度				*/	
	    public Int32 nMaxPlateWidth;		/*  车牌最大宽度				*/	
	    public Int32 bMovingImage;		    /*  识别运动图像还是静止图像	*/	
							                /*  		1：运动				*/	
							                /*  		0：静止				*/
	    public Int32 bIsNight;		    	/*  是否是夜间模式				*/
							                /*  		1：是				*/
							                /*  		0：不是				*/
	    public Int32 nDataTransChannel;	    /*  	数据传输的通道			*/	
							                /*  		0：网络传输通道		*/
							                /*  		1：串口传输通道		*/
							                /*  		2：网络和串口同时传输*/	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public char[] szProvince0;	            /*  默认省份					*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public char[] szProvince1;	            /*  默认省份					*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public char[] szProvince2;	            /*  默认省份					*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public char[] reserved;
    }

    /* 相机版本信息 */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CameraVer_info 
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public char[] chHardwareVer;		/*  硬件版本信息		*/	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public char[] chSystemVer;		/*  系统版本信息		*/	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public char[] chApplicationVer;	/*  应用程序版本信息	*/	
    };


    /* 设备发送的数据 */
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DevData_info
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chIp;
        public IntPtr pchBuf;
        public Int32 nLen;
        public Int32 nStatus;				/* Current recv data status. 0:Normal, other:Non-normal */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] reserved;
    }


    class KHTSDK
    {
        public const int BIG_PICSTREAM_SIZE = 200000 - 312;
        public const int SMALL_PICSTREAM_SIZE = 10000;

        #region 文通智能抓拍一体机回调函数


        /************************************************************************/
        /* 回调函数: 获取报警信息的回调函数										*/
        /*		Parameters:														*/
        /*			alarmInfo[out]:		报警信息								*/
        /*		Return Value:   void											*/
        /*																		*/
        /*		Notice:															*/
        /*			一台PC连接多台设备时，此函数仅需实现一次。当区分不同设备	*/
        /*			的Alarm时，可以通过输出参数中KHT_DevData中的chIp来区分		*/
        /*																		*/
        /************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void WTYAlarmCallback(IntPtr alarmInfo);


        /************************************************************************/
		/* 回调函数: 通知相机设备通讯状态的回调函数								*/
		/*		Parameters:														*/
		/*			chWTYIP[out]:		返回设备IP								*/
		/*			nStatus[out]:		设备状态：0表示网络异常或设备异常；		*/
		/*										  1表示网络正常，设备已连接		*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
         public  delegate void WTYConnectCallback(StringBuilder chWTYIP, UInt32 nStatus);


        /************************************************************************/
        /* 回调函数: 获取Jpeg流的回调函数										*/
        /*		Parameters:														*/
        /*			JpegInfo[out]:		JPEG流数据信息							*/
        /*		Return Value:   void											*/
        /*																		*/
        /*		Notice:															*/
        /*			一台PC连接多台设备时，此函数仅需实现一次。当区分不同设备	*/
        /*			的JPEG流时，可以通过输出参数中KHT_DevData中的chIp来区分		*/
        /*																		*/
        /************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public  unsafe delegate void WTYJpegCallback(IntPtr JpegInfo);


		 
        /************************************************************************/
		/* 回调函数: 获取识别结果的回调函数										*/
		/*		Parameters:														*/
		/*			chWTYIP[out]:		收到对应设备上传的数据					*/
		/*			chPlate[out]:		车牌号码								*/
		/*			chColor[out]:		车牌颜色								*/
		/*			chFullImage[out]:	全景图数据								*/
		/*			nFullLen[out]:		全景图数据长度							*/
		/*			chPlateImage[out]:	车牌图数据								*/
		/*			nPlateLen[out]:		车牌图数据长度							*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WTYDataCallback(StringBuilder sIP, StringBuilder sPlate,
                                           StringBuilder sColor, IntPtr sFullImage, Int32 nFullLen,
                                           IntPtr sPlateImage, Int32 nPlateLen);
										   
        /************************************************************************/
		/* 回调函数: 获取识别结果的回调函数										*/
		/*		Parameters:														*/
		/*			recResult[out]:		识别结果数据							*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void WTYDataExCallback(IntPtr recResult);
		
        //Jpeg数据回调
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WTYGetJpegStreamCallback(IntPtr pData);

        #endregion

        /************************************************************************/
		/* WTY_InitSDK: 连接相机												*/
		/*		Parameters:														*/
		/*			nPort[in]:		连接相机的端口，现默认为8080				*/
		/*			hWndHandle[in]:	接收消息的窗体句柄，当为NULL时，表示无窗体  */
		/*			uMsg[in]:		用户自定义消息，当hWndHandle不为NULL时，	*/
		/*							检测到有新的车牌识别结果并准备好当前车牌	*/
		/*							缓冲区信息后，用::PostMessage 给窗口		*/
		/*							hWndHandle发送uMsg消息，其中WPARAM参数为0，	*/
		/*							LPARAM参数为0								*/
		/*			chServerIP[in]:	相机的IP地址								*/
		/*		Return Value:   int												*/
		/*							0	相机连接成功							*/
		/*							1	相机连接失败							*/
		/*		Notice:   														*/
		/*				如果采用回调的方式获取数据时，hWndHandle句柄为NULL，	*/
		/*				uMsg为0，并且注册回调函数，通知有新的数据；				*/
		/*				反之，在主窗口收到消息时，调用WTY_GetVehicleInfo获取	*/
		/*				数据。													*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_InitSDK(UInt32 nPort, IntPtr hWndHandle, UInt32 uMsg, /*ref  string*/IntPtr chServerIP);
		
		/************************************************************************/
		/* WTY_QuitSDK: 断开所有已经连接设备，释放资源							*/
		/*		Parameters:														*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto)]
        public static extern void WTY_QuitSDK();



        /************************************************************************/
        /* KHT_RegAlarmEvent: 注册获取报警信息的回调函数						*/
        /*		Parameters:														*/
        /*			AlarmInfo[in]:		KHTAlarmExCallback类型回调函数			*/
        /*		Return Value:   void											*/
        /************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegAlarmEvent", SetLastError = true)]
        public static extern void WTY_RegAlarmEvent(WTYAlarmCallback AlarmInfo);
		

		/************************************************************************/
		/* WTY_RegWTYConnEvent: 注册通讯状态的回调函数							*/
		/*		Parameters:														*/
		/*			WTYConnect[in]:		ConnectCallback类型回调函数				*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegWTYConnEvent", SetLastError = true)]
        public static extern void WTY_RegWTYConnEvent(WTYConnectCallback WTYConnect);



        /************************************************************************/
        /* KHT_RegJpegEvent: 注册获取Jpeg流的回调函数							*/
        /*		Parameters:														*/
        /*			JpegInfo[in]:		KHTJpegExCallback类型回调函数			*/
        /*		Return Value:   void											*/
        /************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegJpegEvent", SetLastError = true)]
        public static extern void WTY_RegJpegEvent(WTYJpegCallback JpegInfo);


		/************************************************************************/
		/* 回调函数: 注册接收识别数据回调函数	(老的调用方式)					*/
		/*		Parameters:														*/
		/*			recResult[out]:		识别结果数据							*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegDataEvent", SetLastError = true)]
        public static extern void WTY_RegDataEvent(WTYDataCallback WTYData);

		/************************************************************************/
		/* WTY_RegDataExEvent: 注册获取识别结果的回调函数  (新的调用方式)		*/
		/*		Parameters:														*/
		/*			WTYData[in]:		函数指针								*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegDataExEvent", SetLastError = true)]
        public static extern void WTY_RegDataExEvent(WTYDataExCallback WTYDataEx);


        /************************************************************************/
		/* WTY_CheckStatus: 主动检查设备的通讯状态							    */
		/*		Parameters:														*/
		/*			chWTYIP[in]:		要检查的相机的IP						*/
		/*		Return Value:   int												*/
		/*							0	正常									*/
		/*							1	网络不通								*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_CheckStatus(string chWTYIP);

		

        /************************************************************************/
		/* 	函数: 获取指定IP的相机识别结果，当WTY_initSDK函数中设置了窗体句		*/
		/*			柄，以及消息时，需要调用此函数来主动获取识别结果。			*/
		/*		Parameters:														*/
		/*			chWTYIP[in]:		收到对应设备上传的数据					*/
		/*			chPlate[in]:		车牌号码								*/
		/*			chColor[in]:		车牌颜色								*/
		/*			chFullImage[in]:	全景图数据								*/
		/*			nFullLen[in]:		全景图数据长度							*/
		/*			chPlateImage[in]:	车牌图数据								*/
		/*			nPlateLen[in]:		车牌图数据长度							*/
		/*		Return Value:   int												*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_GetVehicleInfo(StringBuilder chWTYIP, StringBuilder sPlate, StringBuilder nColor, IntPtr chFullImage, ref int nFullLen, IntPtr chPlateImage, ref int nPlateLen); 
        
		/************************************************************************/
		/* WTY_SetSavePath: 如果用户需要直接将图片存储，需设置存储路径，对应各	*/
		/*		个图像路径及文件名称如下，不需要存储时，可以不调用此函数。		*/
		/*		Parameters:														*/
		/*			chSavePath[in]:	文件存储路径，以"\\"结束，如："D:\\Image\\"	*/
		/*		Return Value:   void											*/
		/*																		*/
		/*		Notice:   														*/
		/*				全景图：指定目录\\设备IP\\年月日（YYYYMMDD）\\FullImage\\时分秒-毫秒__颜色_车牌号码__.jpg	*/
		/*				车牌图：指定目录\\设备IP\\年月日（YYYYMMDD）\\PlatelImage\\时分秒-毫秒__颜色_车牌号码__.jpg	*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern void WTY_SetSavePath(string chSavePath);
		
        /************************************************************************/
		/* WTY_SetTrigger: 触发识别												*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:			相机IP								*/
		/*			nCameraPort[in]:		端口								*/
		/*		Return Value:													*/
		/*			触发成功返回	0											*/
		/*			失败返回	-1												*/
		/************************************************************************/	
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_SetTrigger(string chWTYIP,Int32 nCameraPort);
        
		
		
		/************************************************************************/
		/* WTY_RebootSystem: 重启相机。											*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:		相机IP									*/
		/*			nCameraPort[in]:	相机端口								*/
		/*		Return Value:   int												*/
		/*							1	发送成功								*/
		/*							0	发送失败								*/
		/*							-1	连接相机失败							*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_RebootSystem(string chWTYIP, Int32 nCameraPort);
		
		/************************************************************************/
		/* 函数说明: 读取版本信息												*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:			相机IP								*/
		/*			nCameraPort[in]:		端口								*/
		/*			pCameraVerInfo[in]:		相机版本信息						*/
		/*		Return Value:   int												*/
		/*							0	读取成功								*/
		/*							-1	读取失败								*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_ReadVersion(string chWTYIP, Int32 nCameraPort, StringBuilder chHardwareVer, StringBuilder chSystemVer, StringBuilder chApplicationVer);
       
		/************************************************************************/
		/* WTY_SetTransContent: 控制设备上传内容						        */
		/*		Parameters:														*/
		/*			nFullImg[in]:		全景图，0表示不传，1表示传				*/
		/*			nFullImg[in]:		车牌图，0表示不传，1表示传				*/
		/*		Return Value:   int												*/
		/*							0	成功									*/
		/*							-1	失败									*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_SetTransContent(string pCameraIP,Int32 nCameraPort,Int32 nFullImg,Int32 nPlateImg);
		
		/************************************************************************/
		/* 函数说明: 控制继电器的闭合											*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:			相机IP								*/
		/*			nCameraPort[in]:		端口								*/
		/*		Return Value:   int												*/
		/*							0	设置成功								*/
		/*							-1	设置失败								*/
		/*		Notice:   														*/
		/*				通过此功能，可以在PC端通过一体机设备，来控制道闸的抬起	*/
		/*																		*/
		/*				注意：设备继电器输出信号为：开关量信号。				*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_SetRelayClose(string pCameraIP,Int32 nCameraPort);

    }
}