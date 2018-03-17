using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using TCPHelper;

namespace speakDemoApp
{
    public partial class AddDeviceForm : Form
    {
        ClientAsync tcpClient;

        public AddDeviceForm(ClientAsync client)
        {
            tcpClient = client;
            InitializeComponent();
        }

        private void AddDeviceForm_Load(object sender, EventArgs e)
        {
            ini();
        }

        private void ini()
        {
            tcpClient.Completed += new Action<TcpClient, EnSocketAction>((c, enAction) =>
            {
                switch (enAction)
                {
                    case EnSocketAction.ConnectTimeOut:
                        {
                            label3.Invoke(new MethodInvoker(
                               delegate
                               {
                                   label3.Text = "连接服务端超时!";
                               }
                               ));
                            break;
                        }
                }
            });
        }


        private void btn_AddDevice_Click(object sender, EventArgs e)
        {
            if (txt_DeviceIPV4.Text.Length < 6)
            {
                txt_DeviceIPV4.Focus();
                label3.Text = "请输入添加设备的IP地址";
                return;
            }
            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(txt_DeviceIPV4.Text);
            }
            catch (Exception)
            {
                label3.Text = "ip地址格式不正确，请使用正确的ip地址！";
                return;
            }
            tcpClient.ConnectAsync(ipAddress.ToString(), 65005);
            this.Close();
        }


    }
}
