﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Threading;
using System.Collections;
using System.IO;
using System.Data;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics;


namespace ServerWindowsForms
{
    public partial class Form1 : Form
    {
        private Socket socke;
        private IPEndPoint Server, remoServer;
        private IPAddress ip, remoip;
        private Socket accSocket, sendsocke = null;
        private Socket Uptsendsocket = null;
        private Socket Picsendsocket = null;
        System.IO.FileStream fs;
        System.IO.BinaryReader strread;
        private DAO dao;
 
        static string remoIP = "";
        static int remoPORT = 0;
        Hashtable oht = new Hashtable();
        static int pv = 0;
        static string stopFlag = "0";
        bool exitflag1 = false;
        bool exitflag2 = false;
        private const int CP_NOCLOSE_BUTTON = 0x200;


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON ;
                return myCp;
            }
        }

        public Form1()
        {
            InitializeComponent();
            startPicServer();
            socke = new Socket(AddressFamily.InterNetwork,
                     SocketType.Stream, ProtocolType.Tcp);
            string path = System.Environment.CurrentDirectory + "/../../../conf.ini";
            INIFile inifile = new INIFile(path);
            string localIp = inifile.IniReadValue("Ip_Device", "serverip");
            ip = IPAddress.Parse(localIp);
            Server = new IPEndPoint(ip, 21888);
            sendsocke = new Socket(AddressFamily.InterNetwork,
             SocketType.Stream, ProtocolType.Tcp);
            Uptsendsocket = new Socket(AddressFamily.InterNetwork,
             SocketType.Stream, ProtocolType.Tcp);
            Picsendsocket = new Socket(AddressFamily.InterNetwork,
 SocketType.Stream, ProtocolType.Tcp);
            dao = new DAO();
        }

        public void startPicServer(){
            
            string path = System.Environment.CurrentDirectory + "\\FileRecevie.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            Process.Start(startInfo);
        }



        private void Connection()
        {
            string accStr = null;
            int accNum = 0;
            socke.Bind(Server);
            socke.Listen(100);
            while (true)
            {
                try
                {
                    Control.CheckForIllegalCrossThreadCalls = false;
                    accSocket = socke.Accept();
                    if (accSocket.Connected)
                    {
                        InforRichTextBox.AppendText(Dns.GetHostName() + "与客户端建立联系");
                        while (true)
                        {
                            accStr = null;
                            accNum = 0;
                            byte[] accBytes = new byte[1024];
                            NetworkStream ns = new NetworkStream(accSocket);
                            accNum = ns.Read(accBytes, 0, accBytes.Length);
                            if (accNum <= 0)
                                break;
                            accStr = System.Text.Encoding.UTF8.GetString(accBytes).Trim('\0');
                            if (accStr.Equals(stopFlag))
                            {
                                EndApplication();
                            }
                            String FB = dao.process(accStr).ToString();

                            EndPoint a = accSocket.RemoteEndPoint;
                            int start = a.ToString().IndexOf(':');
                            remoIP = a.ToString().Substring(0, start);
                            remoPORT = Convert.ToInt32(a.ToString().Substring(start + 1));
                           

                            InforRichTextBox.AppendText("\n" + "Android：" + accStr);
                            AutoRely(FB);
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //string startup = Application.ExecutablePath;
            //RegistryKey rKey = Registry.LocalMachine;
            //RegistryKey autoRun = rKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            //try
            //{
            //    autoRun.SetValue("CalSlnServer", startup);
            //    rKey.Close();
            //}
            //catch (Exception exp)
            //{
            //    MessageBox.Show(exp.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}   加入注册表
            try
            {
                Thread thread = new Thread(new ThreadStart(Connection));
                thread.Start();
                Thread Thread2 = new Thread(new ThreadStart(update));
                Thread2.Start();
            }
            catch (Exception se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void AutoRely(String rely)
        {
            IPEndPoint ServerEP = null;
            ip = IPAddress.Parse(remoIP);
            ServerEP = new IPEndPoint(ip, remoPORT + 1);
            if (!sendsocke.Connected)
            {
                sendsocke.Connect(ServerEP);
            }

            string sendMessageStr = rely;
            try
            {
                if (sendsocke.Connected)
                {
                    byte[] sendBytes = new byte[1024];
                    NetworkStream ns = new NetworkStream(sendsocke);
                    sendBytes = System.Text.UTF8Encoding.UTF8.GetBytes((sendMessageStr).ToCharArray());
                    ns.Write(sendBytes, 0, sendBytes.Length);
                    InforRichTextBox.AppendText("\nPC(port+1)：" + sendMessageStr);
                    pv = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }


        private void UpdaAutoRely(String rely)
        {

            IPEndPoint ServerEP = null;
            ip = IPAddress.Parse(remoIP);
            ServerEP = new IPEndPoint(ip, remoPORT + 2);
            if (!Uptsendsocket.Connected)
            {
                Uptsendsocket.Connect(ServerEP);
            }

            string sendMessageStr = rely;
            try
            {
                if (Uptsendsocket.Connected)
                {
                    byte[] sendBytes = new byte[1024];
                    NetworkStream ns = new NetworkStream(Uptsendsocket);
                    sendBytes = System.Text.UTF8Encoding.UTF8.GetBytes((sendMessageStr).ToCharArray());
                    ns.Write(sendBytes, 0, sendBytes.Length); 
                    InforRichTextBox.AppendText("\nPC(port+2)：" + sendMessageStr);
                    if (sendMessageStr.Length == 7)
                    {
                        string filepath = new DAO().getpicpath(sendMessageStr);
                        sendpic(filepath);
                    }
                }
            }
            catch (Exception)
            {
                pv = 0;
                EndApplication();
            }
            finally
            {
                
            }
        }

        private void update()
        {
           
            while (true)
            {
                while (pv == 1)
                {
                    Thread.Sleep(5000);
                    try
                    {
                        String SQL = "SELECT * FROM car_pale WHERE name='"+dao.getName()+"'";
                        String result = new DAO().SelectAll(SQL);
                        String change = "null";
                        if (!result.Equals("SQLERROR"))
                        {
                            Hashtable nht = new Hashtable();
                            for (int i = 0; i < result.Length / 10; i++)
                            {
                                nht.Add(result.Substring(i * 10, 7), result.Substring(i * 10 + 7, 3));
                            }
                            if (oht.Count == 0)
                            {
                                oht = nht;
                            }
                            else
                            {
                                foreach (DictionaryEntry de1 in nht)
                                {
                                    foreach (DictionaryEntry de2 in oht)
                                    {
                                        if ((de1.Key.Equals(de2.Key)) && (de1.Value.ToString().Equals("已入场")) && (de2.Value.ToString().Equals("已预约")))
                                        {
                                            change = de1.Key.ToString();
                                            break;
                                        }
                                    }
                                    if (change.Length == 7) { break; }
                                }
                            }
                            oht = nht;

                        }
                        UpdaAutoRely(change);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.StackTrace);
                    }
                }
            }
        }




        private void sendpic(string path)
        {
            IPEndPoint ServerEP = null;

            ip = IPAddress.Parse(remoIP);
         
            ServerEP = new IPEndPoint(ip, remoPORT + 3);


   

            if (!Picsendsocket.Connected)
            {
                Picsendsocket.Connect(ServerEP);
            }

            try
            {
                if (Picsendsocket.Connected)
                {

                    fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read);
                    

                    byte[] fssize = new byte[fs.Length];

                     strread = new System.IO.BinaryReader(fs);

                    strread.Read(fssize, 0, fssize.Length - 1);

                    Picsendsocket.Send(fssize);

                    InforRichTextBox.AppendText("\nPC(port+3_PIC) " + "Send"+fssize.Length+"byte");

                    fs.Close();
                    strread.Close();
                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
               
                    fs.Close();
                
                strread.Close();
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!exitflag1)
            {
                Application.Restart();
            }
            else
            {
                  //关闭图片接收服务器 
            }
        }


        private void EndApplication(){
            this.Close();
            System.Environment.Exit(0);
        }



        private void show_Click(object sender, EventArgs e)
        {

            this.Show();
            this.WindowState = FormWindowState.Normal; 
            this.Activate();
        }

        private void hind_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要重启终端服务程序吗？", "确认", MessageBoxButtons.OKCancel,
               MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                notifyIcon1.Visible = false;
                EndApplication();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要关闭服务器吗？", "确认", MessageBoxButtons.OKCancel,
              MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                exitflag1 = true;
                EndApplication();
            }
        }

        private void Form1_MinimumSizeChanged(object sender, EventArgs e)
        {
             this.Hide(); 
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }
      

    }
}
