using GenerateProgrammeCode.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GenerateProgrammeCode.Oracle
{
    /// <summary>
    /// GenerateAutoCode.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateAutoCode : UserControl
    {
        public GenerateAutoCode()
        {
            InitializeComponent();

            //是否使用表绑定数据源
            cmbIsUseTable.ItemsSource = new List<string>() { "是", "否" };
            //是否使用表改变事件
            cmbIsUseTable.SelectionChanged += CmbIsUseTable_SelectionChanged;

            cmbIsUseTable.SelectedIndex = 0;
        }

        //是否使用表改变事件
        private void CmbIsUseTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Convert.ToString((sender as ComboBox).SelectedValue) == "是")
            {
                tbkMax.Visibility = Visibility.Hidden;
                tbMax.Visibility = Visibility.Hidden;

                tbkTable.Visibility = Visibility.Visible;
                tbTable.Visibility = Visibility.Visible;
            }
            else
            {
                tbkMax.Visibility = Visibility.Visible;
                tbMax.Visibility = Visibility.Visible;

                tbkTable.Visibility = Visibility.Hidden;
                tbTable.Visibility = Visibility.Hidden;
            }
             
        }

        //点击自动生成数据库执行代码
        private void btnGenerateAuto_Click(object sender, RoutedEventArgs e)
        {
            rtbCode.Document.Blocks.Clear();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\autoCode.sql";
            string sql = CommonHelper.LoadFileText(filePath, Encoding.GetEncoding(936));

            //验证数据的完整性
            if (string.IsNullOrEmpty(tbCode.Text))
            {
                MessageBox.Show("域编码不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(tbField.Text))
            {
                MessageBox.Show("字段名不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(tbPre.Text))
            {
                MessageBox.Show("前缀不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(tbLength.Text))
            {
                MessageBox.Show("自增长长度不能为空！");
                return;
            }

            if(Convert.ToString(cmbIsUseTable.SelectedValue)=="是")
            {
                if (string.IsNullOrEmpty(tbTable.Text))
                {
                    MessageBox.Show("表名不能为空！");
                    return;
                }

                sql = sql.Replace("{v_max}", "0");//给一个默认值 0
                sql = sql.Replace("{v_flag}", "");//移除这个标记
                sql = sql.Replace("{v_table}", tbTable.Text);
                sql = sql.Replace("{v_isUseTable}", "1");
            }
            else
            {
                if (string.IsNullOrEmpty(tbMax.Text))
                {
                    MessageBox.Show("最大值不能为空！");
                    return;
                }
                sql = sql.Replace("{v_max}", tbMax.Text);
                sql = sql.Replace("{v_flag}", "--");//将查询表一块的SQL注释掉
                sql = sql.Replace("{v_isUseTable}", "0");
            }

            sql =sql.Replace("{v_id}", tbCode.Text);
            sql = sql.Replace("{v_name}", tbField.Text);
            sql = sql.Replace("{v_pre}", tbPre.Text);
            sql = sql.Replace("{v_length}", tbLength.Text);
                 
            rtbCode.AppendText(sql);

        }
    }
}
