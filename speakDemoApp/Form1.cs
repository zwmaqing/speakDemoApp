using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TCPHelper;

namespace speakDemoApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            arrLst.Clear();
            clsRecDevices();
            foreach (var one in arrLst)
            {
                cmb_MicDeviceList.Items.Add(one);
            }
            if (cmb_MicDeviceList.Items.Count > 0)
            {
                cmb_MicDeviceList.SelectedIndex = 0;
            }
            cmb_SamplesPerSec.SelectedIndex = 0;
            cmb_BitsPerSample.SelectedIndex = 0;

            timer_UpdateProg.Interval = 500;
            this.timer_UpdateProg.Tick += Timer_Tick; ;//定时刷新进度

            timer_Limit.Interval = 1000 * 15;
            timer_Limit.Tick += Timer_Limit_Tick;

            UDPCommTimer.Elapsed += UDPCommTimer_Elapsed;
            UDPCommTimer.AutoReset = false;

            filePath = Path.GetTempPath();

            //SearchDev
            BindDeviceList = new BindingList<deviceInfo>(DeviceList);

            //将DataGridView里的数据源绑定成BindingList

            this.dGrid_devList.DataSource = BindDeviceList;

            startReceiveConfirmData();//

            timer_AudioRequest.Tick += Timer_AudioRequest_Tick;
            timer_AudioRequest.Interval = 700;

            timer_Countdown.Tick += Timer_Countdown_Tick;

            //加载既有设备列表
            loadDeviceInfoFromData();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (speakMulticastCommDev.Count > 0 || speakUnicastCommDev.Count > 0)
            {
                e.Cancel = true;
                MessageBox.Show("退出前请释放占用的设备！");
                return;
            }
            if (threadReceiveFile != null)
            {
                receiveFileUdpClient.Close();
                threadReceiveFile.Abort();
            }

            //threadSearchReceive
            if (threadSearchReceive != null)
            {
                receiveUdpSearchClient.Close();
                threadSearchReceive.Abort();
            }

            if (threadConfirmReceive != null)
            {
                confirmReceiveUdpClient.Close();
                threadConfirmReceive.Abort();
            }
        }


        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int mciSendString(
                string lpstrCommand,
                string lpstrReturnString,
                int uReturnLength,
                int hwndCallback
                 );
        [DllImport("winmm.dll")]
        public static extern int waveInGetNumDevs();

        [DllImport("winmm.dll", EntryPoint = "waveInGetDevCaps")]
        public static extern int waveInGetDevCapsA(int uDeviceID,
                             ref WaveInCaps lpCaps, int uSize);
        private ArrayList arrLst = new ArrayList();

        public int Count
        //to return total sound recording devices found
        {
            get { return arrLst.Count; }
        }

        public string device(int i)
        //return spesipic sound recording device name
        {
            return arrLst[i].ToString();
        }

        public void clsRecDevices() //fill sound recording devices array
        {
            int waveInDevicesCount = waveInGetNumDevs(); //get total
            if (waveInDevicesCount > 0)
            {
                for (int uDeviceID = 0; uDeviceID < waveInDevicesCount; uDeviceID++)
                {
                    WaveInCaps waveInCaps = new WaveInCaps();

                    waveInGetDevCapsA(uDeviceID, ref waveInCaps,

                                      Marshal.SizeOf(typeof(WaveInCaps)));

                    arrLst.Add(waveInCaps.szPname.Trim());
                    //clean garbage
                }
            }
        }

        private int duration = 0;
        private System.Windows.Forms.Timer timer_UpdateProg = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timer_Limit = new System.Windows.Forms.Timer();

        private System.Windows.Forms.Timer timer_Countdown = new System.Windows.Forms.Timer();

        //收发文件数据

        /// <summary>
        /// 锁
        /// </summary>
        private object _obj = new object();

        private UdpClient sendFileUdpClient;
        private UdpClient receiveFileUdpClient;

        private string SendFileGroupIp = "224.1.3.1";
        private int SendFileGroupPort = 65009;

        private Thread threadReceiveFile;

        private String fileName;
        private String filePath;



        //TCP 通信
        private ClientAsync tcpClient;

        private deviceInfo selectDevice;//用户在设备列表里选定的设备信息缓存

        private AuioRequestDev currentDev;//请求语音和退出语音时当前设备信息的缓存

        private List<deviceInfo> speakMulticastCommDev = new List<deviceInfo>();
        private List<deviceInfo> speakUnicastCommDev = new List<deviceInfo>();

        //收发文件数据

        private sendFarmesData sendFarmes;

        System.Timers.Timer UDPCommTimer = new System.Timers.Timer();

        //UDP通信计时器到时标志
        private bool udpCommTimed = false;
        //SearchDev

        public List<netCard> allCards = new List<netCard>();

        private UdpClient receiveUdpSearchClient;

        private string SearchGroupIp = "224.1.1.1";
        private int SearchGroupEndPort = 65000;//目标端口
        private IPEndPoint SearchGroupIpEndPoint;// 组播IP地址

        private Thread threadSearchReceive = null;

        private bool isReceiveJoinGtoup = true;//接收来自组播的信息

        private string currentLocalIPV4 = "";

        private List<deviceInfo> DeviceList = new List<deviceInfo>();//设备列表
        private BindingList<deviceInfo> BindDeviceList;

        private ChangeDevIPData CurrentDev;
        private int selectDevIndex = -1;

        string receiveMessageStr = "";

        //SearchDev

        //确认信息端口
        private UdpClient confirmReceiveUdpClient;//

        private int confirmReceiveUdpPort = 65006;//接收确认信息端口

        private Thread threadConfirmReceive = null;

        private uint Serial = 1;//信息序号

        private System.Windows.Forms.Timer timer_AudioRequest = new System.Windows.Forms.Timer();

        private int thisRequestDevCount = 0;

        private int busDeviceCount = 0;

        private int ConfirmDevCount = 0;

        private UInt32 DelaySecMax = 0;

        private void UDPCommTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            udpCommTimed = true;
            if (threadSearchReceive != null)
            {
                receiveUdpSearchClient.Close();
                // threadSearchReceive.Abort();
                // threadSearchReceive = null;
            }
        }

        //确认信息端口

        /// <summary>
        /// 获取当前是否是正在录制状态
        /// </summary>
        public bool IsRecordingAudio { get; private set; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            duration++;
            UpdateProgress(duration);
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="p">进度值</param>
        private void UpdateProgress(int p)
        {
            prog_RecordProg.Value = p;
        }

        /// <summary>
        /// 录制完成
        /// </summary>
        private void RecordingFinish(int t, string path, bool isCancle)
        {

        }


        /// <summary>
        /// 开始录制
        /// </summary>
        public void Start()
        {
            lock (_obj)
            {
                if (IsRecordingAudio) return;

                //麦克风 (Realtek High Definition
                int op = mciSendString("Open New Type WaveAudio Alias movie", "", 0, 0);
                int bit = mciSendString("set movie BitsPerSample 8", "", 0, 0);
                int hz = mciSendString("set movie SamplesPerSec 11025", "", 0, 0);
                int ch = mciSendString("set movie Channels 1", "", 0, 0);
                int fm = mciSendString("set movie format tag pcm", "", 0, 0);

                mciSendString("Record movie", "", 0, 0);

                duration = 1;
                timer_UpdateProg.Enabled = true;

                IsRecordingAudio = true;

                timer_Limit.Start();
                UpdateProgress(duration);
            }
        }


        /// <summary>
        /// 停止录制
        /// </summary>
        /// <param name="fileName">录音保存文件名</param>
        /// <param name="isCancle">是否取消保存录音</param>
        public void Stop(string fileName, bool isCancle = false)
        {
            lock (_obj)
            {
                if (!IsRecordingAudio) return;
                timer_UpdateProg.Enabled = false;

                // FileUtils.CheckAndCreateFolder();
                string wavPath = fileName;// @"I:\TEMP\" + "Record-" + DateTime.Now.ToString("HHmmss") + ".wav";
                string mp3Path = "resource/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp3";
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                mciSendString("stop movie", "", 0, 0);
                if (!isCancle)
                {
                    mciSendString("save movie " + wavPath, "", 0, 0);
                }
                mciSendString("close movie", "", 0, 0);

                IsRecordingAudio = false;

                int s = duration * 200;
                RecordingFinish(s % 1000 == 0 ? s / 1000 : s / 1000 + 1, basePath + mp3Path, isCancle);
            }
        }

        private void Timer_Limit_Tick(object sender, EventArgs e)
        {
            stopRecord();
            //发送
            btn_SendToDevice_Click(sender, e);
        }

        private void btn_RecordAudio_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// MouseDown 鼠标按下启动录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RecordAudio_MouseDown(object sender, MouseEventArgs e)
        {
            //是否需要协商端口
            //协商端口

            //开始录音
            fileName = DateTime.Now.ToString("HHmmss") + ".wav";

            Start();
        }

        //协商端口
        private void multicastCommNegotiatePort()
        {
            if (speakMulticastCommDev.Count > 1)
            {
                multicastGroupPort.Clear();
                isMultCommPortNegotiated = false;

                QueryPort cmd = new QueryPort(Serial++);
                var queryPortCMD = JsonConvert.SerializeObject(cmd);

                startReceiveMulticastGroupPortData();//启动接收消息
                groupSendData(queryPortCMD);//发送查询信息到设备组
            }
        }

        private void setMultCommPort()
        {
            if (isMultCommPortNegotiated)
            {
                SetCommPort cmd = new SetCommPort(Serial++, speakVoiceMultCommPort, null);
                var setMultCommPortCMD = JsonConvert.SerializeObject(cmd);

                groupSendData(setMultCommPortCMD);
            }
        }

        //收到的正在使用的语音通信组播端口号
        private List<uint> multicastGroupPort = new List<uint>();

        private uint speakVoiceMultCommPort = 65100;//协商后选定的组播端口号

        private bool isMultCommPortNegotiated = false;

        // 启动接收组播端口协商回复消息线程
        private void startReceiveMulticastGroupPortData()
        {
            if (threadSearchReceive != null)
            {
                receiveUdpSearchClient.Close();
                threadSearchReceive.Abort();
                threadSearchReceive = null;
            }

            //判断IP是否有效
            currentLocalIPV4 = GetLocalIP();
            // 创建接收套接字
            IPAddress localIp = IPAddress.Parse(currentLocalIPV4);//选定当前系统的IP
            IPEndPoint localIpEndPoint = new IPEndPoint(localIp, SearchGroupEndPort);
            try
            {
                receiveUdpSearchClient = new UdpClient(localIpEndPoint);
            }
            catch (SocketException sex)
            {
                MessageBox.Show(sex.Message, "发生错误");
                return;
            }

            // 加入组播组接收组播信息
            //！！！考虑不接受组播信息
            if (isReceiveJoinGtoup)
            {
                receiveUdpSearchClient.JoinMulticastGroup(IPAddress.Parse(SearchGroupIp));
                receiveUdpSearchClient.Ttl = 50;
            }

            udpCommTimed = false;
            UDPCommTimer.Interval = 600;
            UDPCommTimer.Start();
            // 启动接受线程
            threadSearchReceive = new Thread(receiveMulticastGroupPort);
            threadSearchReceive.IsBackground = true;
            threadSearchReceive.Start();
        }

        //接收端口协商回复信息的线程方法
        private void receiveMulticastGroupPort()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (!udpCommTimed)
            {
                string receiveMessage = "";
                try
                {
                    //关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = receiveUdpSearchClient.Receive(ref remoteIpEndPoint);
                    if (remoteIpEndPoint.Address.ToString() == currentLocalIPV4)
                    {
                        continue;
                    }
                    receiveMessage = Encoding.UTF8.GetString(receiveBytes);
                    receiveMessageStr = receiveMessage;
                    //分析信息内容
                    // displayReceiveedMsg();
                    //显示信息
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
                try
                {
                    if (!String.IsNullOrEmpty(receiveMessage) && receiveMessage.Contains("QueryPort"))
                    {
                        QueryPortResult result = JsonConvert.DeserializeObject<QueryPortResult>(receiveMessage);
                        if (!multicastGroupPort.Contains(result.Port))
                        {
                            multicastGroupPort.Add(result.Port);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }
            receiveUdpSearchClient.Close();

            if (multicastGroupPort.Count < 1)
            {
                isMultCommPortNegotiated = true;
                speakVoiceMultCommPort = 65100;
            }
            else
            {
                for (uint port = 65102; port < 65200; port += 2)
                {
                    if (!multicastGroupPort.Contains(port))
                    {
                        isMultCommPortNegotiated = true;
                        speakVoiceMultCommPort = port;
                        break;
                    }
                }
            }
            setMultCommPort();
        }

        /// <summary>
        /// MouseUp 鼠标松开结束录音，（一并发送）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RecordAudio_MouseUp(object sender, MouseEventArgs e)
        {
            if (timer_Limit.Enabled)
            {
                stopRecord();

                if (chk_IsAutoSend.Checked)
                {
                    //发送
                    btn_SendToDevice_Click(sender, e);
                }
            }
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        private void stopRecord()
        {
            timer_Limit.Stop();
            timer_UpdateProg.Stop();

            Stop(filePath + fileName);
        }

        /// <summary>
        /// 发送到设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SendToDevice_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("没有录音文件可供发送！请先录音.");
                return;
            }
            sendFarmes = new sendFarmesData();
            //原始数据分帧处理
            //c#文件流读文件 
            using (FileStream fsRead = new FileStream(filePath + fileName, FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);

                sendFarmes.OriginalData = DataFramePacker.BuildPackFrame(heByte, 0x01, 0x00);
            }

            if (speakMulticastCommDev.Count > 1)
            {
                //需要组播发送
                if (!isMultCommPortNegotiated)
                {
                    //还没有协商端口的情况

                    //启动端口协商流程
                    multicastCommNegotiatePort();
                    //在到时的方法里启动发送
                    timer_AudioRequest.Start();
                    return;
                }
                else
                {
                    //已经协商取得端口了的,启动组播流程
                    sendFileToDeviceMultComm();
                }
            }
            //如果只有一个组播设备，则把他移到单播设备列表里去
            if (speakMulticastCommDev.Count == 1 && !speakUnicastCommDev.Contains(speakMulticastCommDev[0]))
            {
                speakUnicastCommDev.Add(speakMulticastCommDev[0]);
            }
            //不需要组播了，启动单播流程
            foreach (var one in speakUnicastCommDev)
            {
                sendFarmes.UnicastReceiver.Add(new IPEndPoint(IPAddress.Parse(one.IPV4), 65009));
            }
            sendFileToDeviceUnicastComm();
        }

        private void Timer_AudioRequest_Tick(object sender, EventArgs e)
        {
            timer_AudioRequest.Stop();
            //组播发送
            sendFileToDeviceMultComm();
        }

        //单播发送语音数据
        private void sendFileToDeviceUnicastComm()
        {
            if (sendFarmes.OriginalData.Count > 0 && sendFarmes.UnicastReceiver.Count > 0)
            {
                // 启动发送线程
                Thread threadSend = new Thread(unicastCommSendDataFarmes);
                threadSend.IsBackground = true;
                threadSend.Start(sendFarmes);
            }
        }

        private void unicastCommSendDataFarmes(object data)
        {
            sendFarmesData farmes = (sendFarmesData)data;
            using (var sendFileUdpUnicastClient = new UdpClient())
            {
                Stopwatch sw;

                int count = 0;

                long usedTimeMaxTicks = 0;

                foreach (var one in farmes.OriginalData)
                {
                    sw = Stopwatch.StartNew();//开始计时
                    foreach (var oneIP in farmes.UnicastReceiver)
                    {
                        sendFileUdpUnicastClient.Send(one, one.Length, oneIP);
                    }
                    sw.Stop();
                    if (sw.ElapsedTicks > usedTimeMaxTicks)
                        usedTimeMaxTicks = sw.ElapsedTicks;//花费的时长

                    count++;

                    Thread.Sleep((int)numb_FrameInterval.Value);//等候5毫秒
                }
                if (lab_sendCount.IsHandleCreated)
                    lab_sendCount.BeginInvoke(new Action(delegate
                    {
                        //发送到UI 线程执行的代码
                        rText_RecordList.Text += String.Format("单播每帧最大时长 {0} Ticks", usedTimeMaxTicks);
                        lab_sendCount.Text = count.ToString();
                    }));
            }
        }

        /// <summary>
        /// 发送过程
        /// </summary>
        private void sendFileToDeviceMultComm()
        {
            try
            {
                // 组播模式
                IPEndPoint groupAudioIpEndPoint = new IPEndPoint(IPAddress.Parse(SendFileGroupIp), (int)speakVoiceMultCommPort);
                sendFarmes.MultCommIpEndPoint = groupAudioIpEndPoint;
            }
            catch
            {
                MessageBox.Show("目标设备IP地址错误!");
                return;
            }

            if (sendFarmes.OriginalData.Count > 0)
            {
                // 启动发送线程
                Thread threadSend = new Thread(multCommSendDataFarmes);
                threadSend.IsBackground = true;
                threadSend.Start(sendFarmes);
            }
        }

        private void Timer_Countdown_Tick(object sender, EventArgs e)
        {
            timer_Countdown.Stop();
            btn_RecordAudio.Enabled = true;
            btn_SendToDevice.Enabled = true;
        }

        private void multCommSendDataFarmes(object data)
        {
            sendFarmesData farmes = (sendFarmesData)data;
            using (sendFileUdpClient = new UdpClient())
            {
                int count = 0;
                foreach (var one in farmes.OriginalData)
                {
                    sendFileUdpClient.Send(one, one.Length, farmes.MultCommIpEndPoint);
                    count++;
                    if (lab_sendCount.IsHandleCreated)
                        lab_sendCount.BeginInvoke(new Action(delegate
                        {
                            //发送到UI 线程执行的代码
                            lab_sendCount.Text = count.ToString();
                        }));

                    Thread.Sleep((int)numb_FrameInterval.Value);//等候5毫秒
                }

                //发送完毕，显示状态信息和开启功能禁用
                if (lab_sendCount.IsHandleCreated)
                    lab_sendCount.BeginInvoke(new Action(delegate
                    {
                        //发送到UI 线程执行的代码
                        lab_sendCount.Text = "OK";

                        WaveInfo _WaveFileInfo = new WaveInfo(filePath + fileName);
                        int timeSpan = (int)(Math.Round(_WaveFileInfo.Second, 3) * 1000);

                        rText_RecordList.Text += "发送录音成功,时长：" + Math.Round(_WaveFileInfo.Second, 1).ToString() + " 秒.\n";

                        timer_Countdown.Interval = timeSpan > 100 ? timeSpan : 100;
                        timer_Countdown.Start();
                        btn_RecordAudio.Enabled = false;
                        btn_SendToDevice.Enabled = false;

                    }));
            }
        }

        private void btn_selectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                String fullPath = openFile.FileName;
                FileInfo f = new FileInfo(openFile.FileName);
                int point = fullPath.LastIndexOf('\\') + 1;
                filePath = fullPath.Substring(0, point);
                fileName = fullPath.Substring(point, fullPath.Length - point);

                // btn_sendFile.Enabled = true;


            }
        }

        /// <summary>
        /// 接收分帧发送来的数据文件 -未使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReceiveFile_Click(object sender, EventArgs e)
        {
            if (receiveFileUdpClient != null)
                receiveFileUdpClient.Close();
            if (threadReceiveFile != null)
            {
                threadReceiveFile.Abort();
            }

            // 创建接收套接字
            IPAddress localIp = IPAddress.Parse("");//本机接收IP地址
            IPEndPoint localIpEndPoint = new IPEndPoint(localIp, int.Parse(""));//本机接收端口
            receiveFileUdpClient = new UdpClient(localIpEndPoint);

            // 启动接受线程
            threadReceiveFile = new Thread(ReceiveFile);
            threadReceiveFile.IsBackground = true;
            threadReceiveFile.Start();
        }

        /// <summary>
        /// 分帧接收数据文件 -未使用
        /// </summary>
        private void ReceiveFile()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            ushort farmesTotal = 0;
            ushort farmesCount = 0;
            List<Byte[]> datas = new List<Byte[]>();
            while (true)
            {
                try
                {
                    //关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = receiveFileUdpClient.Receive(ref remoteIpEndPoint);
                    farmesTotal = DataFramePacker.FarmeTotal(receiveBytes);

                    ushort farmesIndex = DataFramePacker.FarmeIndex(receiveBytes);

                    datas.Add(receiveBytes);
                    farmesCount++;

                    //接收完毕
                    if (farmesIndex == farmesTotal)
                    {
                        Byte[] Original = DataFramePacker.BuildOriginalData(datas);

                        using (FileStream fsWrite = new FileStream(@"D:\ddd.wav", FileMode.OpenOrCreate))
                        {
                            fsWrite.Write(Original, 0, Original.Length);

                            //   if (lab_receivFileFg.IsHandleCreated)
                            //      lab_receivFileFg.BeginInvoke(new Action(delegate
                            //      {
                            //发送到UI 线程执行的代码
                            //           lab_receivFileFg.Text = "完毕";
                            //       }));
                        }
                    }
                    //显示消息内容
                }
                catch
                {

                }
            }
        }

        //SearchDev

        private void loadAllNetCards()
        {
            //获取说有网卡信息
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                //判断是否为以太网卡
                //Wireless80211         无线网卡    Ppp     宽带连接
                //Ethernet              以太网卡   
                //这里篇幅有限贴几个常用的，其他的返回值大家就自己百度吧！
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();

                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;

                    //获取该IP对象的网关
                    GatewayIPAddressInformationCollection gateways = ip.GatewayAddresses;

                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        netCard oneCard = new netCard();

                        foreach (var gateWay in gateways)
                        {
                            //如果能够Ping通网关
                            if (IsPingIP(gateWay.Address.ToString()))
                            {
                                //得到网关地址
                                oneCard.Gateway = gateWay.Address.ToString();
                                //跳出循环
                                break;
                            }
                        }

                        //InterNetwork    IPV4地址      
                        //InterNetworkV6        IPV6地址
                        //Max            MAX 位址
                        //判断是否为ipv4
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {

                            oneCard.Name = adapter.Name;
                            if (ip.DnsAddresses[0] != null)
                                oneCard.DNS = ip.DnsAddresses[0].ToString();

                            oneCard.IPV4 = ipadd.Address.ToString();//获取ip
                            if (ipadd.IPv4Mask != null)
                                oneCard.SubnetMask = ipadd.IPv4Mask.ToString();

                            allCards.Add(oneCard);
                        }
                    }
                }
            }
        }

        /// 尝试Ping指定IP是否能够Ping通      
        /// <param name="strIP">指定IP</param>
        /// <returns>true 是 false 否</returns>
        public static bool IsPingIP(string strIP)
        {
            try
            {
                Ping ping = new Ping();
                IPAddress ip;
                if (!IPAddress.TryParse(strIP, out ip))
                {
                    return false;
                }
                //接受Ping返回值
                PingReply reply = ping.Send(strIP, 1000);
                //Ping通
                return true;
            }
            catch
            {
                //Ping失败
                return false;
            }
        }

        /// <summary>  
        /// 获取当前使用的IP  
        /// </summary>  
        /// <returns></returns>  
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = ((System.Net.IPEndPoint)c.Client.LocalEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>  
        /// 获取本机主DNS  
        /// </summary>  
        /// <returns></returns>  
        public static string GetPrimaryDNS()
        {
            string result = RunApp("nslookup", "", true);
            Match m = Regex.Match(result, @"\d+\.\d+\.\d+\.\d+");
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>  
        /// 运行一个控制台程序并返回其输出参数。  
        /// </summary>  
        /// <param name="filename">程序名</param>  
        /// <param name="arguments">输入参数</param>  
        /// <returns></returns>  
        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                using (System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    //string txt = sr.ReadToEnd();  
                    //sr.Close();  
                    //if (recordLog)  
                    //{  
                    //    Trace.WriteLine(txt);  
                    //}  
                    //if (!proc.HasExited)  
                    //{  
                    //    proc.Kill();  
                    //}  
                    //上面标记的是原文，下面是我自己调试错误后自行修改的  
                    Thread.Sleep(100);           //貌似调用系统的nslookup还未返回数据或者数据未编码完成，程序就已经跳过直接执行  
                                                 //txt = sr.ReadToEnd()了，导致返回的数据为空，故睡眠令硬件反应  
                    if (!proc.HasExited)         //在无参数调用nslookup后，可以继续输入命令继续操作，如果进程未停止就直接执行  
                    {                            //txt = sr.ReadToEnd()程序就在等待输入，而且又无法输入，直接掐住无法继续运行  
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }


        private void btn_SearchDev_Click(object sender, EventArgs e)
        {
            // BindDeviceList.Clear();//清除既有设备数据
            CurrentDev = null;//清除设置设备ip命令标志
                              //  lab_devCount.Text = "0";
            var localIP = GetLocalIP();//获取电脑当前本地IP

            SearchCmd cmd = new SearchCmd("SearchDev", localIP);
            var searchCMD = JsonConvert.SerializeObject(cmd);

            startReceiveData();//启动接收消息
            groupSendData(searchCMD);//发送查询信息到设备组
        }

        // 启动接收搜索消息
        private void startReceiveData()
        {
            if (threadSearchReceive != null)
            {
                receiveUdpSearchClient.Close();
                threadSearchReceive.Abort();
                threadSearchReceive = null;
            }

            //判断IP是否有效
            currentLocalIPV4 = GetLocalIP();
            // 创建接收套接字
            IPAddress localIp = IPAddress.Parse(currentLocalIPV4);//选定当前系统的IP
            IPEndPoint localIpEndPoint = new IPEndPoint(localIp, SearchGroupEndPort);
            try
            {
                receiveUdpSearchClient = new UdpClient(localIpEndPoint);
            }
            catch (SocketException sex)
            {
                MessageBox.Show(sex.Message, "发生错误");
                return;
            }

            // 加入组播组接收组播信息
            //！！！考虑不接受组播信息
            if (isReceiveJoinGtoup)
            {
                receiveUdpSearchClient.JoinMulticastGroup(IPAddress.Parse(SearchGroupIp));
                receiveUdpSearchClient.Ttl = 50;
            }

            udpCommTimed = false;
            UDPCommTimer.Interval = 1000;
            UDPCommTimer.Start();
            // 启动接受线程
            threadSearchReceive = new Thread(ReceiveMessage);
            threadSearchReceive.IsBackground = true;
            threadSearchReceive.Start();
        }

        // 接收搜索消息线程方法
        private void ReceiveMessage()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (!udpCommTimed)
            {
                string receiveMessage = "";
                try
                {
                    //关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = receiveUdpSearchClient.Receive(ref remoteIpEndPoint);
                    if (remoteIpEndPoint.Address.ToString() == currentLocalIPV4)
                    {
                        continue;
                    }
                    receiveMessage = Encoding.UTF8.GetString(receiveBytes);
                    receiveMessageStr = receiveMessage;
                    //分析信息内容
                    // displayReceiveedMsg();
                    //显示信息
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
                try
                {
                    if (receiveMessage != "")
                    {
                        //检查是否是设置设备IP后的返回信息。
                        if (CurrentDev != null)
                        {
                            SetIPResult result = JsonConvert.DeserializeObject<SetIPResult>(receiveMessage);
                            if (result.SN == CurrentDev.SN && result.Status)
                            {
                                displayReceiveedMsg();
                            }
                        }
                        else
                        {
                            bool ex = false;
                            deviceInfo oneDev = JsonConvert.DeserializeObject<deviceInfo>(receiveMessage);
                            oneDev.IsOnLine = true;
                            oneDev.IsMulticastTo = true;
                            foreach (var one in BindDeviceList)
                            {
                                if (one.SN == oneDev.SN)
                                {
                                    one.IsMulticastTo = true;
                                    one.IsOnLine = true;
                                    one.IPV4 = oneDev.IPV4;
                                    ex = true;
                                    break;
                                }
                            }

                            if (this.dGrid_devList.IsHandleCreated && !ex && oneDev.SN.Length > 0)
                                this.dGrid_devList.BeginInvoke(new Action(delegate
                                {
                                    //发送到UI 线程执行的代码
                                    BindDeviceList.Add(oneDev);
                                    this.dGrid_devList.Refresh();
                                }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }
            }

            receiveUdpSearchClient.Close();
        }


        private void displayReceiveedMsg()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(this.displayReceiveedMsg));//检查是否需要委托执行
            }
            else
            {
                BindDeviceList[selectDevIndex].AliasName = CurrentDev.aliasName;
                BindDeviceList[selectDevIndex].IPV4 = CurrentDev.newIP;
                BindDeviceList[selectDevIndex].SubnetMask = CurrentDev.newmask;
                BindDeviceList[selectDevIndex].Gateway = CurrentDev.newgateway;
                BindDeviceList[selectDevIndex].DNS = CurrentDev.newdnsweb;
                BindDeviceList[selectDevIndex].IsDHCP = CurrentDev.Switch;
                this.dGrid_devList.Refresh();
                //实际的执行程序体
                // MessageBox.Show(msgShow);
                CurrentDev = null;
            }
        }

        // 发送搜索组播消息
        private void groupSendData(string data)
        {
            // 组播模式
            SearchGroupIpEndPoint = new IPEndPoint(IPAddress.Parse(SearchGroupIp), SearchGroupEndPort);
            // 启动发送线程发送消息
            Thread sendThread = new Thread(SendMessage);
            sendThread.IsBackground = true;
            sendThread.Start(data);
        }

        // 发送搜索消息线程方法
        private void SendMessage(object obj)
        {
            byte[] messagebytes = Encoding.UTF8.GetBytes(obj.ToString());

            using (var sendUdpClient = new UdpClient())
            {
                // 发送消息到组播或广播地址
                int state = sendUdpClient.Send(messagebytes, messagebytes.Length, SearchGroupIpEndPoint);

                sendUdpClient.Close();
            }
        }


        public string HttpGet(string Url, Dictionary<string, string> parameters)
        {
            //拼装请求参数列表  
            StringBuilder sb = new StringBuilder();
            string strParameters = "";
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> kvp in parameters)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("&");
                    }
                    sb.AppendFormat("{0}={1}", kvp.Key, kvp.Value);
                }
                strParameters = sb.ToString();
            }
            // MessageBox.Show(strParameters);
            string retString = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (strParameters == "" ? "" : "?") + strParameters);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                //  MessageBox.Show("网络错误：" + ex.Message);
                throw (ex);
            }
            return retString;
        }

        private void btn_RetrieveArea_Click(object sender, EventArgs e)
        {
            if (BindDeviceList.Count == 0)
            {
                MessageBox.Show("没有可供检索的设备，请先搜索设备。");
                return;
            }
            freeAllAppliedDev();
            tree_Area.Nodes.Clear();
            //考虑通知已经打开的设备
            //去除已选语音接受设备列表
            speakMulticastCommDev.Clear();
            speakUnicastCommDev.Clear();

            foreach (var one in BindDeviceList)
            {
                if (one.Type == "IPTRUMPET" && one.IsOnLine)
                {
                    TreeNode node = new TreeNode();
                    node.Name = one.AliasName;
                    node.Text = "IP喇叭 " + one.AliasName;
                    node.ToolTipText = one.SN;
                    node.Tag = one;
                    tree_Area.Nodes.Add(node);//插入

                }
                else if (one.Type == "IPCHPOWER" && one.IsOnLine)
                {
                    TreeNode node = new TreeNode();//设备节点
                    node.Name = one.AliasName;
                    node.Text = "网络分区功放-" + one.AliasName;
                    node.ToolTipText = one.SN;
                    node.Tag = one;

                    TreeNode nodeCH = new TreeNode();//分区总节点
                    nodeCH.Name = "拥有的物理分区";
                    nodeCH.Text = "拥有的物理分区";
                    nodeCH.Tag = "All_CH";
                    for (int i = 1; i < 17; i++)
                    {
                        TreeNode nodech = new TreeNode();
                        nodech.Name = "输出分区 " + i.ToString();
                        nodech.Text = "输出分区 " + i.ToString();
                        nodech.Tag = i;
                        nodeCH.Nodes.Add(nodech);
                    }
                    node.Nodes.Add(nodeCH);//添加分区总节点

                    TreeNode nodeGroup = new TreeNode();//分组总节点
                    nodeGroup.Name = "拥有的输出分组";
                    nodeGroup.Text = "拥有的输出分组";
                    nodeGroup.Tag = "ALL_GROUP";

                    LoginDeviceResult logon = loginDeviceHttp(one.IPV4, txt_LoginName.Text, txt_LoginPass.Text);

                    GetCHGroupsResult groupRes = getNetPowerGroups(one.IPV4, logon.Data.Token);

                    foreach (var group in groupRes.Data)
                    {
                        TreeNode nodeg = new TreeNode();
                        nodeg.Name = group.GroupName;
                        nodeg.Text = group.GroupName;
                        nodeg.Tag = group.GroupID;
                        nodeGroup.Nodes.Add(nodeg);
                    }
                    node.Nodes.Add(nodeGroup);//添加分组总节点

                    tree_Area.Nodes.Add(node);//插入
                }
            }
        }

        private LoginDeviceResult loginDeviceHttp(string IP, string name, string pass)
        {
            string url = "http://" + IP + "/API/System";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("CMD", "LogOn");
            parameters.Add("userName", name);
            parameters.Add("password", pass);

            string re = HttpGet(url, parameters);

            try
            {
                LoginDeviceResult result = JsonConvert.DeserializeObject<LoginDeviceResult>(re);
                return result;
            }
            catch
            {
                return null;
            }
        }

        private GetCHGroupsResult getNetPowerGroups(string IP, string token)
        {
            string url = "http://" + IP + "/API/Group";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("CMD", "GetCHGroups");
            parameters.Add("Range", "1-10");
            parameters.Add("Token", token);

            string re = HttpGet(url, parameters);

            try
            {
                GetCHGroupsResult result = JsonConvert.DeserializeObject<GetCHGroupsResult>(re);
                return result;
            }
            catch
            {
                return null;
            }
        }

        private void tree_Area_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private AuioRequestDev[] getUserSlectDeviceArea()
        {
            List<AuioRequestDev> devices = new List<AuioRequestDev>();
            foreach (TreeNode oneDev in tree_Area.Nodes)
            {
                deviceInfo theDev = (deviceInfo)oneDev.Tag;
                //添加IP喇叭
                if (theDev.Type == "IPTRUMPET")
                {
                    if (oneDev.Checked)
                    {
                        AuioRequestDev one = new AuioRequestDev();
                        one.SN = theDev.SN;
                        one.Groups = new int[0];
                        devices.Add(one);
                        thisRequestDevCount++;
                        continue;
                    }
                }
                else
                {
                    AuioRequestDev netDev = null;
                    List<int> groups = new List<int>();
                    foreach (TreeNode oneCH in oneDev.Nodes[0].Nodes)
                    {
                        if (oneCH.Checked)
                        {
                            groups.Add((int)oneCH.Tag);
                        }
                    }
                    if (oneDev.Nodes.Count > 1)
                    {
                        foreach (TreeNode oneGroup in oneDev.Nodes[1].Nodes)
                        {
                            if (oneGroup.Checked)
                            {
                                groups.Add((int)oneGroup.Tag);
                            }
                        }
                    }
                    if (groups.Count > 0)
                    {
                        netDev = new AuioRequestDev();
                        netDev.SN = theDev.SN;
                        netDev.Groups = groups.ToArray();
                        devices.Add(netDev);
                        thisRequestDevCount++;
                    }
                }
            }
            return devices.ToArray();
        }

        private void sendAudioRequest(AuioRequestDev[] devArea)
        {
            AuioRequests request = new AuioRequests();
            request.CMD = "RecordSpeak";
            request.CodingStandard = "WAV";
            request.Serial = ++Serial;
            request.Devices = devArea;

            var searchCMD = JsonConvert.SerializeObject(request);

            groupSendData(searchCMD);//发送查询信息到设备组
        }

        // 接收确认消息线程方法
        private void ReceiveConfirmMessage()
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                string receiveMessage = "";
                try
                {
                    //关闭receiveUdpClient时此时会产生异常
                    byte[] receiveBytes = confirmReceiveUdpClient.Receive(ref remoteIpEndPoint);
                    if (remoteIpEndPoint.Address.ToString() == currentLocalIPV4)
                    {
                        continue;
                    }
                    receiveMessage = Encoding.UTF8.GetString(receiveBytes);
                    receiveMessageStr = receiveMessage;
                    //分析信息内容
                    // displayReceiveedMsg();
                    //显示信息
                }
                catch (Exception ex)
                {
                    continue;
                }
                try
                {
                    if (String.IsNullOrEmpty(receiveMessage))
                    { continue; }
                    if (receiveMessage.Contains("RecordSpeak"))
                    {
                        AuioRequestResult result = JsonConvert.DeserializeObject<AuioRequestResult>(receiveMessage);
                        ConfirmDevCount++;
                        if (result.Accept)
                        {
                        }
                        else
                        {
                            busDeviceCount++;
                            DelaySecMax = result.DelaySec > DelaySecMax ? result.DelaySec : DelaySecMax;
                            if (this.rText_RecordList.IsHandleCreated)
                                this.rText_RecordList.BeginInvoke(new Action(delegate
                                {
                                    //发送到UI 线程执行的代码
                                    string msg = String.Format("设备 {0} 正忙，请等候 {1}秒后再请求.", result.SN, (result.DelaySec));

                                    rText_RecordList.Text += msg + "\n";
                                }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        // 启动接收确认消息
        private void startReceiveConfirmData()
        {
            if (threadConfirmReceive != null)
            {
                confirmReceiveUdpClient.Close();
                threadConfirmReceive.Abort();
                threadConfirmReceive = null;
            }

            //判断IP是否有效
            currentLocalIPV4 = GetLocalIP();

            if (String.IsNullOrEmpty(currentLocalIPV4))
            {
                MessageBox.Show("当前电脑没有有效的网络设备！");
                return;
            }

            // 创建接收套接字
            IPAddress localIp = IPAddress.Parse(currentLocalIPV4);//选定当前系统的IP
            IPEndPoint localIpEndPoint = new IPEndPoint(localIp, confirmReceiveUdpPort);
            confirmReceiveUdpClient = new UdpClient(localIpEndPoint);

            // 启动接受线程
            threadConfirmReceive = new Thread(ReceiveConfirmMessage);
            threadConfirmReceive.IsBackground = true;
            threadConfirmReceive.Start();
        }

        //TCP通信协议

        private void regTcpClientEventMethod()
        {
            tcpClient.Completed += new Action<TcpClient, EnSocketAction>((c, enAction) =>
            {
                if (enAction == EnSocketAction.ConnectTimeOut)
                {
                    rText_RecordList.Invoke(new MethodInvoker(
                                delegate
                                {
                                    rText_RecordList.Text += String.Format("连接服务端超时!");
                                }
                                ));
                    return;
                }
                IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                switch (enAction)
                {
                    case EnSocketAction.Connect:
                        {
                            rText_RecordList.Invoke(new MethodInvoker(
        delegate
        {
            rText_RecordList.Text += String.Format("已经与{0}建立连接", key);
        }
        ));

                            break;
                        }
                    case EnSocketAction.SendMsg:
                        rText_RecordList.Invoke(new MethodInvoker(
                                delegate
                                {
                                    rText_RecordList.Text += String.Format("{0}：向{1}发送了一条消息", DateTime.Now, key);
                                }
                                ));
                        break;
                    case EnSocketAction.Close:
                        rText_RecordList.Invoke(new MethodInvoker(
                                delegate
                                {
                                    rText_RecordList.Text += String.Format("服务端连接关闭");
                                }
                                ));
                        break;
                    default:
                        break;
                }
            });


        }

        /// <summary>
        /// 检查设备是否在线，TCP方法
        /// </summary>
        private void checkDeviceOnlineTcp()
        {
            tcpClient = new ClientAsync();

            tcpClient.Completed += new Action<TcpClient, EnSocketAction>((c, enAction) =>
            {
                switch (enAction)
                {
                    case EnSocketAction.ConnectTimeOut:
                        {
                            rText_RecordList.Invoke(new MethodInvoker(
                                delegate
                                {
                                    rText_RecordList.Text += String.Format("连接服务端超时!");
                                }
                                ));
                            break;
                        }
                    case EnSocketAction.Connect:
                        {
                            var localIP = GetLocalIP();//获取电脑当前本地IP

                            SearchCmd cmd = new SearchCmd("SearchDev", localIP);
                            var searchCMD = JsonConvert.SerializeObject(cmd);
                            tcpClient.SendAsync(searchCMD);
                            break;
                        }
                }
            });

            tcpClient.Received += new Action<string, string>((key, msg) =>
            {
                if (msg.Contains("SoftwareVersion"))
                {
                    bool ex = false;
                    deviceInfo result = JsonConvert.DeserializeObject<deviceInfo>(msg);
                    result.IsOnLine = true;
                    result.IsMulticastTo = false;
                    foreach (var one in BindDeviceList)
                    {
                        if (one.SN == result.SN)
                        {
                            one.IsOnLine = true;
                            one.IsMulticastTo = false;
                            ex = true;
                            break;
                        }
                    }

                    if (this.dGrid_devList.IsHandleCreated && !ex && result.SN.Length > 0)
                        this.dGrid_devList.BeginInvoke(new Action(delegate
                        {
                            //发送到UI 线程执行的代码
                            result.IsOnLine = true;
                            result.IsMulticastTo = false;
                            BindDeviceList.Add(result);
                        }));
                }
            });
        }


        private void recordSpeakApplyFor(AuioRequestDev dev)
        {
            currentDev = dev;

            tcpClient = new ClientAsync();

            tcpClient.Completed -= TcpClient_Completed_SpeakApplyFor;
            tcpClient.Completed += TcpClient_Completed_SpeakApplyFor;
            tcpClient.Received -= TcpClient_Received_SpeakApplyFor;
            tcpClient.Received += TcpClient_Received_SpeakApplyFor;

        }

        private void TcpClient_Received_SpeakApplyFor(string key, string msg)
        {
            if (msg.Contains("Response") && msg.Contains("RecordSpeak"))
            {
                bool ex = false;
                AuioRequestResult result = JsonConvert.DeserializeObject<AuioRequestResult>(msg);
                if (!result.Accept)
                {
                    foreach (TreeNode oneNode in tree_Area.Nodes)
                    {
                        tree_Area.Invoke(new MethodInvoker(
                          delegate
                          {
                              oneNode.Checked = false;
                          }
                          ));
                        break;
                    }
                    MessageBox.Show("该设备忙，不接受请求，请稍后再试.", "提示信息");
                }
                else
                {
                    deviceInfo theDev = DeviceList.Find(x => x.SN == result.SN);
                    if (theDev.IsMulticastTo)
                    {
                        speakMulticastCommDev.Add(theDev);
                    }
                    else
                    {
                        speakUnicastCommDev.Add(theDev);
                    }
                }
            }
        }

        private void TcpClient_Completed_SpeakApplyFor(TcpClient c, EnSocketAction enAction)
        {
            switch (enAction)
            {
                case EnSocketAction.ConnectTimeOut:
                    {
                        rText_RecordList.Invoke(new MethodInvoker(
                            delegate
                            {
                                rText_RecordList.Text += String.Format("连接服务端超时!");
                            }
                            ));
                        break;
                    }
                case EnSocketAction.Connect:
                    {
                        var localIP = GetLocalIP();//获取电脑当前本地IP
                        int[] gtoup = new int[0];

                        AuioRequest cmd = new AuioRequest("RecordSpeak", Serial++, "WAV", new AuioRequestDev(currentDev.SN, gtoup));
                        var requestCMD = JsonConvert.SerializeObject(cmd);
                        tcpClient.SendAsync(requestCMD);
                        break;
                    }
            }
        }


        private void recordSpeakEndApplyFor(AuioRequestDev dev)
        {
            currentDev = dev;

            tcpClient = new ClientAsync();
            tcpClient.Completed -= TcpClient_Completed_SpeakEndApplyFor;
            tcpClient.Completed += TcpClient_Completed_SpeakEndApplyFor;
            tcpClient.Received -= TcpClient_Received_SpeakEndApplyFor;
            tcpClient.Received += TcpClient_Received_SpeakEndApplyFor;

            deviceInfo theDev = DeviceList.Find(x => x.SN == dev.SN);
            speakMulticastCommDev.Remove(theDev);
            speakUnicastCommDev.Remove(theDev);
        }


        private void TcpClient_Received_SpeakEndApplyFor(string key, string msg)
        {
            if (msg.Contains("Response") && msg.Contains("RecordSpeakEnd"))
            {
                bool ex = false;
                AuioRequestEndResult result = JsonConvert.DeserializeObject<AuioRequestEndResult>(msg);
                if (!result.SpeakBusy)
                {
                    foreach (TreeNode oneNode in tree_Area.Nodes)
                    {
                        tree_Area.Invoke(new MethodInvoker(
                          delegate
                          {
                              if (oneNode.Checked)
                                  oneNode.Checked = false;
                          }
                          ));
                        break;
                    }
                }
            }

            rText_RecordList.Invoke(new MethodInvoker(
                          delegate
                          {
                              rText_RecordList.Text += String.Format("收到{0}发来的信息：{1}", key, msg);
                          }
                          ));
        }

        private void TcpClient_Completed_SpeakEndApplyFor(TcpClient c, EnSocketAction enAction)
        {
            switch (enAction)
            {
                case EnSocketAction.ConnectTimeOut:
                    {
                        rText_RecordList.Invoke(new MethodInvoker(
                            delegate
                            {
                                rText_RecordList.Text += String.Format("连接服务端超时!");
                            }
                            ));
                        break;
                    }
                case EnSocketAction.Connect:
                    {
                        var localIP = GetLocalIP();//获取电脑当前本地IP
                        int[] gtoup = new int[0];

                        AuioRequest cmd = new AuioRequest("RecordSpeakEnd", Serial++, "WAV", new AuioRequestDev(currentDev.SN, gtoup));
                        var requestCMD = JsonConvert.SerializeObject(cmd);
                        tcpClient.SendAsync(requestCMD);
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取功放类设备用户选择的输出分区
        /// </summary>
        /// <param name="devNode"></param>
        /// <returns></returns>
        private AuioRequestDev getPowerSelectGroups(TreeNode devNode)
        {
            AuioRequestDev netDev = null;
            List<int> groups = new List<int>();
            foreach (TreeNode oneCH in devNode.Nodes[0].Nodes)
            {
                if (oneCH.Checked)
                {
                    groups.Add((int)oneCH.Tag);
                }
            }
            if (devNode.Nodes.Count > 1)
            {
                foreach (TreeNode oneGroup in devNode.Nodes[1].Nodes)
                {
                    if (oneGroup.Checked)
                    {
                        groups.Add((int)oneGroup.Tag);
                    }
                }
            }

            netDev = new AuioRequestDev();
            netDev.SN = ((deviceInfo)devNode.Tag).SN;
            netDev.Groups = groups.ToArray();
            return netDev;
        }

        private void tree_Area_Leave(object sender, EventArgs e)
        {
            // label5.Text = "tree_Area_Leave";
        }

        private void btn_AddDevice_Click(object sender, EventArgs e)
        {
            tcpClient = new ClientAsync();
            checkDeviceOnlineTcp();
            AddDeviceForm addform = new AddDeviceForm(tcpClient);
            addform.ShowDialog(this);

        }

        private void tree_Area_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Checked)
            {
                //取消
                //IP喇叭不带在分区的
                if (e.Node.Parent == null)
                {
                    deviceInfo theDevice = (deviceInfo)e.Node.Tag;
                    if (e.Node.Nodes.Count == 0)
                    {
                        recordSpeakEndApplyFor(new AuioRequestDev(theDevice.SN, new int[0]));
                        tcpClient.ConnectAsync(theDevice.IPV4, 65005);
                    }
                    else
                    {
                        AuioRequestDev devGroup = getPowerSelectGroups(e.Node);
                        recordSpeakEndApplyFor(devGroup);
                        tcpClient.ConnectAsync(theDevice.IPV4, 65005);
                    }
                }
            }
            else
            {
                //选中
                //IP喇叭不带在分区的
                if (e.Node.Parent == null)
                {
                    deviceInfo theDevice = (deviceInfo)e.Node.Tag;
                    if (e.Node.Nodes.Count == 0)
                    {
                        recordSpeakApplyFor(new AuioRequestDev(theDevice.SN, new int[0]));
                        tcpClient.ConnectAsync(theDevice.IPV4, 65005);
                    }
                    else
                    {
                        AuioRequestDev devGroup = getPowerSelectGroups(e.Node);
                        recordSpeakApplyFor(devGroup);
                        tcpClient.ConnectAsync(theDevice.IPV4, 65005);

                    }
                }
            }
        }

        /// <summary>
        /// 释放所有申请的设备，
        /// </summary>
        private void freeAllAppliedDev()
        {
            if (speakMulticastCommDev.Count > 0)
            {
                List<AuioRequestDev> deviceList = new List<AuioRequestDev>();
                foreach (var one in speakMulticastCommDev)
                {
                    AuioRequestDev oneDev = new AuioRequestDev(one.SN, new int[0]);
                    deviceList.Add(oneDev);
                }

                AuioRequests request = new AuioRequests();
                request.CMD = "RecordSpeakEnd";
                request.CodingStandard = "WAV";
                request.Serial = ++Serial;
                request.Devices = deviceList.ToArray();

                var requestCMD = JsonConvert.SerializeObject(request);

                groupSendData(requestCMD);//发送查询信息到设备组
                speakMulticastCommDev.Clear();
            }
            if (speakUnicastCommDev.Count > 0)
            {
                try
                {
                    foreach (var one in speakUnicastCommDev)
                    {
                        recordSpeakEndApplyFor(new AuioRequestDev(one.SN, new int[0]));
                        tcpClient.ConnectAsync(one.IPV4, 65005);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void btn_SendTCP_Click(object sender, EventArgs e)
        {

        }

        private void dGrid_devList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DeviceList.Count > 0)
                selectDevice = DeviceList[e.RowIndex];
        }

        private void btn_SaveDeviceList_Click(object sender, EventArgs e)
        {
            foreach (var one in DeviceList)
            {
                one.IsMulticastTo = false;
                one.IsOnLine = false;
            }
            var requestCMD = JsonConvert.SerializeObject(DeviceList);
            string filePath = Application.StartupPath + "/" + "device.dat";
            StreamWriter str = File.CreateText(filePath);
            str.Write(requestCMD);
            str.Close();
            MessageBox.Show("数据已存储!");
        }

        private void loadDeviceInfoFromData()
        {
            string filePath = Application.StartupPath + "/" + "device.dat";
            string data = File.ReadAllText(filePath);
            if (!String.IsNullOrEmpty(data))
            {
                try
                {
                    List<deviceInfo> result = JsonConvert.DeserializeObject<List<deviceInfo>>(data);
                    if (result.Count > 0)
                    {
                        foreach (var one in result)
                        {
                            BindDeviceList.Add(one);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载数据发生错误!");
                }
            }
        }

        private void btn_DelDevice_Click(object sender, EventArgs e)
        {
            if (selectDevice != null)
            {
                string message = "真的要删除 " + selectDevice.IPV4 + " 设备信息？";
                string caption = "请选择是否删除信息";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    // 操作
                    BindDeviceList.Remove(selectDevice);
                }
            }
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]

    public struct WaveInCaps
    {
        public short wMid;

        public short wPid;

        public int vDriverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

        public string szPname;

        public int dwFormats;

        public short wChannels;

        public short wReserved1;
    }

    public struct netCard
    {
        public string Name;
        public string IPV4;
        public string IPV6;
        public string SubnetMask;
        public string Gateway;
        public string DNS;
    }

    public class deviceInfo
    {
        public Int64 TerminalID { get; set; }
        public string AliasName { get; set; }
        public string Type { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public string SN { get; set; }

        public bool IsDHCP { get; set; }
        public string IPV4 { get; set; }
        public string SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DNS { get; set; }
        public String MAC { get; set; }
        public String IPV6 { get; set; }

        public bool IsOnLine { get; set; }
        public bool IsMulticastTo { get; set; }
        public String TaskStatus { get; set; }
        public bool SpeakBusy { get; set; }
        public String MonitorStatus { get; set; }
        public Int32 DefaultVolume { get; set; }
        public String ModeStr { get; set; }



        public deviceInfo() { }

    }

    public class SearchCmd
    {
        public string CMD;
        public string InquirersIPV4;

        public SearchCmd(string cmd, string ip)
        {
            CMD = cmd;
            InquirersIPV4 = ip;
        }
    }

    public class ChangeDevIPData
    {
        public string CMD = "ChangeNet";
        public string SN;
        public bool Switch;
        public string newIP;
        public string newmask;
        public string newgateway;
        public string newdnsweb;
        public string aliasName;
        public string userName;
        public string password;
    }

    public class SetIPResult
    {
        public bool Status;
        public int StatusCode;//
        public string SN;
        public string StatusMessage;// UserName or password error.
    }

    public class AuioRequestDev
    {
        public string SN;
        public int[] Groups;
        public AuioRequestDev() { }
        public AuioRequestDev(string sn, int[] groups)
        {
            SN = sn;
            Groups = groups;
        }
    }

    public class AuioRequests
    {
        public string CMD;
        public UInt32 Serial;
        public string CodingStandard;
        public AuioRequestDev[] Devices;

    }

    public class AuioRequest
    {
        public string CMD;
        public UInt32 Serial;
        public string CodingStandard;
        public AuioRequestDev Devices;
        public AuioRequest() { }
        public AuioRequest(string cmd, UInt32 serial, string codingStandard, AuioRequestDev devices)
        {
            CMD = cmd;
            Serial = serial;
            CodingStandard = codingStandard;
            Devices = devices;
        }
    }

    public class LoginDeviceResult
    {
        public bool Status;
        public int StatusCode;
        public string StatusMessage;
        public object DetailedInfo;
        public TokenData Data;
    }

    public class TokenData
    {
        public string Token;
    }

    public class GetCHGroupsResult
    {
        public bool Status;
        public int StatusCode;
        public string StatusMessage;
        public object DetailedInfo;
        public CHGroup[] Data;
    }

    public class CHGroup
    {
        public int GroupID;
        public string GroupName;
        public int[] ChannelList;
        public bool Status;
    }

    public class AuioRequestResult
    {
        public string Type;
        public string CMD;
        public bool Accept;
        public string SN;
        public UInt32 Serial;
        public UInt32 DelaySec;
    }

    public class AuioRequestEndResult
    {
        public string Type;
        public string CMD;
        public bool SpeakBusy;
        public UInt32 Serial;
        public AuioRequestDev Devices;
    }

    public class QueryPort
    {
        public string CMD;
        public string Channel;
        public UInt32 Serial;
        public QueryPort(UInt32 serial)
        {
            CMD = "QueryPort";
            Channel = "VoiceSpeak";
            Serial = serial;
        }
    }

    public class QueryPortResult
    {
        public string Type;
        public string CMD;
        public string SN;
        public UInt32 Serial;
        public UInt32 Port;
    }

    public class SetCommPort
    {
        public string CMD;
        public string Channel;
        public UInt32 Serial;
        public UInt32 Port;
        public AuioRequestDev[] Devices;
        public SetCommPort(UInt32 serial, UInt32 port, AuioRequestDev[] devices)
        {
            CMD = "SetCommPort";
            Channel = "VoiceSpeak";
            Serial = serial;
            Port = port;
            Devices = devices;
        }
    }

    public class SetCommPortResult
    {
        public string Type;
        public string CMD;
        public string Channel;
        public UInt32 Serial;
        public UInt32 Port;
    }
}
