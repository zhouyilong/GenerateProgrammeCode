using GenerateProgrammeCode.CSharp.WCF.Model;
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

namespace GenerateProgrammeCode.CSharp.WCF.View
{
    /// <summary>
    /// GenerateEntity.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateEntity : UserControl
    {
        #region===属性字段===
        // 属性信息列表
        List<PropertyInfo> properyInfoList = new List<PropertyInfo>();
        #endregion

        public GenerateEntity()
        {
            InitializeComponent();

            this.Loaded += GenerateEntity_Loaded;
        }

        #region===事件===
        void GenerateEntity_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= GenerateEntity_Loaded;

            //属性类型初始化
            List<string> propertyTypeList = new List<string>() { "string", "datetime", "number" };
            cmbPropertyType.ItemsSource = propertyTypeList;

            BindingPropertyInfos();
        }

        void btnPropertyInsert_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbProperty.Text) || cmbPropertyType.SelectedIndex < 0)
            {
                MessageBox.Show("属性名或者属性类型为空！");
            }
            else
            {
                PropertyInfo propertyInfo = new PropertyInfo();
                propertyInfo.PropertyName = tbProperty.Text;
                propertyInfo.PropertyNote = tbPropertyNote.Text;

                switch (Convert.ToString(cmbPropertyType.SelectedValue))
                {
                    case "string":
                        propertyInfo.PropertyType = "string";
                        propertyInfo.PropertyDBType = "VARCHAR2";
                        break;
                    case "datetime":
                        propertyInfo.PropertyType = "System.Nullable<System.DateTime>";
                        propertyInfo.PropertyDBType = "DATE";
                        break;
                    case "number":
                        propertyInfo.PropertyType = "System.Nullable<decimal>";
                        propertyInfo.PropertyDBType = "NUMBER";
                        break;
                }

                properyInfoList.Add(propertyInfo);

                BindingPropertyInfos();
            }


        }

        void btnGenerateEntity_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbDBName.Text) || string.IsNullOrEmpty(tbClass.Text))
            {
                MessageBox.Show("数据库名或者类名为空！");
            }
            else
            {
                StringBuilder sbClassCode = GenerateClassCode(tbDBName.Text, tbClass.Text, tbClassNote.Text);

                rtbCode.Document.Blocks.Clear();
                rtbCode.AppendText(sbClassCode.ToString());
            }
        }
        #endregion

        #region===类公共方法===
        void BindingPropertyInfos()
        {
            lbProperties.ItemsSource = null;
            lbProperties.ItemsSource = properyInfoList;
            lbProperties.DisplayMemberPath = "PropertyName ";
            lbProperties.SelectedValuePath = "PropertyName ";
        }

        StringBuilder GenerateClassCode(string dbName,string className,string classNote)
        {
            StringBuilder classCode=new StringBuilder();
            if (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(dbName))
            {
                if (!string.IsNullOrEmpty(classNote))
                {
                    classCode.Append("/// <summary>");
                    classCode.AppendLine();
                    classCode.AppendFormat("/// {0}", classNote);
                    classCode.AppendLine();
                    classCode.Append("/// </summary>");
                    classCode.AppendLine();
                }
                classCode.AppendFormat("[Table(Name = \"{0}.{1}\")]", dbName, className);
                classCode.AppendLine();
                classCode.AppendFormat("public partial class {0} : INotifyPropertyChanged", className);
                classCode.AppendLine();
                classCode.Append("{");

                foreach (var item in properyInfoList)
                {
                    classCode.AppendLine();
                    classCode.Append(GeneratePropertyCode(item));
                    classCode.AppendLine();
                }
                classCode.Append("#region 实现INotifyPropertyChanged接口");
                classCode.AppendLine();
                classCode.Append("public event PropertyChangedEventHandler PropertyChanged;");
                classCode.AppendLine();
                classCode.Append("public void OnPropertyChanged(string propertyName)");
                classCode.AppendLine();
                classCode.Append("{");
                classCode.AppendLine();
                classCode.Append("if (this.PropertyChanged != null)");
                classCode.AppendLine();
                classCode.Append("{");
                classCode.AppendLine();
                classCode.Append("this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));");
                classCode.AppendLine();
                classCode.Append("}");
                classCode.AppendLine();
                classCode.Append("}");
                classCode.AppendLine();
                classCode.Append("#endregion");
                classCode.AppendLine();
                classCode.Append("}");
            }

            return classCode;
        }

        StringBuilder GeneratePropertyCode(PropertyInfo properyInfo)
        {
            StringBuilder template = new StringBuilder();
            if (properyInfo != null)
            {
                string name = properyInfo.PropertyName;
                string _name = "_" + (name.ToLower().Replace("_", ""));
                string note = properyInfo.PropertyNote;
                string type = properyInfo.PropertyType;
                string dbType = properyInfo.PropertyDBType;

                template.AppendFormat("private {0} {1};", type, _name);
                template.AppendLine();
                if (!string.IsNullOrEmpty(note))
                {
                    template.Append("/// <summary>");
                    template.AppendLine();
                    template.AppendFormat("///{0}", note);
                    template.AppendLine();
                    template.Append("/// </summary>");
                    template.AppendLine();
                }
                template.AppendFormat("[Column(Storage = \"{0}\", Name = \"{1}\", DbType = \"{2}\", AutoSync = AutoSync.Never)]"
                    , _name, name, dbType);
                template.AppendLine();
                template.Append("[DebuggerNonUserCode()]");
                template.AppendLine();
                template.AppendFormat("public {0} {1}", type, name);
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
            }
            return template;
        }
        #endregion


    }
}
