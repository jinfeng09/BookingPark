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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Booking1._0.FormView
{
    /// <summary>
    /// RegisterPark.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterPark : Window
    {
        public ParkControl pc;
        public  ParkManagerControl pmc;
        int parkid;
        public RegisterPark()
        {
            InitializeComponent();
            pc = new ParkControl();
            pmc = new ParkManagerControl();
        }

        private void BTParkSub_Click(object sender, RoutedEventArgs e)
        {
            //RegisterResult PR = new RegisterResult();
            //PR.ShowDialog();
            if (ParkName.Text == string.Empty)
            {
                lblname.Content = "停车场姓名不能为空"; 
            }
            else { lblname.Content = ""; }
            if (ParkAdress.Text == string.Empty)
            {
                lbladress.Content = "停车场地址不能为空"; 
            }
            else { lbladress.Content = ""; }
            if (ParkCount.Text == string.Empty)
            {
                lblcount.Content = "停车场数量不能为空"; 
            }
            else { lblcount.Content = ""; }
            if (ParkPrice.Text == string.Empty)
            {
                lblprice.Content = "停车场价格不能为空"; 
            }
            else { lblprice.Content = ""; }
            if (ParkFreetime.Text == string.Empty)
            {
                lblfreetime.Content = "停车场免费不能为空"; 
            }
            else { lblfreetime.Content = ""; }

            if (ParkName.Text != string.Empty && ParkAdress.Text != string.Empty && ParkCount.Text != string.Empty && ParkPrice.Text != string.Empty && ParkFreetime.Text != string.Empty)
            {
                ParkControl pc = new ParkControl();
                int resultnum =  pc.RegisterPark(ParkName.Text, Convert.ToInt32(ParkFreetime.Text), Convert.ToInt32(ParkPrice.Text), Convert.ToInt32(ParkCount.Text), ParkAdress.Text);
                if (resultnum > 0)
                {
                    RegisterResult(ParkName.Text, ParkFreetime.Text, ParkPrice.Text, ParkCount.Text, ParkAdress.Text);
                }
                else 
                {
                    MessageBox.Show("注册失败!");
                }
            }
            else
            { MessageBox.Show("字符串不能为空"); }
            
        }

        public void RegisterResult(string parkName, string parkFreetime, string parkPrice, string parkCount, string parkAddress)
        {
            Window w = new Window();
            w.Width = 300;
            w.Height = 500;
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ResizeMode = ResizeMode.NoResize;
            Label BT = new Label();
            BT.Content = "注册成功!";
            BT.Foreground = Brushes.Red;
            BT.Margin = new Thickness(100, 10, 0, 0);
            Label lb1 = new Label();
            lb1.Content = "您注册的停车场名称：" + parkName.ToString();
            lb1.Margin = new Thickness(0, 30, 10, 0);
            Label lb2 = new Label();
            lb2.Content = "您注册的停车场价格：" + parkFreetime.ToString();
            lb2.Margin = new Thickness(0, 60, 10, 0);
            Label lb3 = new Label();
            lb3.Content = "您注册的停车场免费时间：" + parkPrice.ToString();
            lb3.Margin = new Thickness(0, 90, 10, 0);
            Label lb4 = new Label();
            lb4.Content = "您注册的停车场车位数量：" + parkCount.ToString();
            lb4.Margin = new Thickness(0, 120, 10, 0);
            Label lb5 = new Label();
            lb5.Content = "您注册的停车场地址：" + parkAddress.ToString();
            lb5.Margin = new Thickness(0, 150, 10, 0);
            Grid panel = new Grid();
            IAddChild container = panel;
            container.AddChild(BT);
            container.AddChild(lb1);
            container.AddChild(lb2);
            container.AddChild(lb3);
            container.AddChild(lb4);
            container.AddChild(lb5);
            //将面板放置到窗体中
            container = w;
            container.AddChild(panel);
            w.Show();
        }

        private void BTmangerSub_Click(object sender, RoutedEventArgs e)
        {
            BTmangerSub.IsEnabled = false;
            if (selectPark.Text == string.Empty)
            {
                lblpark.Content = "停车场不能为空";
            }
            else { lblpark.Content = ""; }
             if (Mangertype.Text == string.Empty)
            {
                lbltype.Content = "管理员不能为空";
            }
             else { lbltype.Content = ""; }
            if (Mangeraccount.Text == string.Empty)
            {
                lblaccount.Content = "账号不能为空";
            }
            else { lblaccount.Content = ""; }
            if (Mangerpwd1.Password == string.Empty)
            {
                lblpwd1.Content = "密码不能为空";
            }
            else { lblpwd1.Content = ""; }
            if (Mangerpwd2.Password == string.Empty)
            {
                lblpwd2.Content = "确认密码不能为空";
            }
            else { lblpwd2.Content = ""; }
            if (Mangernick.Text == string.Empty)
            {
                lblnick.Content = "管理员昵称不能为空";
            }
            else { lblnick.Content = ""; }
            if (selectPark.Text != string.Empty && Mangertype.Text != string.Empty && Mangeraccount.Text != string.Empty && Mangerpwd1.Password != string.Empty && Mangerpwd2.Password != string.Empty && Mangernick.Text != string.Empty)
            {
                if (Mangerpwd1.Password == Mangerpwd2.Password)
                {
                    switch (Mangertype.Text)
                    {
                        case "管理员": 
                            int resmanger= pmc.RegisterParkManager(parkid,Mangeraccount.Text,Mangerpwd2.Password,Mangernick.Text);
                            if (resmanger > 0)
                            {
                                MessageBox.Show("注册成功！");
                            }
                            else { MessageBox.Show("注册失败！"); }
                            BTmangerSub.IsEnabled = true;
                            refreshcontent();
                            break;
                        case "门卫":
                            int resdoor = pmc.RegisterDoorman(parkid,Mangeraccount.Text,Mangerpwd2.Password,Mangernick.Text);
                            if (resdoor > 0)
                            {
                                MessageBox.Show("注册成功！");
                            }
                            else { MessageBox.Show("注册失败！"); }
                            BTmangerSub.IsEnabled = true;
                            refreshcontent();
                            break;
                    }
                }
                else
                {
                    lblpwd1.Content = "请保证用户名密码一致！";
                    lblpwd2.Content = "请保证用户名密码一致！";
                    BTmangerSub.IsEnabled = true;
                }
            }
            else { BTmangerSub.IsEnabled = true; }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            { 
            selectPark.Items.Clear();
            Mangertype.Items.Clear();
            ParkInfoEntity GetParkInfo = pc.GetParkInfo(int.Parse(pc.GetCurrentParkId().ToString())); parkid = int.Parse(pc.GetCurrentParkId().ToString());
            if (GetParkInfo != null)
            { 
            selectPark.Items.Add(GetParkInfo.Name);
            Mangertype.Items.Add("管理员");
            Mangertype.Items.Add("门卫");
            }
          }
        }

        public void refreshcontent()
        {
            selectPark.Text = null;
            Mangertype.Text = null;
            Mangeraccount.Text = null;
            Mangerpwd1.Password = null;
            Mangerpwd2.Password = null;
            Mangernick.Text = null;
        }

        private void selectPark_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
           // selectPark.Items.Clear();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            LoginPark lp = new LoginPark();
            this.Hide();
            lp.Show();
        }
    }
}
