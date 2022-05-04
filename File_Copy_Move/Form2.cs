using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace File_Copy_Move
{
    public partial class Form2 : Form
    {

        public delegate void SetProgCallBack(int vy);
        public delegate void SetLabelCallBack(string str);

        private Thread t1;
        private byte[] bts = new byte[4096];
        private FileStream fsSrc = null;
        private FileStream fsDest = null;

        public string FileName { get; set; }
        public Form2(string src, string dest)
        {
            InitializeComponent();

            fsSrc = new FileStream(src, FileMode.Open, FileAccess.Read);
            fsDest = new FileStream(dest, FileMode.Create, FileAccess.Write);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.pgbCopy.Maximum = 100;
            this.lblFileName.Text = " 파 일 : " + FileName;
            t1 = new Thread(new ThreadStart(FileCopy));
            t1.Start();
        }

        private void FileCopy()
        {
            int vv = 1;
            int cnt = 0;
            int kk = (int)(fsSrc.Length / 4096) - 1;
            int ss = (int)(fsSrc.Length % 4096);

            while(true)
            {
                Thread.Sleep(10);
                if(vv>=100)
                    break;
                bts = new byte[4096];

                if(cnt<=kk)
                {
                    fsSrc.Seek(4096 * cnt, SeekOrigin.Begin);
                    fsSrc.Read(bts, 0, 4096);

                    fsDest.Seek(4096 * cnt, SeekOrigin.Begin);
                    fsDest.Write(bts, 0, 4096);
                }
                else
                {
                    fsSrc.Seek(4096 * cnt, SeekOrigin.Begin);
                    fsSrc.Read(bts, 0, ss);

                    fsDest.Seek(4096 * cnt, SeekOrigin.Begin);
                    fsDest.Write(bts, 0, ss);
                }

                cnt++;

                vv = (int)(fsDest.Length * 100 / fsSrc.Length);

                if (vv > 100)
                    SetProgBar(100);
                else
                    SetProgBar(vv);

                SetLabel("복사 : " + vv.ToString() + "%");
            }
            fsDest.Close();
            fsSrc.Close();
            DialogResult = DialogResult.OK;
        }

        private void SetProgBar(int vv)
        {
            if (this.pgbCopy.InvokeRequired)
            {
                SetProgCallBack del = new SetProgCallBack(SetProgBar);
                this.Invoke(del, new object[] { vv });
            }
            else
                this.pgbCopy.Value = vv;
        }

        private void SetLabel(string str)
        {
            if (this.lblCopy.InvokeRequired)
            {
                SetLabelCallBack del = new SetLabelCallBack(SetLabel);
                this.Invoke(del, new object[] { str });
            }
            else
                this.lblCopy.Text = str;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(t1 != null)
            {
                t1.Abort();
            }
        }
    }
}
