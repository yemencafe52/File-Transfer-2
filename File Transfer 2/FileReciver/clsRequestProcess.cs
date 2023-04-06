namespace FileReciver
{
    using System;
    using System.Net.Sockets;
    using System.IO;
    internal class RequestProcess
    {
        internal RequestProcess(Socket s)
        {
            try
            {
                byte[] buffer = new byte[1024 * 8];
                int len = s.Receive(buffer);

                if (len > 0)
                {
                    byte[] temp = new byte[len];
                    Array.Copy(buffer, 0, temp, 0, len);

                    FileInfo fi = new FileInfo(temp);

                    try
                    {
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Downloads");
                    }
                    catch
                    {
                        throw new Exception();
                    }

                    string filePath = Directory.GetCurrentDirectory() + "\\downloads\\" + fi.Name;
                    long fileLen = 0;

                    if(File.Exists(filePath))
                    {
                        System.IO.FileInfo finfo = new System.IO.FileInfo(filePath);
                        long flen = finfo.Length;
                    }

                    byte[] ar4 = new byte[9];
                    ar4[0] = 254;

                    unsafe
                    {
                        void* p2 = &fileLen;

                        for(int i =0;i<8; i++)
                        {
                            ar4[i+1] = *((byte*)p2+i);
                        }
                    }
                    

                    s.Send(ar4);

                    buffer = new byte[1024 * 8];

                    while (s.Connected)
                    {
                        len = s.Receive(buffer);
                        if (len > 0)
                        {
                            //temp = new byte[len];
                            //Array.Copy(buffer, 0, temp, 0, len);

                            FileStream fs = new FileStream(filePath, FileMode.Append);
                            fs.Write(buffer, 0, len);

                            if (fs.Length >= fi.Size)
                            {
                                fs.Close();
                                break;
                            }

                            fs.Close();
                        }
                    }

                    s.Close();
                }

            }
            catch
            { }
        }
    }
}
