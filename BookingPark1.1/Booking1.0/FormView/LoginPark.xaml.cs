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
    /// <summary>
    /// LoginPark.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPark : Window
    {
        public ParkManagerControl pmc;
        public ParkControl pc;
        public LoginPark()
        {
                InitializeComponent();
                pmc = new ParkManagerControl();
                pc = new ParkControl();
                cbxsource();
                SystemControl sc = new SystemControl();
                bool res = sc.CheckDatabaseOrCreate();
                if (res == false)
                {
                    MessageBox.Show("创建数据库失败!请检查数据库用户名密码是否正确");
                    this.Close();
                }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            RegisterPark lp = new RegisterPark();
            this.Hide();
            lp.Show();
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
                        this.Hide();
                        ParkForm pf = new ParkForm();
                        pf.Show();
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
                    this.Hide();
                    ParkForm pf = new ParkForm();
                    pf.Show();
                }
                else
                {
                    MessageBox.Show("用户名/密码/类型可能输入错误！请重新输入");
                }
                }
            }
            else { MessageBox.Show("用户名/密码/类型不能为空!"); }
        }
        private void cbxsource()
        {
            RegisterType.Items.Add("管理员");
            RegisterType.Items.Add("门卫");
        }

        private void RegisterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SystemControl sc = new SystemControl();
            sc.Close();
        }


    }
}
