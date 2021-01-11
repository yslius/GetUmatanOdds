using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetUmatanOdds
{
    public class UmatanOddsRT
    {
        Form1 _form1;
        private clsCodeConv objCodeConv;
        private ClassLog cLog;
        //int size = 0;
        int count = 0;
        clsCommon cCommon;
        public List<clsUmatanOdds> listUmatanOddsH1 = new List<clsUmatanOdds>();
        public List<clsRaceUma> listUmatanOddsO1 = new List<clsRaceUma>();
        public List<clsUmatanOdds> listUmatanOdds = new List<clsUmatanOdds>();
        public List<clsOddsSanrentan> listOddsSanrentan = new List<clsOddsSanrentan>();

        public UmatanOddsRT(clsCommon cCommon1, Form1 form1)
        {
            _form1 = form1;
            cCommon = cCommon1;
            cLog = new ClassLog();
            objCodeConv = new clsCodeConv();
        }

        public void GetRTDataDetailData(ClassCSV cCSV, string strDateTarg,
            string placeTarg, string racenumTarg)
        {
            //データ取得する
            _form1.axJVLink1.JVClose();
            if (cCommon.checkInit() != 0)
                return;

            if (!GetRTDataDetailData1(cCSV, strDateTarg, placeTarg, racenumTarg))
            {
                _form1.axJVLink1.JVClose();
                return;
            }
            _form1.prgDownload.Value = 51;
            _form1.prgDownload.Value--;


            //計算する
            cCommon.CreateCompositeOdds(cCSV, listUmatanOddsH1, listUmatanOddsO1,
                listUmatanOdds, listOddsSanrentan);

            _form1.prgDownload.Maximum++;
            _form1.prgDownload.Value = _form1.prgDownload.Maximum;
            _form1.prgDownload.Maximum--;
        }

        bool GetRTDataDetailData1(ClassCSV cCSV, string strDateTarg,
            string placeTarg, string racenumTarg)
        {
            string codeJyo;
            string retbuff;

            codeJyo = cCommon.JyogyakuCord(placeTarg);
            if (codeJyo == "")
                return false;

            //速報オッズ（馬単）の呼び出し
            retbuff = GeJVRTRead(strDateTarg, codeJyo, racenumTarg, "0B34", 4100);
            if (retbuff == null)
                return false;
            listUmatanOdds = cCommon.setDataO4(retbuff, strDateTarg,
                placeTarg, racenumTarg);

            //速報オッズ（単複枠）の呼び出し
            retbuff = GeJVRTRead(strDateTarg, codeJyo, racenumTarg, "0B31", 1000);
            if (retbuff == null)
                return false;
            listUmatanOddsO1 = cCommon.setDataO1(retbuff, strDateTarg,
                placeTarg, racenumTarg);


            //３連単オッズの呼び出し
            retbuff = GeJVRTRead(strDateTarg, codeJyo, racenumTarg, "0B36", 110000);
            if (retbuff == null)
                return false;
            listOddsSanrentan = cCommon.setDataO6(retbuff, strDateTarg,
                placeTarg, racenumTarg);

            //速報票数(全賭式)の呼び出し
            retbuff = GeJVRTRead(strDateTarg, codeJyo, racenumTarg, "0B20", 30000);
            if (retbuff != null)
            {
                listUmatanOddsH1 = cCommon.setDataH1(retbuff, strDateTarg,
                placeTarg, racenumTarg);
            }

            return true;

        }

        string GeJVRTRead(string strDateTarg, string codeJyo, string numRace, string codeRT, int size)
        {
            clsUmatanOdds cUmatanOdds = new clsUmatanOdds();

            string retbuff;

            if (cCommon.checkClose() != 0)
                return null;
            if (cCommon.checkInit() != 0)
                return null;
            if (!cCommon.isJVRTOpen(codeRT, strDateTarg + codeJyo + numRace))
            {
                _form1.axJVLink1.JVClose();
                return null;
            }
            retbuff = cCommon.loopJVRead(size, count, false);
            if (retbuff == "" || retbuff == "END")
            {
                _form1.axJVLink1.JVClose();
                return null;
            }

            return retbuff;
        }


    }
}
