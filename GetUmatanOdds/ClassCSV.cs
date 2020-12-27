using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace GetUmatanOdds
{
    public class ClassCSV
    {
        public string dataCsvAll;
        public string[] linedataCsvAll;
        public string[,] arrdataCsvAll;

        public void CreateCSVarrdata()
        {
            int colmax = 9;
            arrdataCsvAll = new string[3820, colmax];
        }

        public void CreateCSVdataAll()
        {
            string linedataCsv;
            bool isAllNull = false;
            for (int i = 0; i < arrdataCsvAll.GetLength(0); i++)
            {
                linedataCsv = "";
                isAllNull = true;
                for (int j = 0; j < arrdataCsvAll.GetLength(1); j++)
                {
                    if (arrdataCsvAll[i, j] != null)
                        isAllNull = false;
                    linedataCsv += arrdataCsvAll[i, j] + ",";
                }
                if (isAllNull)
                    break;
                linedataCsv = linedataCsv.Substring(0, linedataCsv.Length - 1);
                dataCsvAll += linedataCsv + "\r\n";
            }
        }


        public void setData(long indRow, long indCol, string InputData)
        {
            //string[] arrdataCsv;
            //string[] arrdatadataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //if (indCol > arrdataCsv.Length)
            //{
            //    Array.Resize(ref arrdataCsv, (int)indRow);
            //}
            //arrdatadataCsv = arrdataCsv[indRow - 1].Split(',');
            //if (indCol > arrdatadataCsv.Length)
            //{
            //    Array.Resize(ref arrdatadataCsv, (int)indCol);
            //}
            //arrdatadataCsv[indCol - 1] = InputData;
            //arrdataCsv[indRow - 1] = string.Join(",", arrdatadataCsv);
            //dataCsvAll = string.Join("\r\n", arrdataCsv);

            arrdataCsvAll[indRow - 1, indCol - 1] = InputData;

        }

        public string getData(long indRow, long indCol)
        {
            //string[] arrdataCsv;
            //string[] arrdatadataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //arrdatadataCsv = arrdataCsv[indRow - 1].Split(',');
            //return arrdatadataCsv[indCol - 1];

            return arrdataCsvAll[indRow - 1, indCol - 1];
        }
        public long getDataMaxRow()
        {
            //string[] arrdataCsv;
            //arrdataCsv = dataCsvAll.Split(new[] { "\r\n" }, StringSplitOptions.None);
            //return arrdataCsv.Length - 1;

            return arrdataCsvAll.GetLength(0);
        }

    }
}
