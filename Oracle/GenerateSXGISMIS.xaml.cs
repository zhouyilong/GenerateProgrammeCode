using System;
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

namespace GenerateProgrammeCode.Oracle
{
    /// <summary>
    /// GenerateSXGISMIS.xaml 的交互逻辑
    /// </summary>
    public partial class GenerateSXGISMIS : UserControl
    {
        public GenerateSXGISMIS()
        {
            InitializeComponent();

            cmbTable.ItemsSource = new List<string>() { "A10_DM", "A10C_DMVALUE", "A10CC_AUTH", "A10CC_DMVALUE", "A10CC_GROUP", "A10CC_NOTNULL" };
        }

        private void btnGenerateSXGISMIS_Click(object sender, RoutedEventArgs e)
        {
            rtbCode.Document.Blocks.Clear();
            if (!string.IsNullOrEmpty(tbCode.Text)&&cmbTable.SelectedItem != null)
            {
                string strSelectedItem = cmbTable.SelectedItem as string;
                switch (strSelectedItem)
                {
                    case "A10_DM":
                        rtbCode.AppendText("select * from A10_DM a10 where a10.a10_code='" + tbCode.Text + "'");
                        break;
                    case "A10C_DMVALUE":
                        rtbCode.AppendText("select * from A10C_DMVALUE t where t.a10_id in (select a10.a10_id from A10_DM a10 where a10.a10_code ='" + tbCode.Text + "')");
                        break;
                    case "A10CC_AUTH":
                        rtbCode.AppendText("select * from A10CC_AUTH t where t.a10c_id in (select a10c.a10c_id from A10C_DMVALUE a10c where a10c.a10_id in (select a10.a10_id from A10_DM a10 where a10.a10_code ='" + tbCode.Text + "'))");
                        break;
                    case "A10CC_DMVALUE":
                        rtbCode.AppendText("select * from A10CC_DMVALUE t where t.a10c_id in (select a10c.a10c_id from A10C_DMVALUE a10c where a10c.a10_id in (select a10.a10_id from A10_DM a10 where a10.a10_code ='" + tbCode.Text + "'))");
                        break;
                    case "A10CC_GROUP":
                        rtbCode.AppendText("select * from A10CC_GROUP t where t.a10c_id in (select a10c.a10c_id from A10C_DMVALUE a10c where a10c.a10_id in (select a10.a10_id from A10_DM a10 where a10.a10_code ='" + tbCode.Text + "'))");
                        break;
                    case "A10CC_NOTNULL":
                        rtbCode.AppendText("select * from A10CC_NOTNULL t where t.a10c_id in (select a10c.a10c_id from A10C_DMVALUE a10c where a10c.a10_id in (select a10.a10_id from A10_DM a10 where a10.a10_code ='" + tbCode.Text + "'))");
                        break;
                }
            }
        }
    }
}
