using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using WpfApp1.Helper;
using WpfApp1;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region property
        private readonly string path = @"C:\Users\Shjn\Desktop\Assistant\Python\";
        public dynamic speak { get; set; }

        
        #endregion
       
        // event handdle with order view
        public EventHandler Clicked;
        public MainWindow()
        {
            InitializeComponent();

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
            int y = 150;
            if(e.ClickCount >=2)
            {
                speak = null;
                speak = PythonInstance.RunFromCmd(path + "main.py");
                string values = speak.Replace("\r", string.Empty);
                string[] temp = values.Split('\n');
                if (temp[0].Contains("cầu") == false && temp[0].Contains("hình") == false)
                {
                    if (!temp[0].Contains("mở") || !temp[0].Contains("Mở"))
                    {
                        if (temp[1] != "")
                        {
                            Process notepad = new Process();

                            notepad.StartInfo.FileName = "notepad.exe";

                            notepad.Start();

                            notepad.WaitForInputIdle();

                            NodepadHelper.notepadHandle = notepad.MainWindowHandle;

                            NodepadHelper.WriteLineToNotePad(temp[2]);
                        }

                    }
                    else
                    {
                        var charsToRemove = new string[] { "@", ",", ".", ";", "'", "(", ")" };
                        foreach (var c in charsToRemove)
                        {
                            temp[2] = temp[2].Replace(c, string.Empty);
                        }

                        for (int i = 0; i < temp[2].Length; i++)
                        {
                            if (i == y)
                            {
                            Loop: if (temp[2].Substring(i, 1) == " ")
                                {
                                    temp[2] = temp[2].Insert(i, " \n ");
                                    y += 150;
                                }
                                else
                                {
                                    i = i + 1;
                                    goto Loop;
                                }
                            }
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
