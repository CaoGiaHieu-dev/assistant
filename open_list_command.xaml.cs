using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for open_list_command.xaml
    /// </summary>
    public partial class open_list_command : Window
    {
        private static SqlConnection conn = new SqlConnection(@"Data Source=SHJN\SQLEXPRESS;Initial Catalog=Assistant;Trusted_Connection=yes");

        AssistantEntities dataEntities = new AssistantEntities();

        public open_list_command()
        {
            InitializeComponent();
            dataListView();
        }
        public void dataListView()
        {
            try
            {
                //Select command in DB

                var query = from open_command in dataEntities.open_command select new { open_command.command, open_command.request};

                dataGrid1.ItemsSource = query.ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi ");
            }
        }
    }
}
