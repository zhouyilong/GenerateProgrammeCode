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
    /// GenerateTabOp.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateTabOp : UserControl
    {
        public GenerateTabOp()
        {
            InitializeComponent();
            this.Loaded += GenerateTabOp_Loaded;
        }

        #region===事件===
        void GenerateTabOp_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= GenerateTabOp_Loaded;
        }

        void btnGenerateTabOp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbClassName.Text))
            {
                MessageBox.Show("类名不能为空");
            }
            else
            {
                StringBuilder sbTableOpCode = GenerateTabOpClass(tbClassName.Text, tbClassDesc.Text, tbAuthor.Text, tbTime.Text);

                rtbCode.Document.Blocks.Clear();
                rtbCode.AppendText(sbTableOpCode.ToString());
            }
        }
        #endregion

        #region===类公共方法===
        StringBuilder GenerateTabOpClass(string className, string classDesc, string author, string time)
        {
            StringBuilder tableOpSB = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
            {
                tableOpSB.AppendFormat(" public class {0}OP", className);
                tableOpSB.AppendLine();
                tableOpSB.Append("{");
                tableOpSB.AppendLine();
                tableOpSB.Append(GenerateGetTableMethod(className, classDesc, author, time));
                tableOpSB.AppendLine();
                tableOpSB.Append(GenereateInsertMethod(className, classDesc, author, time));
                tableOpSB.AppendLine();
                tableOpSB.Append(GenereateDeleteMethod(className, classDesc, author, time));
                tableOpSB.AppendLine();
                tableOpSB.Append(GenereateUpdateMethod(className, classDesc, author, time));
                tableOpSB.AppendLine();
                tableOpSB.Append("}");
                tableOpSB.AppendLine();
            }

            return tableOpSB;
        }

        StringBuilder GenerateGetTableMethod(string className, string classDesc, string author, string time)
        {
            StringBuilder getTableSB = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
            {
                getTableSB.Append("/// <summary>");
                getTableSB.AppendLine();
                if (!string.IsNullOrEmpty(classDesc))
                {
                    getTableSB.AppendFormat("/// <para>获得{0}基本信息列表</para>", classDesc);
                    getTableSB.AppendLine();
                }
                if (!string.IsNullOrEmpty(author) && !string.IsNullOrEmpty(time))
                {
                    getTableSB.AppendFormat("/// <para>by {0} {1}</para>", author, time);
                    getTableSB.AppendLine();
                }
                else if (!string.IsNullOrEmpty(author))
                {
                    getTableSB.AppendFormat("/// <para>by {0}</para>", author);
                    getTableSB.AppendLine();
                }
                getTableSB.Append(" /// </summary>");
                getTableSB.AppendLine();
                getTableSB.Append("/// <param name=\"v\">数据库连接对象</param>");
                getTableSB.AppendLine();
                getTableSB.Append("/// <param name=\"condition \">条件对象</param>");
                getTableSB.AppendLine();
                if (!string.IsNullOrEmpty(classDesc))
                {
                    getTableSB.AppendFormat("/// <returns>{0}基本信息列表</returns>", classDesc);
                    getTableSB.AppendLine();
                }
                else
                {
                    getTableSB.Append("/// <returns></returns>");
                    getTableSB.AppendLine();
                }
                getTableSB.AppendFormat("public IQueryable<{0}> GetIQ{0}(sxgismis2 v, {0} condition)", className);
                getTableSB.AppendLine();
                getTableSB.Append("{");
                getTableSB.AppendLine();
                getTableSB.Append("if (condition != null)");
                getTableSB.AppendLine();
                getTableSB.Append("{");
                getTableSB.AppendLine();
                getTableSB.AppendFormat("IQueryable<{0}> result = v.{0};", className);
                getTableSB.AppendLine();
                getTableSB.Append("result=CommonOP.GetIQData(result, condition);");
                getTableSB.AppendLine();
                getTableSB.Append("return result;");
                getTableSB.AppendLine();
                getTableSB.Append("}");
                getTableSB.AppendLine();
                getTableSB.Append("else");
                getTableSB.AppendLine();
                getTableSB.Append("{");
                getTableSB.AppendLine();
                getTableSB.Append("return null;");
                getTableSB.AppendLine();
                getTableSB.Append("}");
                getTableSB.AppendLine();
                getTableSB.Append("}");
            }
            return getTableSB;
        }

        StringBuilder GenereateInsertMethod(string className, string classDesc, string author, string time)
        {
            StringBuilder insertSB = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
            {
                string instanceName = GetInstanceName(className);
                string pkName = className.Split('_')[0] + "_ID";
                insertSB.Append("/// <summary>");
                insertSB.AppendLine();
                if (!string.IsNullOrEmpty(classDesc))
                {
                    insertSB.AppendFormat("/// <para>新增{0}基本信息</para>", classDesc);
                    insertSB.AppendLine();
                }
                if (!string.IsNullOrEmpty(author) && !string.IsNullOrEmpty(time))
                {
                    insertSB.AppendFormat("/// <para>by {0} {1}</para>", author, time);
                    insertSB.AppendLine();
                }
                else if (!string.IsNullOrEmpty(author))
                {
                    insertSB.AppendFormat("/// <para>by {0}</para>", author);
                    insertSB.AppendLine();
                }
                insertSB.Append(" /// </summary>");
                insertSB.AppendLine();
                insertSB.Append(" /// <param name=\"v\">数据库连接对象</param>");
                insertSB.AppendLine();
                insertSB.AppendFormat(" /// <param name=\"{0}\">{1}基本信息新增对象</param>", instanceName, classDesc);
                insertSB.AppendLine();
                insertSB.AppendFormat(" public void InsertData(sxgismis2 v, {0} {1})",className,instanceName);
                insertSB.AppendLine();
                insertSB.Append(" {");
                insertSB.AppendLine();
                insertSB.AppendFormat("  if ({0} != null && !string.IsNullOrEmpty({0}.{1}))", instanceName, pkName);
                insertSB.AppendLine();
                insertSB.Append(" {");
                insertSB.AppendLine();
                insertSB.AppendFormat("  v.{0}.InsertOnSubmit({1});",className,instanceName);
                insertSB.AppendLine();
                insertSB.Append(" }");
                insertSB.AppendLine();
                insertSB.Append(" }");
            }
            return insertSB;
        }

        StringBuilder GenereateDeleteMethod(string className, string classDesc, string author, string time)
        {
            StringBuilder deleteSB = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
            {
                string instanceName = GetInstanceName(className);
                string pkName = className.Split('_')[0] + "_ID";
                deleteSB.Append("/// <summary>");
                deleteSB.AppendLine();
                if (!string.IsNullOrEmpty(classDesc))
                {
                    deleteSB.AppendFormat("/// <para>删除{0}基本信息</para>", classDesc);
                    deleteSB.AppendLine();
                }
                if (!string.IsNullOrEmpty(author) && !string.IsNullOrEmpty(time))
                {
                    deleteSB.AppendFormat("/// <para>by {0} {1}</para>", author, time);
                    deleteSB.AppendLine();
                }
                else if (!string.IsNullOrEmpty(author))
                {
                    deleteSB.AppendFormat("/// <para>by {0}</para>", author);
                    deleteSB.AppendLine();
                }
                deleteSB.Append(" /// </summary>");
                deleteSB.AppendLine();
                deleteSB.Append(" /// <param name=\"v\">数据库连接对象</param>");
                deleteSB.AppendLine();
                deleteSB.AppendFormat(" /// <param name=\"{0}\">{1}基本信息删除对象</param>", instanceName, classDesc);
                deleteSB.AppendLine();
                deleteSB.AppendFormat(" public void DeleteData(sxgismis2 v, {0} {1})", className, instanceName);
                deleteSB.AppendLine();
                deleteSB.Append(" {");
                deleteSB.AppendLine();
                deleteSB.AppendFormat("  if ({0} != null && !string.IsNullOrEmpty({0}.{1}))", instanceName, pkName);
                deleteSB.AppendLine();
                deleteSB.Append(" {");
                deleteSB.AppendLine();
                deleteSB.AppendFormat("v.{0}.DeleteOnSubmit(v.{0}.First(d => d.{1} == {2}.{1}));", className,pkName ,instanceName);
                deleteSB.AppendLine();
                deleteSB.Append(" }");
                deleteSB.AppendLine();
                deleteSB.Append(" }");
            }
            return deleteSB;
        }

        StringBuilder GenereateUpdateMethod(string className, string classDesc, string author, string time)
        {
            StringBuilder updateSB = new StringBuilder();

            if (!string.IsNullOrEmpty(className))
            {
                string instanceName = GetInstanceName(className);
                string pkName = className.Split('_')[0] + "_ID";
                updateSB.Append("/// <summary>");
                updateSB.AppendLine();
                if (!string.IsNullOrEmpty(classDesc))
                {
                    updateSB.AppendFormat("/// <para>更新{0}基本信息</para>", classDesc);
                    updateSB.AppendLine();
                }
                if (!string.IsNullOrEmpty(author) && !string.IsNullOrEmpty(time))
                {
                    updateSB.AppendFormat("/// <para>by {0} {1}</para>", author, time);
                    updateSB.AppendLine();
                }
                else if (!string.IsNullOrEmpty(author))
                {
                    updateSB.AppendFormat("/// <para>by {0}</para>", author);
                    updateSB.AppendLine();
                }
                updateSB.Append(" /// </summary>");
                updateSB.AppendLine();
                updateSB.Append(" /// <param name=\"v\">数据库连接对象</param>");
                updateSB.AppendLine();
                updateSB.AppendFormat(" /// <param name=\"{0}\">{1}基本信息更新对象</param>", instanceName, classDesc);
                updateSB.AppendLine();
                updateSB.AppendFormat(" public void UpdateData(sxgismis2 v, {0} {1})", className, instanceName);
                updateSB.AppendLine();
                updateSB.Append(" {");
                updateSB.AppendLine();
                updateSB.AppendFormat("  if ({0} != null && !string.IsNullOrEmpty({0}.{1}))", instanceName, pkName);
                updateSB.AppendLine();
                updateSB.Append(" {");
                updateSB.AppendLine();
                updateSB.AppendFormat(" {0} model = v.{0}.First(d => d.{1} == {2}.{1});", className, pkName, instanceName);
                updateSB.AppendLine();
                updateSB.AppendFormat("ClassValueCopier.Copy(model, {0},\"{1}_CT\",\"{1}_CREATOR\");", instanceName, className.Split('_')[0]);
                updateSB.AppendLine();
                updateSB.Append(" }");
                updateSB.AppendLine();
                updateSB.Append(" }");
            }
            return updateSB;
        }

        //获得实例名
        string GetInstanceName(string className)
        {
            string instanceName = string.Empty;
            if (!string.IsNullOrEmpty(className))
            {
                string[] separate = className.Split('_');
                if (separate.Length >= 2)
                {
                    for(int i=1;i<separate.Length;i++)
                    {
                        if(i==1)
                        {
                            instanceName += separate[i].ToLower();
                        }
                        else
                        {
                            instanceName += System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(separate[i].ToLower());
                        }
                        
                    }
                }
                else if (separate.Length == 1)
                {
                    instanceName = separate[0].ToLower();
                }
            }
            return instanceName;
        }
        #endregion
    }
}
