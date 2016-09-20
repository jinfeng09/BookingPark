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
        public char[] cameraName; /* ��ǰ��������� */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chIP; /* IP��ַ 	 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chGateway; /* ���ص�ַ	 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chNetmask; /* �������� 	 */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chDNS; /* DNS������	 */
    }

    /* ���ʱ�� */
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct camera_time
    {
        public Int32 Year;           /* �� */
        public Int32 Month;          /* �� */
        public Int32 Day;            /* �� */
        public Int32 Hour;           /* ʱ */
        public Int32 Minute;         /* �� */
        public Int32 Second;         /* �� */
        public Int32 Millisecond;    /* ΢�� */
    }

    /* ʶ�������� */
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct plate_location
    {
        public Int32 Left;	/* �� */
        public Int32 Top;	/* �� */
        public Int32 Right;	/* �� */
        public Int32 Bottom;/* �� */
    }

    /* ʶ���� */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct plate_result
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chWTYIP;                          /* ���IP           */
        public Int32 nFullLen;                          /* ȫ��ͼ�����ݴ�С */
        public Int32 nPlateLen;                         /* ����ͼ�����ݴ�С */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200000 - 312)]
        public byte[] chFullImage;                      /* ȫ��ͼ������ 	*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10000)]
        public byte[] chPlateImage;                     /* ȫ��ͼ������ 	*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] chColor;                          /* ������ɫ 		*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] chLicense;                        /* ���ƺ��� 		*/
        public plate_location pcLocation;	    	    /* ������ͼ���е����� */
        public camera_time shootTime;		    		/* ʶ������Ƶ�ʱ�� */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] reserved;                              
    }

    /* �������ò��� */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PlateIDCfg
    {
	    public Int32 nMinPlateWidth;		/*  ������С���				*/	
	    public Int32 nMaxPlateWidth;		/*  ���������				*/	
	    public Int32 bMovingImage;		    /*  ʶ���˶�ͼ���Ǿ�ֹͼ��	*/	
							                /*  		1���˶�				*/	
							                /*  		0����ֹ				*/
	    public Int32 bIsNight;		    	/*  �Ƿ���ҹ��ģʽ				*/
							                /*  		1����				*/
							                /*  		0������				*/
	    public Int32 nDataTransChannel;	    /*  	���ݴ����ͨ��			*/	
							                /*  		0�����紫��ͨ��		*/
							                /*  		1�����ڴ���ͨ��		*/
							                /*  		2������ʹ���ͬʱ����*/	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public char[] szProvince0;	            /*  Ĭ��ʡ��					*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public char[] szProvince1;	            /*  Ĭ��ʡ��					*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	    public char[] szProvince2;	            /*  Ĭ��ʡ��					*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
	    public char[] reserved;
    }

    /* ����汾��Ϣ */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CameraVer_info 
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public char[] chHardwareVer;		/*  Ӳ���汾��Ϣ		*/	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public char[] chSystemVer;		/*  ϵͳ�汾��Ϣ		*/	
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
	    public char[] chApplicationVer;	/*  Ӧ�ó���汾��Ϣ	*/	
    };


    /* �豸���͵����� */
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

        #region ��ͨ����ץ��һ����ص�����


        /************************************************************************/
        /* �ص�����: ��ȡ������Ϣ�Ļص�����										*/
        /*		Parameters:														*/
        /*			alarmInfo[out]:		������Ϣ								*/
        /*		Return Value:   void											*/
        /*																		*/
        /*		Notice:															*/
        /*			һ̨PC���Ӷ�̨�豸ʱ���˺�������ʵ��һ�Ρ������ֲ�ͬ�豸	*/
        /*			��Alarmʱ������ͨ�����������KHT_DevData�е�chIp������		*/
        /*																		*/
        /************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void WTYAlarmCallback(IntPtr alarmInfo);


        /************************************************************************/
		/* �ص�����: ֪ͨ����豸ͨѶ״̬�Ļص�����								*/
		/*		Parameters:														*/
		/*			chWTYIP[out]:		�����豸IP								*/
		/*			nStatus[out]:		�豸״̬��0��ʾ�����쳣���豸�쳣��		*/
		/*										  1��ʾ�����������豸������		*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
         public  delegate void WTYConnectCallback(StringBuilder chWTYIP, UInt32 nStatus);


        /************************************************************************/
        /* �ص�����: ��ȡJpeg���Ļص�����										*/
        /*		Parameters:														*/
        /*			JpegInfo[out]:		JPEG��������Ϣ							*/
        /*		Return Value:   void											*/
        /*																		*/
        /*		Notice:															*/
        /*			һ̨PC���Ӷ�̨�豸ʱ���˺�������ʵ��һ�Ρ������ֲ�ͬ�豸	*/
        /*			��JPEG��ʱ������ͨ�����������KHT_DevData�е�chIp������		*/
        /*																		*/
        /************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public  unsafe delegate void WTYJpegCallback(IntPtr JpegInfo);


		 
        /************************************************************************/
		/* �ص�����: ��ȡʶ�����Ļص�����										*/
		/*		Parameters:														*/
		/*			chWTYIP[out]:		�յ���Ӧ�豸�ϴ�������					*/
		/*			chPlate[out]:		���ƺ���								*/
		/*			chColor[out]:		������ɫ								*/
		/*			chFullImage[out]:	ȫ��ͼ����								*/
		/*			nFullLen[out]:		ȫ��ͼ���ݳ���							*/
		/*			chPlateImage[out]:	����ͼ����								*/
		/*			nPlateLen[out]:		����ͼ���ݳ���							*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WTYDataCallback(StringBuilder sIP, StringBuilder sPlate,
                                           StringBuilder sColor, IntPtr sFullImage, Int32 nFullLen,
                                           IntPtr sPlateImage, Int32 nPlateLen);
										   
        /************************************************************************/
		/* �ص�����: ��ȡʶ�����Ļص�����										*/
		/*		Parameters:														*/
		/*			recResult[out]:		ʶ��������							*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void WTYDataExCallback(IntPtr recResult);
		
        //Jpeg���ݻص�
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WTYGetJpegStreamCallback(IntPtr pData);

        #endregion

        /************************************************************************/
		/* WTY_InitSDK: �������												*/
		/*		Parameters:														*/
		/*			nPort[in]:		��������Ķ˿ڣ���Ĭ��Ϊ8080				*/
		/*			hWndHandle[in]:	������Ϣ�Ĵ���������ΪNULLʱ����ʾ�޴���  */
		/*			uMsg[in]:		�û��Զ�����Ϣ����hWndHandle��ΪNULLʱ��	*/
		/*							��⵽���µĳ���ʶ������׼���õ�ǰ����	*/
		/*							��������Ϣ����::PostMessage ������		*/
		/*							hWndHandle����uMsg��Ϣ������WPARAM����Ϊ0��	*/
		/*							LPARAM����Ϊ0								*/
		/*			chServerIP[in]:	�����IP��ַ								*/
		/*		Return Value:   int												*/
		/*							0	������ӳɹ�							*/
		/*							1	�������ʧ��							*/
		/*		Notice:   														*/
		/*				������ûص��ķ�ʽ��ȡ����ʱ��hWndHandle���ΪNULL��	*/
		/*				uMsgΪ0������ע��ص�������֪ͨ���µ����ݣ�				*/
		/*				��֮�����������յ���Ϣʱ������WTY_GetVehicleInfo��ȡ	*/
		/*				���ݡ�													*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_InitSDK(UInt32 nPort, IntPtr hWndHandle, UInt32 uMsg, /*ref  string*/IntPtr chServerIP);
		
		/************************************************************************/
		/* WTY_QuitSDK: �Ͽ������Ѿ������豸���ͷ���Դ							*/
		/*		Parameters:														*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto)]
        public static extern void WTY_QuitSDK();



        /************************************************************************/
        /* KHT_RegAlarmEvent: ע���ȡ������Ϣ�Ļص�����						*/
        /*		Parameters:														*/
        /*			AlarmInfo[in]:		KHTAlarmExCallback���ͻص�����			*/
        /*		Return Value:   void											*/
        /************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegAlarmEvent", SetLastError = true)]
        public static extern void WTY_RegAlarmEvent(WTYAlarmCallback AlarmInfo);
		

		/************************************************************************/
		/* WTY_RegWTYConnEvent: ע��ͨѶ״̬�Ļص�����							*/
		/*		Parameters:														*/
		/*			WTYConnect[in]:		ConnectCallback���ͻص�����				*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegWTYConnEvent", SetLastError = true)]
        public static extern void WTY_RegWTYConnEvent(WTYConnectCallback WTYConnect);



        /************************************************************************/
        /* KHT_RegJpegEvent: ע���ȡJpeg���Ļص�����							*/
        /*		Parameters:														*/
        /*			JpegInfo[in]:		KHTJpegExCallback���ͻص�����			*/
        /*		Return Value:   void											*/
        /************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegJpegEvent", SetLastError = true)]
        public static extern void WTY_RegJpegEvent(WTYJpegCallback JpegInfo);


		/************************************************************************/
		/* �ص�����: ע�����ʶ�����ݻص�����	(�ϵĵ��÷�ʽ)					*/
		/*		Parameters:														*/
		/*			recResult[out]:		ʶ��������							*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegDataEvent", SetLastError = true)]
        public static extern void WTY_RegDataEvent(WTYDataCallback WTYData);

		/************************************************************************/
		/* WTY_RegDataExEvent: ע���ȡʶ�����Ļص�����  (�µĵ��÷�ʽ)		*/
		/*		Parameters:														*/
		/*			WTYData[in]:		����ָ��								*/
		/*		Return Value:   void											*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "WTY_RegDataExEvent", SetLastError = true)]
        public static extern void WTY_RegDataExEvent(WTYDataExCallback WTYDataEx);


        /************************************************************************/
		/* WTY_CheckStatus: ��������豸��ͨѶ״̬							    */
		/*		Parameters:														*/
		/*			chWTYIP[in]:		Ҫ���������IP						*/
		/*		Return Value:   int												*/
		/*							0	����									*/
		/*							1	���粻ͨ								*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_CheckStatus(string chWTYIP);

		

        /************************************************************************/
		/* 	����: ��ȡָ��IP�����ʶ��������WTY_initSDK�����������˴����		*/
		/*			�����Լ���Ϣʱ����Ҫ���ô˺�����������ȡʶ������			*/
		/*		Parameters:														*/
		/*			chWTYIP[in]:		�յ���Ӧ�豸�ϴ�������					*/
		/*			chPlate[in]:		���ƺ���								*/
		/*			chColor[in]:		������ɫ								*/
		/*			chFullImage[in]:	ȫ��ͼ����								*/
		/*			nFullLen[in]:		ȫ��ͼ���ݳ���							*/
		/*			chPlateImage[in]:	����ͼ����								*/
		/*			nPlateLen[in]:		����ͼ���ݳ���							*/
		/*		Return Value:   int												*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_GetVehicleInfo(StringBuilder chWTYIP, StringBuilder sPlate, StringBuilder nColor, IntPtr chFullImage, ref int nFullLen, IntPtr chPlateImage, ref int nPlateLen); 
        
		/************************************************************************/
		/* WTY_SetSavePath: ����û���Ҫֱ�ӽ�ͼƬ�洢�������ô洢·������Ӧ��	*/
		/*		��ͼ��·�����ļ��������£�����Ҫ�洢ʱ�����Բ����ô˺�����		*/
		/*		Parameters:														*/
		/*			chSavePath[in]:	�ļ��洢·������"\\"�������磺"D:\\Image\\"	*/
		/*		Return Value:   void											*/
		/*																		*/
		/*		Notice:   														*/
		/*				ȫ��ͼ��ָ��Ŀ¼\\�豸IP\\�����գ�YYYYMMDD��\\FullImage\\ʱ����-����__��ɫ_���ƺ���__.jpg	*/
		/*				����ͼ��ָ��Ŀ¼\\�豸IP\\�����գ�YYYYMMDD��\\PlatelImage\\ʱ����-����__��ɫ_���ƺ���__.jpg	*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern void WTY_SetSavePath(string chSavePath);
		
        /************************************************************************/
		/* WTY_SetTrigger: ����ʶ��												*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:			���IP								*/
		/*			nCameraPort[in]:		�˿�								*/
		/*		Return Value:													*/
		/*			�����ɹ�����	0											*/
		/*			ʧ�ܷ���	-1												*/
		/************************************************************************/	
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_SetTrigger(string chWTYIP,Int32 nCameraPort);
        
		
		
		/************************************************************************/
		/* WTY_RebootSystem: ���������											*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:		���IP									*/
		/*			nCameraPort[in]:	����˿�								*/
		/*		Return Value:   int												*/
		/*							1	���ͳɹ�								*/
		/*							0	����ʧ��								*/
		/*							-1	�������ʧ��							*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_RebootSystem(string chWTYIP, Int32 nCameraPort);
		
		/************************************************************************/
		/* ����˵��: ��ȡ�汾��Ϣ												*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:			���IP								*/
		/*			nCameraPort[in]:		�˿�								*/
		/*			pCameraVerInfo[in]:		����汾��Ϣ						*/
		/*		Return Value:   int												*/
		/*							0	��ȡ�ɹ�								*/
		/*							-1	��ȡʧ��								*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_ReadVersion(string chWTYIP, Int32 nCameraPort, StringBuilder chHardwareVer, StringBuilder chSystemVer, StringBuilder chApplicationVer);
       
		/************************************************************************/
		/* WTY_SetTransContent: �����豸�ϴ�����						        */
		/*		Parameters:														*/
		/*			nFullImg[in]:		ȫ��ͼ��0��ʾ������1��ʾ��				*/
		/*			nFullImg[in]:		����ͼ��0��ʾ������1��ʾ��				*/
		/*		Return Value:   int												*/
		/*							0	�ɹ�									*/
		/*							-1	ʧ��									*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_SetTransContent(string pCameraIP,Int32 nCameraPort,Int32 nFullImg,Int32 nPlateImg);
		
		/************************************************************************/
		/* ����˵��: ���Ƽ̵����ıպ�											*/
		/*		Parameters:														*/
		/*			pCameraIP[in]:			���IP								*/
		/*			nCameraPort[in]:		�˿�								*/
		/*		Return Value:   int												*/
		/*							0	���óɹ�								*/
		/*							-1	����ʧ��								*/
		/*		Notice:   														*/
		/*				ͨ���˹��ܣ�������PC��ͨ��һ����豸�������Ƶ�բ��̧��	*/
		/*																		*/
		/*				ע�⣺�豸�̵�������ź�Ϊ���������źš�				*/
		/************************************************************************/
        [DllImport("WTY.dll", CharSet = CharSet.Ansi)]
        public static extern int WTY_SetRelayClose(string pCameraIP,Int32 nCameraPort);

    }
}