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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenerateProgrammeCode
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //string
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = tb.Text;
            if (!string.IsNullOrEmpty(text))
            {
                string[] properties = text.Split(',');

                StringBuilder sb = new StringBuilder();

                foreach (var item in properties)
                {
                    if (item.Split(':').Length == 2)
                    {
                        sb.Append(GetStringPropertyCode(item.Split(':')[0], item.Split(':')[1]));
                        sb.AppendLine();
                    }
                }
                rtb.Document.Blocks.Clear();
                rtb.AppendText(sb.ToString());

            }
        }



        //datetime
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string text = tb.Text;
            if (!string.IsNullOrEmpty(text))
            {
                string[] properties = text.Split(',');

                StringBuilder sb = new StringBuilder();

                foreach (var item in properties)
                {
                    if (item.Split(':').Length == 2)
                    {
                        sb.Append(GetDatetimePropertyCode(item.Split(':')[0], item.Split(':')[1]));
                        sb.AppendLine();
                    }
                }
                rtb.Document.Blocks.Clear();
                rtb.AppendText(sb.ToString());

            }
        }

        //number
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string text = tb.Text;
            if (!string.IsNullOrEmpty(text))
            {
                string[] properties = text.Split(',');

                StringBuilder sb = new StringBuilder();

                foreach (var item in properties)
                {
                    if (item.Split(':').Length == 2)
                    {
                        sb.Append(GetNumberPropertyCode(item.Split(':')[0], item.Split(':')[1]));
                        sb.AppendLine();
                    }
                }
                rtb.Document.Blocks.Clear();
                rtb.AppendText(sb.ToString());

            }
        }


        StringBuilder GetStringPropertyCode(string name, string note)
        {
            string _name = "_" + (name.ToLower().Replace("_", ""));
            StringBuilder template = new StringBuilder();
            template.AppendFormat("private string {0};", _name);
            template.AppendLine();
            template.Append("/// <summary>");
            template.AppendLine();
            template.AppendFormat("///{0}", note);
            template.AppendLine();
            template.Append("/// </summary>");
            template.AppendLine();
            template.AppendFormat("[Column(Storage = \"{0}\", Name = \"{1}\", DbType = \"VARCHAR2\", AutoSync = AutoSync.Never)]"
                , _name, name);
            template.AppendLine();
            template.Append("[DebuggerNonUserCode()]");
            template.AppendLine();
            template.AppendFormat("public string {0}", name);
            template.AppendLine();
            template.Append("{get{");
            template.AppendLine();
            template.AppendFormat("return this.{0};", _name);
            template.AppendLine();
            template.Append("}set{");
            template.AppendLine();
            template.AppendFormat(" if (({0} == value) == false)",
                _name);
            template.AppendLine();
            template.Append("{");
            template.AppendLine();
            template.AppendFormat("this.{0} = value;this.OnPropertyChanged(\"{1}\");", _name, name);
            template.AppendLine();
            template.Append("}}}");

            return template;
        }

        StringBuilder GetDatetimePropertyCode(string name, string note)
        {
            string _name = "_" + (name.ToLower().Replace("_", ""));
            StringBuilder template = new StringBuilder();
            template.AppendFormat("private System.Nullable<System.DateTime> {0};", _name);
            template.AppendLine();
            template.Append("/// <summary>");
            template.AppendLine();
            template.AppendFormat("///{0}", note);
            template.AppendLine();
            template.Append("/// </summary>");
            template.AppendLine();
            template.AppendFormat("[Column(Storage = \"{0}\", Name = \"{1}\", DbType = \"DATE\", AutoSync = AutoSync.Never)]"
                , _name, name);
            template.AppendLine();
            template.Append("[DebuggerNonUserCode()]");
            template.AppendLine();
            template.AppendFormat("public System.Nullable<System.DateTime> {0}", name);
            template.AppendLine();
            template.Append("{get{");
            template.AppendLine();
            template.AppendFormat("return this.{0};", _name);
            template.AppendLine();
            template.Append("}set{");
            template.AppendLine();
            template.AppendFormat(" if (({0} == value) == false)",
                _name);
            template.AppendLine();
            template.Append("{");
            template.AppendLine();
            template.AppendFormat("this.{0} = value;this.OnPropertyChanged(\"{1}\");", _name, name);
            template.AppendLine();
            template.Append("}}}");

            return template;
        }

        StringBuilder GetNumberPropertyCode(string name, string note)
        {
            string _name = "_" + (name.ToLower().Replace("_", ""));
            StringBuilder template = new StringBuilder();
            template.AppendFormat("private System.Nullable<decimal> {0};", _name);
            template.AppendLine();
            template.Append("/// <summary>");
            template.AppendLine();
            template.AppendFormat("///{0}", note);
            template.AppendLine();
            template.Append("/// </summary>");
            template.AppendLine();
            template.AppendFormat("[Column(Storage = \"{0}\", Name = \"{1}\", DbType = \"NUMBER\", AutoSync = AutoSync.Never)]"
                , _name, name);
            template.AppendLine();
            template.Append("[DebuggerNonUserCode()]");
            template.AppendLine();
            template.AppendFormat("public System.Nullable<decimal> {0}", name);
            template.AppendLine();
            template.Append("{get{");
            template.AppendLine();
            template.AppendFormat("return this.{0};", _name);
            template.AppendLine();
            template.Append("}set{");
            template.AppendLine();
            template.AppendFormat(" if (({0} == value) == false)",
                _name);
            template.AppendLine();
            template.Append("{");
            template.AppendLine();
            template.AppendFormat("this.{0} = value;this.OnPropertyChanged(\"{1}\");", _name, name);
            template.AppendLine();
            template.Append("}}}");

            return template;
        }
    }
}
