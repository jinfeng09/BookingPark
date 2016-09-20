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
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : Window
    {
        public BLL.CamearHelper ch;
        public ServerPreferencesControl cc;
        public StartWindow()
        {
            ch = new BLL.CamearHelper();
            bool res = ch.getMysqlConfig();
            if (res == true)
            {
                LoginPark lp = new LoginPark();
                lp.ShowDialog();
                this.Close();
            }
            else
            {
                InitializeComponent();
                cc = new ServerPreferencesControl();
                cc.LoadDefaultConfig();
                //传参数到UI界面
                ServerEntity SQLdata= cc.GetServerConfig();
                txtservername.Text = SQLdata.Server;
                txtport.Text = SQLdata.Port.ToString();
                txtsqlname.Text = SQLdata.Database;
                txtusername.Text = SQLdata.User;
                txtpwd.Text = SQLdata.Password;
            }
        }

        private void BTSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (txtpwd.Text != null)
            {
                cc.SetPassword(txtpwd.Text);
                    MessageBox.Show("提交成功，单机确定进入登陆界面");
                    LoginPark lp = new LoginPark();
                    lp.Show();
                    this.Close();
            }
            else { MessageBox.Show("密码不能为空!"); }

        }
    }
}
