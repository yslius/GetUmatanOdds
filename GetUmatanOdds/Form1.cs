using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetUmatanOdds
{
    public partial class Form1 : Form
    {
        private OperateForm cOperateForm;
        clcCommon cCommon;
        private ClassLog cLog = new ClassLog();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cLog.writeLog("Form1_Load " + this.Text);

            cCommon = new clcCommon(this);
            if (cCommon.checkInit() != 0)
            {
                //return;
            }
            cOperateForm = new OperateForm(this);
        }

        private void mnuConfJV_Click(object sender, EventArgs e)
        {
            cLog.writeLog("mnuConfJV_Click");
            //cJVLink.callMenu();
            cCommon.callMenu();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cLog.writeLog("button1_Click");
            prgJVRead.Value = 0;
            prgDownload.Value = 0;
            cOperateForm.readFolder();
        }

        private void btnGetJVData_Click(object sender, EventArgs e)
        {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            cLog.writeLog("btnGetJVData_Click");
            string strTmp;

            if (this.textBox1.Text == "")
            {
                System.Media.SystemSounds.Asterisk.Play();
                MessageBox.Show("出馬表を格納しているフォルダを選択してください。");
                cOperateForm.enableButton();
                return;
            }

            //try
            //{
            //    strTmp = listBox1.SelectedItem.ToString();
            //}
            //catch
            //{
            //    System.Media.SystemSounds.Asterisk.Play();
            //    MessageBox.Show("会場を選択してください。");
            //    cOperateForm.enableButton();
            //    return;
            //}
            //try
            //{
            //    strTmp = listBox2.SelectedItem.ToString();
            //}
            //catch
            //{
            //    System.Media.SystemSounds.Asterisk.Play();
            //    MessageBox.Show("レースを選択してください。");
            //    cOperateForm.enableButton();
            //    return;
            //}

            UmatanOdds cUmatanOdds = new UmatanOdds(cCommon, cOperateForm, this);
            cUmatanOdds.getUmatanOdds();

            //sw.Stop();
            //TimeSpan ts = sw.Elapsed;
            //rtbData.Text = $"{ts}";

        }
    }
}
