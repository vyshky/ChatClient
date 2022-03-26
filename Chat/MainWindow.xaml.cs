using System.Windows;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Chat
{

    public partial class MainWindow : Window
    {
        TcpClient client;
        NetworkStream netStream;
        List<string> Names;
        public MainWindow()
        {
            InitializeComponent();
            Names = new List<string>() { "Bujhm", "Александр", "Дмитрий", "Василий" };

            client = new TcpClient("127.0.0.1", 8888);
            netStream = client.GetStream();

            Thread thread = new Thread(ServerListener);
            thread.Start();

            Random random = new Random();
            SendMessage("<Name>" + Names[random.Next(Names.Count)]);
        }

        void ServerListener()
        {
            while (true)
            {
                NetworkStream _stream = client.GetStream();
                byte[] buffer = new byte[255];
                _stream.Read(buffer, 0, 255);

                string message = Encoding.Default.GetString(buffer);
                message = message.Remove(message.IndexOf("\0"));
                if (message.IndexOf("<LIST>") == 0)
                {
                    message = message.Remove(0, 6);
                    List<string> names = JsonConvert.DeserializeObject<List<string>>(message);

                    Dispatcher.Invoke(new System.Action(() => Clients.Items.Clear()));


                    foreach (string name in names)
                    {
                        Dispatcher.Invoke(new System.Action(() => Clients.Items.Add(name)));
                    }
                }
                else
                {
                    message += "\n";
                    Dispatcher.Invoke(new System.Action(() => History.AppendText(message)));
                }
            }
        }
        void SendMessage(string message)
        {
            byte[] buffer;
            buffer = Encoding.Default.GetBytes(message);

            netStream.Write(buffer, 0, message.Length);
            netStream.Flush();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string message = Message.Text;
            SendMessage(message);
            Message.Text = "";

        }
    }
}
