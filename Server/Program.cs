using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            var ip = IPAddress.Loopback;
            var port = 27001;
            var ep = new IPEndPoint(ip, port);

            var listener = new TcpListener(ep);
            Console.WriteLine("Server");
            try
            {
                listener.Start();

                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    _ = Task.Run(() =>
                    {
                        var networkStream= client.GetStream();
                        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Screenshots_Desktop");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        string path = Path.Combine(folderPath, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                        using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            var len = 0;
                            var bytes = new byte[1024];

                            while ((len = networkStream.Read(bytes, 0, bytes.Length)) > 0)
                            {
                                fs.Write(bytes, 0, len);
                            }
                        }
                        Console.WriteLine("File received");
                        client.Close();
                    });


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}
