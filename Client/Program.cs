using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = IPAddress.Parse("192.168.0.215");
            var port = 27001;
            var ep = new IPEndPoint(ip, port);
            Console.WriteLine("Client");
            try
            {
                while (true)
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect(ep);
                        if (client.Connected)
                        {
                            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            Bitmap memoryImage = new Bitmap(1920, 1080);
                            Size s = new Size(memoryImage.Width, memoryImage.Height);
                            using (Graphics memoryGraphics = Graphics.FromImage(memoryImage))
                            {
                                memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                            }

                            string path = Path.Combine(folderPath, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                            memoryImage.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                            Console.WriteLine($"Screenshot saved to: {path}");
                            memoryImage.Dispose();

                            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                var buffer = new byte[1024];
                                int len = 0;
                                using (var networkStream = client.GetStream())
                                {
                                    while ((len = fs.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        networkStream.Write(buffer, 0, len);
                                    }
                                }
                            }
                            Console.WriteLine($"Screenshot sent to server.");
                        }
                    }
                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}