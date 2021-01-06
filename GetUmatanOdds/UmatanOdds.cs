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

        public UmatanOdds(clsCommon cCommon, OperateForm cOperateForm1,
            Form1 form1)
        {
            _form1 = form1;
            cOperateForm = cOperateForm1;
            cLog = new ClassLog();
            cUmatanOddsRT = new UmatanOddsRT(cCommon, form1);
            cUmatanOddsStock = new UmatanOddsStock(cCommon, form1);
            cCSV = new ClassCSV();
        }

        public void getUmatanOdds(string strDateTarg, string placeTarg, string racenumTarg)
        {
            cLog.writeLog("getUmatanOdds");
            cOperateForm.disableButton();

            string pathTarg;
            string nameFileTarg;
            
            pathTarg = _form1.textBox1.Text;
            placeTarg = placeTarg.Replace("競馬場","");

            //strDateTarg = "20201206"; //20201206 20210105
            //placeTarg = "中山";
            //racenumTarg = "01";

            // CSV初期化
            var encoding = Encoding.GetEncoding("shift_jis");
            cCSV.CreateCSVarrdata();

            // 追加項目を記入
            writeHeadData(cCSV);

            // 速報開催情報(一括)の呼び出し
            int retval = checkJVRTOpen(strDateTarg);
            if (retval < -1)
                return;

            if (retval == -1)
            {
                cUmatanOddsStock.GetStockDataDetailData(cCSV, strDateTarg, placeTarg, racenumTarg);
            }
            else
            {
                cUmatanOddsRT.GetRTDataDetailData(cCSV, strDateTarg, placeTarg, racenumTarg);
            }

            // ファイル出力
            racenumTarg = Strings.StrConv(racenumTarg, VbStrConv.Wide);
            nameFileTarg = "馬単オッズ_" + strDateTarg + "_" + placeTarg + racenumTarg + ".csv";
            cCSV.CreateCSVdataAll();
            File.WriteAllText(pathTarg + "\\" + nameFileTarg, cCSV.dataCsvAll, encoding);

            _form1.axJVLink1.JVClose();
            System.Media.SystemSounds.Asterisk.Play();
            cOperateForm.enableButton();

            _form1.rtbData.Text = nameFileTarg + "\n" +
                "取得完了しました。";

        }

        private int checkJVRTOpen(string strDateTarg)
        {
            string dataspec = "0B14";

            int ret = _form1.axJVLink1.JVClose();
            if (_form1.axJVLink1.JVClose() != 0)
                MessageBox.Show("JVClose エラー：" + ret);

            ret = _form1.axJVLink1.JVRTOpen(dataspec, strDateTarg);

            return ret;
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
