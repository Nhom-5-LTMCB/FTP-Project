﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FTP_Client
{
    public partial class Form1 : Form
    {
        private string uri = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;
        public Form1()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';

            btnDownload.Enabled = false;
            btnUpload.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            uri = txtIp.Text;
            user = txtUsername.Text;
            pass = txtPassword.Text;

            if(string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(user)) {
                MessageBox.Show("Thông tin không được bỏ trống, vui lòng nhập đầy đủ", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpRequest.Credentials = new NetworkCredential(user, pass);


                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    // Đăng nhập thành công
                    MessageBox.Show("Đăng nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.None);

                    txtIp.ReadOnly = true;
                    txtPassword.ReadOnly = true;
                    txtUsername.ReadOnly = true;

                    btnConnect.Enabled = false;
                    btnDownload.Enabled = true;
                    btnUpload.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Kết nối thất bại, vui lòng thực hiện lại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }         
        }

        public void upload(string localFile, string filename)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(uri + filename);
                ftpRequest.Credentials = new NetworkCredential(user, pass);

                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;

                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpStream = ftpRequest.GetRequestStream();
                FileStream localFileStream = new FileStream(localFile, FileMode.Open);

                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);

                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            return;

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            string filePath = null;
            string fileName = null;
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                fileName = ofd.SafeFileName;
            }
            upload(filePath, fileName);
        }
    }
}
