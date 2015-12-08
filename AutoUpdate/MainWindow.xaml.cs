﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;

namespace AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        #region===字段属性===
        //更新包地址
        private string url = "";
        //文件名字
        private string filename = "";
        //下载文件存放全路径
        private string filepath = "";

        //更新后打开的程序名
        string startexe = "";
        //下载包大小
        int totalByes = 0;
        #endregion
        public MainWindow()
        {
            InitializeComponent();

            if (Application.Current.Properties["startexe"] != null)
            {
                startexe = Application.Current.Properties["startexe"].ToString().Trim();
            }

            if (Application.Current.Properties["totalByes"] != null)
            {
                Int32.TryParse(Application.Current.Properties["totalByes"].ToString().Trim(), out totalByes);

            }

        }

        private void Window_Activated(object sender, EventArgs e)
        {
            url = ConfigurationManager.AppSettings["Url"].Trim();

            if (url != "")
            {
                filename = url.Substring(url.LastIndexOf("/") + 1);
                //下载文件存放在临时文件夹中
                filepath = Environment.GetEnvironmentVariable("TEMP") + @"/" + filename;

                if (filename != "")
                {
                    try
                    {
                        KillExeProcess();
                        DownloadFile();
                        //UnZipFile();
                        //UpdateVersionInfo();
                        //OpenUpdatedExe();

                        //writeLog("更新成功！");
                    }
                    catch (Exception ex)
                    {
                        writeLog(ex.Message);
                    }

                }
                else
                {
                    writeLog("更新失败：下载的文件名为空！");
                    return;
                }
            }

            else
            {
                writeLog("更新失败：未在App.config中指定需要下载的文件位置！");
            }

            //if (File.Exists(filepath))
            //{
            //    File.Delete(filepath);
            //}

            //this.Close();
        }

        /// <summary>
        /// 杀掉正在运行的需要更新的程序
        /// </summary>
        private void KillExeProcess()
        {
            //后缀起始位置
            int startpos = -1;

            try
            {
                if (startexe != "")
                {
                    if (startexe.EndsWith(".EXE"))
                    {
                        startpos = startexe.IndexOf(".EXE");
                    }
                    else if (startexe.EndsWith(".exe"))
                    {
                        startpos = startexe.IndexOf(".exe");
                    }
                    foreach (Process p in Process.GetProcessesByName(startexe.Remove(startpos)))
                    {
                        p.Kill();

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("清杀原程序进程出错：" + ex.Message);
            }
        }

        /// <summary>
        /// 下载更新包
        /// </summary>
        public void DownloadFile()
        {
            WebClient client = new WebClient();
            try
            {
                Uri address = new Uri(url);

                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                client.DownloadFileAsync(address, filepath);
                client.DownloadFileCompleted += (sender, e) =>
                {
                    try
                    {
                        UnZipFile();
                        if (File.Exists(filepath))
                        {
                            File.Delete(filepath);
                        }
                        if (MessageBox.Show("下载完成，是否打开更新后的程序!", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            OpenUpdatedExe();

                            this.Close();
                        }
                        else
                        {
                            this.Close();
                        }
                        writeLog("更新成功！");
                    }
                    catch (Exception ex)
                    {
                        writeLog(ex.Message);
                    }          

                };
                client.DownloadProgressChanged += (sender, e) =>
                {
                    if (totalByes>0)
                    {
                        double percent = ((double)e.BytesReceived / totalByes) * 100;
                        progressBar.Value = percent;
                    }
                };


            }
            catch (Exception ex)
            {
                throw new Exception("下载更新文件出错：" + ex.Message);
            }

        }


        private void UpdateVersionInfo()
        {
            //try
            //{
            //    Configuration cfa = ConfigurationManager.OpenExeConfiguration(startexe);
            //    cfa.AppSettings.Settings["Version"].Value = version;
            //    cfa.Save();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("更新版本信息出错：" + ex.Message);
            //}

        }

        /// <summary>
        /// 打开更新后的程序
        /// </summary>
        private void OpenUpdatedExe()
        {
            try
            {
                if (ConfigurationManager.AppSettings["StartAfterUpdate"] == "true" && startexe != "")
                {
                    Process openupdatedexe = new Process();
                    openupdatedexe.StartInfo.FileName = startexe;
                    openupdatedexe.Start();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打开更新后程序出错：" + ex.Message);
            }
        }

        #region 不好用
        /// <summary>
        /// 解压压缩包，格式必须是*.zip,否则不能解压
        /// 需要添加System32下的Shell32.dll
        /// 不好用总是弹出来对话框
        /// </summary>
        //private void UnZip()
        //{
        //    try
        //    {
        //        ShellClass sc = new ShellClass();
        //        Folder SrcFolder = sc.NameSpace(filepath);
        //        Folder DestFolder = sc.NameSpace(System.Environment.CurrentDirectory);
        //        FolderItems items = SrcFolder.Items();
        //        DestFolder.CopyHere(items, 16);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("解压缩更新包出错：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        #endregion

        #region 解压zip
        /// <summary>
        /// 解压压缩包，格式必须是*.zip,否则不能解压
        /// </summary>
        /// <returns></returns>
        private void UnZipFile()
        {
            try
            {
                using (ZipInputStream zis = new ZipInputStream(File.OpenRead(filepath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = zis.GetNextEntry()) != null)
                    {
                        string directoryName = System.IO.Path.GetDirectoryName(theEntry.Name);
                        string zipfilename = System.IO.Path.GetFileName(theEntry.Name);

                        if (directoryName.Length > 0 && !Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        if (zipfilename != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(theEntry.Name))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zis.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解压缩更新包出错：" + ex.Message);
            }

        }
        #endregion

        private void writeLog(string str)
        {

            string strLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + str + "/r/n";

            StreamWriter errorlog = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, @"log.txt"), true);
            errorlog.Write(strLog);
            errorlog.Flush();
            errorlog.Close();
        }
    }
}
