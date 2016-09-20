using SmartParkDatabase.Control;
using SmartParkDatabase.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Booking1._0.FormView
{
    public delegate void ChangeManger(bool topmost);  //委托
    /// <summary>
    /// ChangeWorkerLogin.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeWorkerLogin : Window
    {
        public event ChangeManger Changemanger;
        public ParkManagerControl pmc;
        public ParkControl pc;
        public ChangeWorkerLogin()
        {
            InitializeComponent();
            pmc = new ParkManagerControl();
            pc = new ParkControl();
            cbxsource();
        }

        private void BTlogin_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUsername.Text != string.Empty && TxtPassword.Password != string.Empty && RegisterType.Text != string.Empty)
            {
                if (RegisterType.Text == "管理员")
                {
                    ManagerLoginEntity ManagerLogin = pmc.ParkManagerLogin(TxtUsername.Text, TxtPassword.Password, int.Parse(pc.GetCurrentParkId().ToString()));
                    if (ManagerLogin != null)
                    {
                        Model.DicValue.IsManOrDoor = "管理员";
                        Model.DicValue.MangerID = ManagerLogin.ManagerId;
                        this.Closed += ChangeWorkerLogin_Closed;this.Close();
                    }
                    else
                    {
                        MessageBox.Show("用户名/密码/类型可能输入错误！请重新输入");
                    }
                }
                if (RegisterType.Text == "门卫")
                {
                    ManagerLoginEntity ManagerLogin = pmc.DoormanLogin(TxtUsername.Text, TxtPassword.Password, int.Parse(pc.GetCurrentParkId().ToString()));
                    if (ManagerLogin != null)
                    {
                        Model.DicValue.IsManOrDoor = "门卫";
                        Model.DicValue.MangerID = ManagerLogin.ManagerId;
                        this.Closed += ChangeWorkerLogin_Closed; this.Close();
                    }
                    else
                    {
                        MessageBox.Show("用户名/密码/类型可能输入错误！请重新输入");
                    }
                }
            }
            else { MessageBox.Show("用户名/密码/类型不能为空!"); }
        }

        void ChangeWorkerLogin_Closed(object sender, EventArgs e)
        {
            Changemanger(true);
        }
        public partial class aa:ParkForm
        {

        }
        private void cbxsource()
        {
            RegisterType.Items.Add("管理员");
            RegisterType.Items.Add("门卫");
        }
    }
}
