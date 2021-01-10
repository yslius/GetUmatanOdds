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

namespace GetUmatanOdds
{
    public partial class Form1 : Form
    {
        private OperateForm cOperateForm;
        clsCommon cCommon;
        private ClassLog cLog = new ClassLog();
        clsDatabase cDatabase;
        private DateTime oldSelectDate;
        private bool isFormLord = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cLog.writeLog("Form1_Load " + this.Text);

            cCommon = new clsCommon(this);
            cDatabase = new clsDatabase(cCommon, this);
            cOperateForm = new OperateForm(this, cDatabase, cCommon);
            oldSelectDate = DateTime.Now;

            // JV-Linkチェック
            if (cCommon.checkInit() != 0)
            {
                //return;
            }

            // CodeTable.csvのチェック
            if (!File.Exists(Application.StartupPath + "\\CodeTable.csv"))
                Application.Exit();

            // DBファイルがなければDB作る
            if (!cDatabase.isExistDbFile())
                if (!cDatabase.isMakeDB())
                {
                    MessageBox.Show("DBが作成できません。");
                    Application.Exit();
                }

            // 前回選択した日付があれば表示する
            List<clsDbInfo> listdbHistory;
            listdbHistory = cDatabase.getDbHistory();
            if (listdbHistory.Count > 0)
            {
                clsDbInfo cDbInfo = listdbHistory[listdbHistory.Count - 1];
                cOperateForm.selectHistory(cDbInfo);
            }
            isFormLord = false;


        }

        private void mnuConfJV_Click(object sender, EventArgs e)
        {
            cLog.writeLog("mnuConfJV_Click");
            cCommon.callMenu();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cLog.writeLog("button1_Click");
            prgJVRead.Value = 0;
            prgDownload.Value = 0;
            cOperateForm.readFolder();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cDatabase.putDbData())
            {
                MessageBox.Show("データ取得完了しました。",
                    "GetUmatanOdds",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            rtbData.Text = "";
        }

        private void btnGetJVData_Click(object sender, EventArgs e)
        {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            cLog.writeLog("btnGetJVData_Click");

            DateTime datetimeTarg = dateTimePicker1.Value;
            string strDateTarg = datetimeTarg.ToString("yyyyMMdd");
            string strPlace;
            string strRace;
            if (this.textBox1.Text == "")
            {
                System.Media.SystemSounds.Asterisk.Play();
                MessageBox.Show("出力ファイルを保存するフォルダを選択してください。");
                cOperateForm.enableButton();
                return;
            }

            try
            {
                strPlace = listBox1.SelectedItem.ToString();
            }
            catch
            {
                System.Media.SystemSounds.Asterisk.Play();
                MessageBox.Show("会場を選択してください。");
                cOperateForm.enableButton();
                return;
            }
            try
            {
                strRace = listBox2.SelectedItem.ToString();
            }
            catch
            {
                System.Media.SystemSounds.Asterisk.Play();
                MessageBox.Show("レースを選択してください。");
                cOperateForm.enableButton();
                return;
            }

            UmatanOdds cUmatanOdds = new UmatanOdds(cCommon, cOperateForm, this);
            cUmatanOdds.getUmatanOdds(strDateTarg, strPlace, strRace);

            // 保持用
            string codeJyo = cCommon.JyogyakuCord(strPlace.Substring(0, 2));
            clsDbInfo cDbInfo = new clsDbInfo();
            cDbInfo.strdate = strDateTarg;
            cDbInfo.nameJyo = strPlace;
            cDbInfo.racenum = strRace;
            cDbInfo.raceId = strDateTarg + codeJyo + strRace;
            cDatabase.putHistory(cDbInfo);

            //sw.Stop();
            //TimeSpan ts = sw.Elapsed;
            //rtbData.Text = $"{ts}";

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (isFormLord)
                return;
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            if (oldSelectDate.Month != dateTimePicker1.Value.Month)
            {
                oldSelectDate = dateTimePicker1.Value;
                return;
            }
            DateTime datetimeTarg = dateTimePicker1.Value;
            string strDateTarg = datetimeTarg.ToString("yyyyMMdd");
            if (!cDatabase.getDbDataDate(strDateTarg))
            {
                MessageBox.Show("データがありません。",
                    "GetUmatanOdds",
                    MessageBoxButtons.OK);
            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;
            string strPlace = listBox1.SelectedItem.ToString();
            listBox2.Items.Clear();
            listBox3.Items.Clear();

            DateTime datetimeTarg = dateTimePicker1.Value;
            string strDateTarg = datetimeTarg.ToString("yyyyMMdd");
            if (!cDatabase.getDbDataPlace(strDateTarg, strPlace))
            {
                MessageBox.Show("データがありません。",
                    "GetUmatanOdds",
                    MessageBoxButtons.OK);
            }
        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null)
                return;
            string strPlace = listBox1.SelectedItem.ToString();
            string strRace = listBox2.SelectedItem.ToString();
            listBox3.Items.Clear();

            DateTime datetimeTarg = dateTimePicker1.Value;
            string strDateTarg = datetimeTarg.ToString("yyyyMMdd");
            if (!cDatabase.getDbDataBamei(strDateTarg, strPlace, strRace))
            {
                MessageBox.Show("データがありません。",
                    "GetUmatanOdds",
                    MessageBoxButtons.OK);
            }
        }

        private void listBox3_Click(object sender, EventArgs e)
        {
            listBox3.SelectedItem = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //cDatabase.putHistory("2101050000");
            cDatabase.getDbHistory();
        }
    }
}
