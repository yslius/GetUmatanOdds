using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetUmatanOdds
{
    class UmatanOddsRT
    {
        Form1 _form1;
        private clsCodeConv objCodeConv;
        private ClassLog cLog;
        int size = 0;
        int count = 0;
        clcCommon cCommon;

        public UmatanOddsRT(clcCommon cCommon1, Form1 form1)
        {
            _form1 = form1;
            cCommon = cCommon1;
            cLog = new ClassLog();
            objCodeConv = new clsCodeConv();
        }

        void GetRTDataDetailData(ClassCSV cCSV, string strDateTarg)
        {
            List<clcUmatanOdds> listUmatanOdds = new List<clcUmatanOdds>();
            string codeJyo;
            string numRace;
            

            //速報オッズ（馬単）の呼び出し


            //速報オッズ（単複枠）の呼び出し


            //速報票数(全賭式)の呼び出し


            //馬単裏計算


            //馬単合成計算


            //3連単1・2着軸総流し計算
            //３連単オッズの呼び出し


        }

        bool GetRTDataDetailData1(ClassCSV cCSV, string strDateTarg, string codeJyo, string numRace)
        {
            clcUmatanOdds cUmatanOdds = new clcUmatanOdds();

            string retbuff;
            long cntLoop = 0;

            if (cCommon.checkInit() != 0)
                return false;
            if (!cCommon.isJVOpenReal("0B34", strDateTarg + codeJyo + numRace))
            {
                _form1.axJVLink1.JVClose();
                return false;
            }
            retbuff = cCommon.loopJVRead(size, count, false);
            if (retbuff == "" || retbuff == "END")
            {
                _form1.axJVLink1.JVClose();
                return false;
            }
            JVData_Struct.JV_O4_ODDS_UMATAN mO4Data =
                new JVData_Struct.JV_O4_ODDS_UMATAN();
            mO4Data.SetDataB(ref retbuff);


            return true;

        }


    }
}
