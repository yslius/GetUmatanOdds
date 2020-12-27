using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace GetUmatanOdds
{

    public class UmatanOdds
    {
        Form1 _form1;
        private OperateForm cOperateForm;
        private ClassLog cLog;
        private UmatanOddsStock cUmatanOddsStock;
        private UmatanOddsRT cUmatanOddsRT;
        ClassCSV cCSV;

        public UmatanOdds(clcCommon cCommon, OperateForm cOperateForm1,
            Form1 form1)
        {
            _form1 = form1;
            cOperateForm = cOperateForm1;
            cLog = new ClassLog();
            cUmatanOddsRT = new UmatanOddsRT(cCommon, form1);
            cUmatanOddsStock = new UmatanOddsStock(cCommon, form1);
            cCSV = new ClassCSV();
        }

        public void getUmatanOdds()
        {
            cLog.writeLog("getUmatanOdds");
            cOperateForm.disableButton();

            string pathTarg;
            string placeTarg;
            string racenumTarg;
            string nameFileTarg;

            DateTime datetimeTarg = _form1.dateTimePicker1.Value;
            string strDateTarg = datetimeTarg.ToString("yyyyMMdd");

            strDateTarg = "20201206";
            pathTarg = _form1.textBox1.Text;
            //placeTarg = _form1.listBox1.SelectedItem.ToString();
            placeTarg = "中山";
            //racenumTarg = Strings.StrConv(_form1.listBox1.SelectedItem.ToString(), VbStrConv.Wide);
            racenumTarg = "01";

            // CSV初期化
            var encoding = Encoding.GetEncoding("shift_jis");
            cCSV.CreateCSVarrdata();

            // 追加項目を記入
            writeHeadData(cCSV);

            // 速報開催情報(一括)の呼び出し
            int retval = checkJVRTOpen(datetimeTarg);
            if (retval < -1)
                return;

            if (retval == -1)
            {
                cUmatanOddsStock.GetStockDataDetailData(cCSV, strDateTarg, placeTarg, racenumTarg);
            }
            else
            {
                //cUmatanOddsRT.GetRTDataDetailData(cCSV, strDateTarg);
            }

            // ファイル出力
            racenumTarg = Strings.StrConv("01", VbStrConv.Wide);
            nameFileTarg = "馬単オッズ_" + strDateTarg + "_" + placeTarg + racenumTarg + ".csv";
            cCSV.CreateCSVdataAll();
            File.WriteAllText(pathTarg + "\\" + nameFileTarg, cCSV.dataCsvAll, encoding);

            _form1.axJVLink1.JVClose();
            System.Media.SystemSounds.Asterisk.Play();
            cOperateForm.enableButton();

            _form1.rtbData.Text = nameFileTarg + "\n" +
                "取得完了しました。";

        }

        private int checkJVRTOpen(DateTime datetimeTarg)
        {
            string dataspec = "0B14";
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0);
            string strDate =
                (datetimeTarg - timeSpan).ToString("yyyyMMdd");

            int num2 = _form1.axJVLink1.JVClose();
            if (num2 != 0)
                MessageBox.Show("JVClose エラー：" + num2);

            int num1 = _form1.axJVLink1.JVRTOpen(dataspec, strDate);

            return num1;
        }

        void writeHeadData(ClassCSV cCSV)
        {
            long rowTarget = 1;
            cCSV.setData(rowTarget, 1, "目1");
            cCSV.setData(rowTarget, 2, "目2");
            cCSV.setData(rowTarget, 3, "馬単オッズ");
            cCSV.setData(rowTarget, 4, "人気1");
            cCSV.setData(rowTarget, 5, "人気2");
            cCSV.setData(rowTarget, 6, "馬単票数");
            cCSV.setData(rowTarget, 7, "馬単裏");
            cCSV.setData(rowTarget, 8, "馬単合成");
            cCSV.setData(rowTarget, 9, "3連単1・2着軸総流し");
        }

        

    }
}
