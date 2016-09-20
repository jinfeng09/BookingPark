using Booking1._0.BLL;
using Booking1._0.Model;
using SmartParkDatabase.Control;
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
    /// AddMember.xaml 的交互逻辑
    /// </summary>
    public partial class AddMember : Window
    {
        public ParkMemberControl pmc;
        public ParkControl pc;
        public ParkTicketControl ptc;
        public int membertypeid;
        TextLimit tl = new TextLimit();
        public AddMember()
        {
            InitializeComponent();
            pmc = new ParkMemberControl();
            pc = new ParkControl();
            ptc = new ParkTicketControl();
            DicValue.typelist = new List<Dictionary<string, int>>();
            Getparkinfo();
            GetAllMemberType();
        }

        private void BTParkSub_Click(object sender, RoutedEventArgs e)
        {
            if (ParkName.Text != null && NumberName.Text != null && NumberTime.Text != null && NumberPrice.Text != null)
            {
               int res= pmc.AddMemberType(NumberName.Text, int.Parse(NumberTime.Text), int.Parse(NumberPrice.Text), int.Parse(pc.GetCurrentParkId().ToString()));
               if (res > 0)
               {
                   MessageBox.Show("添加成功!");
               }
               else
               { MessageBox.Show("添加失败！"); }
            }
        }
        private void BTNumberSub_Click(object sender, RoutedEventArgs e)
        {
            if (NumLicense.Text != null && NumType != null)
            {
                int res = pmc.AddParkMember(NumLicense.Text, membertypeid, NumNickName.Text, NumPhone.Text);
              if (res > 0)
              {
                  MessageBox.Show("车牌号为"+NumLicense.Text+"的车主已成为会员");
              }
              else
              {
                  MessageBox.Show("添加失败!");
              }
            }
        }

        private void BTTicket_Click(object sender, RoutedEventArgs e)
        {
            if (TxtTicket.Text != null && TxtTime.Text != null)
            {
                int res = ptc.AddParkingTicketType(int.Parse(pc.GetCurrentParkId().ToString()),TxtTicket.Text,int.Parse(TxtTime.Text));
                if (res > 0)
                {
                    MessageBox.Show("添加成功！");
                }
                else { MessageBox.Show("添加失败!"); }
            }
        }
        private void GetAllMemberType() //获取所有会员类型
        {
            List<SmartParkDatabase.Model.Entity.MemberTypeEntity> memberTypeList=pmc.GetAllMemberType(int.Parse(pc.GetCurrentParkId().ToString()));
            Dictionary<string, int> tmp = new Dictionary<string, int>();
            for (int i = 0; i < memberTypeList.Count; i++)
            {
                NumType.Items.Add(memberTypeList[i].Name);
                tmp.Add(memberTypeList[i].Name.ToString(), memberTypeList[i].Id);
                DicValue.typelist.Add(tmp);
            }
            NumType.SelectedIndex = 0;
        }
        private void Getparkinfo() //获取停车场信息
        {
            SmartParkDatabase.Model.Entity.ParkInfoEntity pif = pc.GetParkInfo(int.Parse(pc.GetCurrentParkId().ToString()));
            if (pif != null)
            {
                if (pif.Name != null)
                {
                    ParkName.Items.Add(pif.Name);
                    ParkName.SelectedIndex = 0;
                    parkname2.Items.Add(pif.Name);
                    parkname2.SelectedIndex = 0;
                    parkname3.Items.Add(pif.Name);
                    parkname3.SelectedIndex = 0;
                }
            }
        }
        private void NumType_SelectionChanged(object sender, SelectionChangedEventArgs e) //获取会员类型ID
        {
            int index = NumType.SelectedIndex;
            Dictionary<string, int> data = DicValue.typelist[index];
            foreach (KeyValuePair<string, int> qq in data)
            {
                if (qq.Key.Equals(NumType.SelectedValue.ToString()))
                {
                    Console.WriteLine(qq.Value);
                    membertypeid = qq.Value;
                }
            }
        }

        private void textBox1_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            tl.Pasting(e);
        }
 
        private void textBox1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            tl.PreviewKeyDown(e);
        }
 
        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            tl.PreviewTextInput(e);
        }

    }
}
