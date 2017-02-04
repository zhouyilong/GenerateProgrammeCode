using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using GenerateProgrammeCode.Storage;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace GenerateProgrammeCode
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 属性字段
        //更新包地址
        private string url = "";
        //新版本号
        string version = "";

        JObject serverJObject;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        #region 事件
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;

            CheckUpdate();

            this.Title = "生成代码" + version;

            cmbLanguageType.ItemsSource = new List<ViewType> { 
                 new ViewType(){ Desc="Silverlight",Value= "Silverlight"},
                  new ViewType(){ Desc="WCF",Value= "WCF"},
                new ViewType(){ Desc="Oracle",Value= "Oracle"}};
            cmbLanguageType.SelectedValuePath = "Value";
            cmbLanguageType.DisplayMemberPath = "Desc";



            cmbViewType.ItemsSource = new List<ViewType> { 
                new ViewType(){ Desc="生成Linq类实体",Value= "GenerateEntity"},
                new ViewType(){ Desc="生成TableOp",Value= "GenerateTabOp"} ,
                new ViewType(){ Desc="生成域配置执行SQL",Value= "GenerateSXGISMIS"}};

            cmbViewType.SelectedValuePath = "Value";
            cmbViewType.DisplayMemberPath = "Desc";
        }

        private void cmbViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ViewType viewType = (sender as ComboBox).SelectedItem as ViewType;
                string generateViewName = viewType.Value;

                Type generateViewType = this.GetType().Assembly.GetTypes().Single(p => p.Name == generateViewName);
                if (generateViewType != null)
                {
                    object generateView = Activator.CreateInstance(generateViewType);
                    if (generateView != null && generateView is UserControl)
                    {
                        gridContent.Children.Clear();
                        gridContent.Children.Add(generateView as UserControl);
                    }
                }
            }
            catch (Exception) { }
        }

        private void cmbLanguageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewType viewType = (sender as ComboBox).SelectedItem as ViewType;
            List<ViewType> viewTypeList = new List<ViewType> { };


            switch (viewType.Value)
            {
                case "WCF":
                    viewTypeList.Add(new ViewType() { Desc = "生成TableOp", Value = "GenerateTabOp" });
                    viewTypeList.Add(new ViewType() { Desc = "生成Linq类实体", Value = "GenerateEntity" });
                    break;
                case "Oracle":
                    viewTypeList.Add(new ViewType() { Desc = "生成域配置执行SQL", Value = "GenerateSXGISMIS" });
                    viewTypeList.Add(new ViewType() { Desc = "生成自动编码执行SQL", Value = "GenerateAutoCode" });
                    break;
            }

            cmbViewType.ItemsSource = viewTypeList;
            cmbViewType.SelectedValuePath = "Value";
            cmbViewType.DisplayMemberPath = "Desc";

        }
        #endregion

        #region 类公共方法
        //是否需要更新
        bool isNeedUpdate = false;
        //是否必须更新
        bool isMustUpdate = false;
        //是否更新自动更新程序
        bool isUpdateAutoUpdate = false;

        //检查是否有更新
        void CheckUpdate()
        {
            url = ConfigurationManager.AppSettings["Url"].Trim();
            version = ConfigurationManager.AppSettings["Version"].Trim();
            if (string.IsNullOrEmpty(version))
            {
                isNeedUpdate = true;
            }
            else if (!string.IsNullOrEmpty(url))
            {
                WebClient webClient = new WebClient();

                Uri address = new Uri(url);

                //下载软件版本号
                webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
                MyBusyIndicator.IsBusy = true;

                webClient.DownloadStringAsync(address);

            }
        }

        //从服务器中下载版本号完成后执行
        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            /*
             * 版本号类似 v1.4.3
             * 当第1位发生改变时，需要强制更新程序
             * 其他位发生数字变大时，将会进行更新提示，可不进行更新
             * 当第2位发生改变时，需要更新自动更新程序
             * */
            if (e.Error == null)
            {
                try
                {
                    string serverJson = e.Result;

                    serverJObject = JToken.Parse(serverJson) as JObject;

                    string serverVersion = serverJObject["version"].ToString();
                    if (!string.IsNullOrEmpty(serverVersion))
                    {
                        string[] serverVersions = serverVersion.Replace("v", "").Split('.');
                        string[] versions = version.Replace("v", "").Split('.');
                        for (int i = 0; i < serverVersions.Length; i++)
                        {
                            if (versions.Length >= i + 1)
                            {
                                int serverNum = 0;
                                int num = 0;
                                if (Int32.TryParse(serverVersions[i], out serverNum) && Int32.TryParse(versions[i], out num))
                                {
                                    if (i == 0)
                                    {
                                        if (serverNum > num)
                                        {
                                            //强制更新时，需要更新自动更新程序
                                            isUpdateAutoUpdate = true;
                                            isMustUpdate = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (serverNum > num)
                                        {
                                            //当第2位发生改变时，需要更新自动更新程序
                                            if (i == 1)
                                            {
                                                isUpdateAutoUpdate = true;
                                            }

                                            isNeedUpdate = true;
                                            break;
                                        }
                                    }

                                }
                            }
                            else
                            {
                                int serverNum = 0;
                                int num = 0;
                                if (Int32.TryParse(serverVersions[versions.Length - 1], out serverNum) && Int32.TryParse(versions[versions.Length - 1], out num))
                                {

                                    if (serverNum == num)
                                    {
                                        isNeedUpdate = true;
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                }
                            }
                        }
                    }

                    //更新主体程序前，更新自动更新程序
                    if (isUpdateAutoUpdate)
                    {
                        if (ConfigurationManager.AppSettings.AllKeys.Contains("AutoUpdateUrl"))
                        {
                            string url = ConfigurationManager.AppSettings["AutoUpdateUrl"].Trim();
                            string filename = url.Substring(url.LastIndexOf("/") + 1);
                            //下载文件存放在临时文件夹中
                            string filepath = Environment.GetEnvironmentVariable("TEMP") + @"/" + filename;

                            if (!string.IsNullOrEmpty(filename))
                            {
                                try
                                {
                                    KillExeProcess();
                                    DownloadFile(url, filepath);
                                }
                                catch (Exception ex)
                                {
                                    writeLog(ex.Message);
                                }
                            }
                            else
                            {
                                writeLog("AutoUpdate更新失败：下载的文件名为空！");
                                return;
                            }
                        }
                        else
                        {
                            writeLog("AutoUpdate更新失败：未在App.config中指定需要下载的文件位置！");
                            return;
                        }
                    }

                    if (isMustUpdate)
                    {
                        if (MessageBox.Show("存在必要更新，点击确定更新!\r\n更新日志：" + "\r\n" + serverJObject["log"].ToString(), "提示", MessageBoxButton.OK) == MessageBoxResult.OK)
                        {
                            Process openupdatedexe = new Process();
                            openupdatedexe.StartInfo.FileName = "AutoUpdate.exe";
                            openupdatedexe.StartInfo.Arguments = "GenerateProgrammeCode.exe " + serverJObject["totalByes"].ToString();
                            openupdatedexe.Start();

                            this.Close();
                        }
                    }
                    else if (isNeedUpdate)
                    {
                        if (MessageBox.Show("存在更新，点击确定更新,点击取消不更新!\r\n更新日志：" + "\r\n" + serverJObject["log"].ToString(), "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            Process openupdatedexe = new Process();
                            openupdatedexe.StartInfo.FileName = "AutoUpdate.exe";
                            openupdatedexe.StartInfo.Arguments = "GenerateProgrammeCode.exe " + serverJObject["totalByes"].ToString();
                            openupdatedexe.Start();

                            this.Close();
                        }
                    }

                }
                catch (Exception)
                {
                }
            }

            MyBusyIndicator.IsBusy = false;
        }

        /// <summary>
        /// 杀掉正在运行的需要更新的程序
        /// </summary>
        void KillExeProcess()
        {
            try
            {
                foreach (Process p in Process.GetProcessesByName("AutoUpdate"))
                {
                    p.Kill();
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
        void DownloadFile(string fileUrl, string filepath)
        {
            WebClient client = new WebClient();
            try
            {
                Uri address = new Uri(fileUrl);

                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                client.DownloadFileAsync(address, filepath);
                client.DownloadFileCompleted += (sender, e) =>
                {
                    try
                    {
                        UnZipFile(filepath);
                        if (File.Exists(filepath))
                        {
                            File.Delete(filepath);
                        }
                        writeLog("更新AutoUpdate成功！");
                    }
                    catch (Exception ex)
                    {
                        writeLog(ex.Message);
                    }

                };
            }
            catch (Exception ex)
            {
                throw new Exception("下载AutoUpdate更新文件出错：" + ex.Message);
            }

        }

        /// <summary>
        /// 解压压缩包，格式必须是*.zip,否则不能解压
        /// </summary>
        /// <returns></returns>
        void UnZipFile(string filepath)
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

        private void writeLog(string str)
        {

            string strLog = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + str + "/r/n";

            StreamWriter errorlog = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, @"log.txt"), true);
            errorlog.Write(strLog);
            errorlog.Flush();
            errorlog.Close();
        }
        #endregion

        #region 内部类
        class ViewType
        {
            /// <summary>
            /// 显示值
            /// </summary>
            public string Desc { get; set; }

            /// <summary>
            /// 实际值
            /// </summary>
            public string Value { get; set; }
        }
        #endregion

    }
}
