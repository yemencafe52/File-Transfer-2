namespace FileSender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Net.Sockets;


   internal enum State : byte
    {
        SENDING =1,
        PAUSED,
        COMPLETE
    }

    internal class FileSender
    {
        private int number;
        private string ip;
        private int port;
        private string filePath;
        private State state;
        private float precent;

        internal FileSender(
         int number,
         string ip,
         int port,
         string filePath,
         State state,
         float precent
            )
        {
            this.number = number;
            this.ip = ip;
            this.port = port;
            this.filePath = filePath;
            this.state = state;
            this.precent = precent;
        }
        internal async void Send(TransferManager.CallBack callback)
        {
            while (this.state == State.SENDING)
            {

                try
                {
                    await Task.Run(() =>
                    {
                        System.IO.FileInfo fi = new FileInfo(this.filePath);
                        MyFileInfo myFileInfo = new MyFileInfo(fi.Length, fi.Name);

                        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        s.Connect(this.ip, this.port);

                        s.Send(myFileInfo.ToBytes());
                        byte[] buffer = new byte[9];

                        int len = s.Receive(buffer);
                        long pos = 0;

                        // OOP ,, FATAL ERROR :)
                        if (len == 9)
                        {
                            if (buffer[0] == 254)
                            {
                                unsafe
                                {
                                    fixed (void* p = &buffer[1])
                                    {
                                        pos = *(long*)p;
                                    }
                                }

                                using (FileStream fs = new FileStream(this.filePath, FileMode.Open))
                                {
                                    fs.Position = pos;
                                    int bufferSize = 1024 * 1024;

                                    while (fs.Length > fs.Position)
                                    {
                                        if (!(fs.Length - fs.Position > bufferSize))
                                        {
                                            bufferSize = (int)(fs.Length - fs.Position);
                                        }

                                        buffer = new byte[bufferSize];
                                        fs.Read(buffer, 0, buffer.Length);
                                        s.Send(buffer);

                                        if (fs.Position == fs.Length)
                                        {
                                            this.state = State.COMPLETE;
                                        }

                                        this.precent =  ((((float)fs.Position / ((float)fs.Length)) * ((float)100)));
                                        callback.Invoke(this);
                                        
                                    }


                                    s.Close();
                                    fs.Close();
                                }
                            }
                        }

                    });

                }
                catch
                { }
            }
        }

        internal int Number
        {
            get
            {
                return this.number;
            }
        }

        internal float Precent
        {
            get
            {
                return this.precent;
            }
        }

        internal State StateInfo
        {
            get
            {
                return this.state;
            }
        }

        internal string FilePath
        {
            get
            {
                return this.filePath;
            }
        }

    }
}
