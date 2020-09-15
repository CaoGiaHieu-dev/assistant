using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.Helper
{
    public class NodepadHelper
    {
        [DllImport("USER32.DLL")]

        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static IntPtr notepadHandle;

        public static void waitABit()

        {

            Random r = new Random();

            Thread.Sleep(r.Next(50, 150)); //Lower numbers is faster typing

        }

        public static void waitABitLonger()

        {

            Random r = new Random();

            Thread.Sleep(r.Next(2000, 5000)); //Higher numbers is longer waiting

        }

        public static void SendKeyStroke(string KeyStroke)

        {

            SetForegroundWindow(notepadHandle); //Make sure Notepad is the top window

            SendKeys.SendWait(KeyStroke); //And send a keystroke

        }

        public static void WriteLineToNotePad(string line)

        {

            for (int i = 0; i < line.Length; i++)

            {//for every letter

                //waitABit(); //wait a bit

                SendKeyStroke(line[i].ToString()); //then send the keystroke

            }

        }
    }
}
