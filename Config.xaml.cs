using System;
using System.Collections.Generic;
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
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {
	    private static SqlConnection conn = new SqlConnection(@"Data Source=SHJN\SQLEXPRESS;Initial Catalog=Assistant;Trusted_Connection=yes");
        public Config()
        {
            InitializeComponent();
            
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //remove special character in request
            Command.Text = Command.Text.Replace('"'.ToString(), string.Empty);

            
            if (Command.Text==string.Empty && Request.Text==string.Empty)
            {
                MessageBox.Show("Không được để trống dữ liệu !", "Cảnh báo");
            }
            else
            {
                //check connection ( if closed -> open ) 
                if(conn.State== System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                else
                {
                    try
                    {
                        //delete request if exits
                        string delete = "delete from open_command where command = (N'" + Command.Text + "');";

                        //set type for command
                        int type;
                        if (Request.Text.Contains("http"))
                        {
                            type = 2;
                        }
                        else
                        {
                            type = 1;
                        }

                        //insert to table
                        SqlCommand cmd = new SqlCommand(delete + "insert into open_command values('" + Command.Text + "','" + Request.Text + "'," + type + ");", conn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Thành công");
                        Command.Text = string.Empty;
                        Request.Text = string.Empty;
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi ");
                    }
                }
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
