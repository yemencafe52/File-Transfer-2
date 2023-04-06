using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace FileSender
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void UpdateList(FileSender fs)
        {

            if(this.InvokeRequired)
            {
                this.Invoke(new TransferManager.CallBack(UpdateList),fs);
                return;
            }

            ListViewItem lvi = listView1.FindItemWithText(fs.Number.ToString());

            if (lvi != null)
            {
                lvi.SubItems[2].Text = fs.StateInfo.ToString();
                lvi.SubItems[3].Text = fs.Precent.ToString("#0.#0");
            }

            toolStripProgressBar1.Value = (int)TransferManager.TotallPB();

        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            DialogResult r = ofd.ShowDialog();

            if (r == DialogResult.OK)
            {
                for(int i=0;i<ofd.SafeFileNames.Count();i++)
                {
                    FileSender fs = TransferManager.Add(ofd.FileNames[i], new TransferManager.CallBack(UpdateList));

                    if (fs != null)
                    {
                        ListViewItem lvi = new ListViewItem(fs.Number.ToString());
                        lvi.SubItems.Add(fs.FilePath);
                        lvi.SubItems.Add(fs.StateInfo.ToString());
                        lvi.SubItems.Add(fs.Precent.ToString("#0.#0"));
                        listView1.Items.Add(lvi);
                    }
                }
            }
        }
    }
}
