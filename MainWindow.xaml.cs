using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Helper;
using System.Windows.Media.Animation;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        #region property
        private readonly string path = @"C:\Users\Shjn\Desktop\Assistant\Python\";
        public dynamic speak { get; set; }
        private string say;
        private SqlCommand cmd;
        private string ConnectionString = "Trusted_Connection=yes;" + "Initial Catalog=;" + @"Data Source=SHJN\SQLEXPRESS;";

        // event handdle with order view
        public EventHandler Clicked;
        #endregion

        #region DB
        private void CreateTatble()
        {
            SqlConnection conn = new SqlConnection(@"Data Source=SHJN\SQLEXPRESS;Initial Catalog=Assistant;Trusted_Connection=yes");
            conn.Open();
            try
            {
                string open_command = "If not exists (select name from sysobjects where name = 'open_command') CREATE TABLE open_command(ID int identity(1,1) NOT NULL Primary Key,command nvarchar(MAX),request nvarchar(MAX) , type int NOT NULL);";
                string search_command = "If not exists (select name from sysobjects where name = 'search_command') CREATE TABLE search_command(ID int identity(1,1) NOT NULL Primary Key,command nvarchar(MAX),request nvarchar(MAX));";

                cmd = new SqlCommand(open_command + search_command, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Đã xảy ra lỗi ");
                conn.Close();
            }
        }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            using (var connection = new SqlConnection(ConnectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "if NOT EXISTS (SELECT name FROM master.dbo.sysdatabases where name ='Assistant') CREATE DATABASE Assistant";
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    CreateTatble();
                    connection.Close();
                }
                catch(Exception e)
                {
                    Console.WriteLine( e );
                    connection.Close();
                }
            }

        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
                if (Left == 0 || Left == SystemParameters.PrimaryScreenWidth - Width)
                {
                    Clicked?.Invoke(sender, e);
                    // Assistant luôn nằm trong 1 cạnh của screen
                    // Trong trường hợp Assistant không dịch chuyển 
                    // Event Clicked sẽ được gọi
                    // Điều kiện có thể thay đổi đê chính xác hơn
                }
                else
                {
                    MoveToEdge();
                    // ngược lại với trường hợp trên 
                    // thì luôn di chuyển về 1 trong 2 cạnh màn hình
                }
            }
            catch
            {

            }
            
        }

        public void MoveToEdge()
        {
            Point centerPoint = new Point(Left + Width / 2, Top + Height / 2);
            double destination = 0;
            if (centerPoint.X > SystemParameters.PrimaryScreenWidth / 2)
            {
                destination = SystemParameters.PrimaryScreenWidth - Width;
                // Trong trường hợp assistant bị kéo lệch sang bên phải thì move sang cạnh phải
            }
            // ngược lại, move sang cạnh trái.

            TimeSpan timeOfAnimation = TimeSpan.FromMilliseconds(200 + Math.Abs(destination - Left));
            // Thời gian phụ thuộc vào khoảng cách với cạnh màn hình
            DoubleAnimation moveToEdge = new DoubleAnimation()
            {
                From = Left,
                To = destination,
                Duration = new Duration(timeOfAnimation),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new QuarticEase()
                {
                    EasingMode = EasingMode.EaseOut
                    // Thêm easing function cho chân thật
                }
            };
            BeginAnimation(LeftProperty, moveToEdge);
            // Đây là cách đơn giản nhất để begin animation
        }

        private void Ellipse_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if(e.ClickCount >=2)
            {
                //get speeking from python and splitit to array when end of line
                speak = PythonInstance.RunFromCmd(path + "main.py");
                string values = speak.Replace("\r", string.Empty);
                string[] temp = values.Split('\n');

                //get command and do request
                //check if it'a have config word or not
                if (temp[0].Contains("cấu") == false && temp[0].Contains("hình") == false)
                {
                    //check if it have open word or not
                    if (!temp[0].Contains("mở") || !temp[0].Contains("Mở"))
                    {
                        //if respone not null
                        if (temp[1] != "")
                        {

                            for ( int a = 1; a<temp.Length;a++)
                            {
                                int y = 150;
                                //remove special char form string
                                var charsToRemove = new string[] { "@", ",", ".", ";", "'", "(", ")" };
                                foreach (var c in charsToRemove)
                                {
                                    temp[a] = temp[a].Replace(c, string.Empty);
                                }

                                //Slipt string into a line
                                for (int i = 0; i < temp[a].Length; i++)
                                {
                                    if (i == y)
                                    {
                                    Loop: if (temp[a].Substring(i, 1) == " ")
                                        {
                                            temp[a] = temp[a].Insert(i, "\n");
                                            y += 150;
                                        }
                                        else
                                        {
                                            i += 1;
                                            goto Loop;
                                        }
                                    }
                                }
                                say += temp[a];
                            }
                            

                            //Write it into notepad
                            Process notepad = new Process();

                            notepad.StartInfo.FileName = "notepad.exe";

                            notepad.Start();

                            notepad.WaitForInputIdle();

                            NodepadHelper.notepadHandle = notepad.MainWindowHandle;

                            NodepadHelper.WriteLineToNotePad(say);
                        }

                    }
                }
                else
                {
                    Config config = new Config();
                    config.Show();
                }
                
            }
            
        }
        //public void CreateAnEllipse()
        //{
        //    // Create an Ellipse    
        //    Ellipse blueRectangle = new Ellipse();
        //    blueRectangle.Height = 100;
        //    blueRectangle.Width = 200;
        //    // Create a blue and a black Brush    
        //    SolidColorBrush blueBrush = new SolidColorBrush();
        //    blueBrush.Color = Colors.Blue;
        //    SolidColorBrush blackBrush = new SolidColorBrush();
        //    blackBrush.Color = Colors.Black;
        //    // Set Ellipse's width and color    
        //    blueRectangle.StrokeThickness = 4;
        //    blueRectangle.Stroke = blackBrush;
        //    // Fill rectangle with blue color    
        //    blueRectangle.Fill = blueBrush;
        //    // Add Ellipse to the Grid.    
        //    LayoutRoot.Children.Add(blueRectangle);
        //}
    }
}
