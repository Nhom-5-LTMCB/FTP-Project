using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FTP_Client
{
    public partial class Form1 : Form
    {
        private FtpWebRequest request = null;
        public Form1()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';

            btnDownload.Enabled = false;
            btnUpload.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtIp.Text) || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtUsername.Text)) {
                MessageBox.Show("Thông tin không được bỏ trống, vui lòng nhập đầy đủ", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                request = (FtpWebRequest)WebRequest.Create(new Uri(txtIp.Text));
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(txtUsername.Text, txtPassword.Text);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    // Đăng nhập thành công
                    MessageBox.Show("Đăng nhập thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.None);
                    displayFileIntoListView(response);
                    txtIp.ReadOnly = true;
                    txtPassword.ReadOnly = true;
                    txtUsername.ReadOnly = true;

                    btnConnect.Enabled = false;
                    btnDownload.Enabled = true;
                    btnUpload.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Kết nối thất bại, vui lòng thực hiện lại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void displayFileIntoListView(FtpWebResponse response)
        {
            lstDisplayList.Items.Clear();
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    // Lấy tên file từ thông tin chi tiết
                    string fileName = line.Substring(line.LastIndexOf(' ') + 1);
                    // Thêm tên file vào ListView
                    lstDisplayList.Items.Add(fileName);
                }
            }
        }
         private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedFile = null;
                if (lstDisplayList.SelectedItems.Count > 0)
                        {
                            ListViewItem iTem = lstDisplayList.SelectedItems[0];
                            selectedFile = iTem.Text;
                        }

                     SaveFileDialog saveFileDialog = new SaveFileDialog();
                     saveFileDialog.FileName = selectedFile;
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string savePath = saveFileDialog.FileName;
                            request = (FtpWebRequest)WebRequest.Create(new Uri(txtIp.Text + "/" + selectedFile));
                            request.Method = WebRequestMethods.Ftp.DownloadFile;
                            request.Credentials = new NetworkCredential(txtUsername.Text, txtPassword.Text);

                            request.UseBinary = true;
                            request.UsePassive = true;
                            request.KeepAlive = true;
                            
                            using (FtpWebResponse downloadResponse = (FtpWebResponse)request.GetResponse())
                            {
                                using (Stream responseStream = downloadResponse.GetResponseStream())
                                {
                                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                                    {
                                        byte[] buffer = new byte[2048];
                                        int bytesRead;
                                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            fileStream.Write(buffer, 0, bytesRead);
                                        }
                                    }
                                }
                            }
                            MessageBox.Show("Download Complete", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
            }
            catch (WebException ex)
            {
             
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
