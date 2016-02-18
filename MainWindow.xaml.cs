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
using Newtonsoft.Json.Linq;


namespace GenerateProgrammeCode
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region===属性字段===
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

        #region===事件===
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;

            CheckUpdate();

            this.Title = "生成代码" + version;


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

        #endregion

        #region===类公共方法===
        //检查是否有更新
        bool isNeedUpdate = false;
        bool isMustUpdate = false;
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
                                            isMustUpdate = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (serverNum > num)
                                        {
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

                    if (isMustUpdate)
                    {
                        if (MessageBox.Show("存在必要更新，点击确定更新!", "提示", MessageBoxButton.OK) == MessageBoxResult.OK)
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
        #endregion

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
    }
}
