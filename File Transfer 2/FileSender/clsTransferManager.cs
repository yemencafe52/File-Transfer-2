using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSender
{
    static class TransferManager
    {
        internal delegate void CallBack(FileSender fs);
        private static int number = 0;

        private static List<FileSender> t = new List<FileSender>();

        public static FileSender Add(string filePath,TransferManager.CallBack cb)
        {
            FileSender res = null;

            FileSender fs = new FileSender(++number, "127.0.0.1",1234, filePath, State.SENDING, 0);
            t.Add(fs);

            fs.Send(cb);
            res = fs;
             
            return res;
        }

        public static void Pause(int index)
        {

        }

        public static void Resume(int index)
        {

        }

        public static void PauseALL()
        {

        }

        public static void ResumeALL()
        {

        }

        public static float TotallPB()
        {
            float res = 0;

            float total = 0;
            for(int i=0;i<t.Count;i++)
            {
                total += t[i].Precent;
            }

            res = (float)((total / (((float)(t.Count) * (float)100))) * (float)100);

            return res;
        }
    }
}
