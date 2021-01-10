using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace GetUmatanOdds
{
    public class clsDatabase
    {
        Form1 _form1;
        public string filenameDB = "GetUmatanOdds.db";
        public string nameTableMain = "tablemain";
        public string nameColumnMain1 = "race_date";
        public string nameColumnMain2 = "name_place";
        public string nameColumnMain3 = "race_number";
        public string nameColumnMain4 = "race_id";
        public string nameTableSub = "tablesub";
        public string nameColumnSub1 = "race_id";
        public string nameColumnSub2 = "umaban";
        public string nameColumnSub3 = "bamei";
        public string nameTableHistory = "tablehistory";
        public string nameColumnHis1 = "race_date";
        public string nameColumnHis2 = "name_place";
        public string nameColumnHis3 = "race_number";
        public string nameColumnHis4 = "race_id";
        public string nameColumnHis5 = "created_at";

        clsCommon cCommon;
        clsCodeConv objCodeConv;
        List<clsDbInfo> listRaceInfo;
        List<clsDbInfo> listdbInfo;

        public clsDatabase(clsCommon cCommon1, Form1 form1)
        {
            cCommon = cCommon1;
            _form1 = form1;
            objCodeConv = new clsCodeConv();
            objCodeConv.FileName = Application.StartupPath + "\\CodeTable.csv";
        }

        public bool isExistDbFile()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + filenameDB))
                return true;
            else
                return false;
        }
        public bool isMakeDB()
        {
            string strSqlMain = "CREATE TABLE " + nameTableMain + "(" +
                                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                nameColumnMain1 + " TEXT, " +
                                nameColumnMain2 + " TEXT, " +
                                nameColumnMain3 + " TEXT, " +
                                nameColumnMain4 + " TEXT)";

            string strSqlSub = "CREATE TABLE " + nameTableSub + "(" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            nameColumnSub1 + " TEXT, " +
                            nameColumnSub2 + " TEXT, " +
                            "bamei TEXT)";

            string strSqlHis = "CREATE TABLE " + nameTableHistory + "(" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                            nameColumnHis1 + " TEXT, " +
                            nameColumnHis2 + " TEXT, " +
                            nameColumnHis3 + " TEXT, " +
                            nameColumnHis4 + " TEXT, " +
                            nameColumnHis5 + " TEXT)";
            try
            {
                using (var conn = new SQLiteConnection("Data Source=" + filenameDB))
                {
                    conn.Open();
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = strSqlMain;
                        command.ExecuteNonQuery();
                        command.CommandText = strSqlSub;
                        command.ExecuteNonQuery();
                        command.CommandText = strSqlHis;
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー",
                                MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }

            return true;
        }

        public bool putDbData()
        {
            DateTime datetimeTarg = DateTime.Now;

            List<string> listdbDate = new List<string>();
            listdbDate = getDbDate();

            // JV-Link初期化
            _form1.axJVLink1.JVClose();
            if (cCommon.checkInit() != 0)
                return false;

            // JV-Linkからデータの取得
            listRaceInfo = new List<clsDbInfo>();
            listdbInfo = new List<clsDbInfo>();
            if (!getDbDataJvLink(datetimeTarg, listdbDate))
            {
                MessageBox.Show("JV-Linkからデータの取得に失敗しました。",
                    "GetUmatanOdds",
                    MessageBoxButtons.OK);
                return false;
            }

            // DBへ登録取得
            if (!putJvLinkData())
            {
                MessageBox.Show("DBの登録に失敗しました。",
                    "GetUmatanOdds",
                    MessageBoxButtons.OK);
                return false;
            }

            return true;
        }

        public bool putHistory(clsDbInfo cDbInfo)
        {
            DateTime datetimeCreate = DateTime.Now;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + filenameDB))
                {
                    conn.Open();
                    using (SQLiteTransaction trans = conn.BeginTransaction())
                    {
                        SQLiteCommand cmd = conn.CreateCommand();
                        string strsql = "INSERT INTO " +
                                nameTableHistory + " (" +
                                nameColumnHis1 + "," +
                                nameColumnHis2 + "," +
                                nameColumnHis3 + "," +
                                nameColumnHis4 + "," +
                                nameColumnHis5 + ") VALUES (" +
                                "@" + nameColumnHis1 + ", " +
                                "@" + nameColumnHis2 + ", " +
                                "@" + nameColumnHis3 + ", " +
                                "@" + nameColumnHis4 + ", " +
                                "@" + nameColumnHis5 + ")";
                        cmd.CommandText = strsql;
                        cmd.Parameters.Add(nameColumnHis1, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnHis2, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnHis3, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnHis4, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnHis5, System.Data.DbType.String);
                        cmd.Parameters[nameColumnHis1].Value = cDbInfo.strdate;
                        cmd.Parameters[nameColumnHis2].Value = cDbInfo.nameJyo;
                        cmd.Parameters[nameColumnHis3].Value = cDbInfo.racenum;
                        cmd.Parameters[nameColumnHis4].Value = cDbInfo.raceId;
                        cmd.Parameters[nameColumnHis5].Value = datetimeCreate;
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                return false;
            }

            return true;
        }

        public List<clsDbInfo> getDbHistory()
        {
            List<clsDbInfo> listdbHistory = new List<clsDbInfo>();

            using (SQLiteConnection conn =
                new SQLiteConnection("Data Source=" + filenameDB))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " + "*" +
                    " FROM " + nameTableHistory;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listdbHistory.Add(new clsDbInfo()
                        {
                            strdate = reader[nameColumnHis1].ToString(),
                            nameJyo = reader[nameColumnHis2].ToString(),
                            racenum = reader[nameColumnHis3].ToString(),
                            raceId = reader[nameColumnHis4].ToString(),
                        });
                    }
                }
                conn.Close();
            }
            return listdbHistory;
        }

        List<string> getDbDate()
        {
            List<string> listdbData = new List<string>();

            using (SQLiteConnection conn =
                new SQLiteConnection("Data Source=" + filenameDB))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " + "*" +
                    " FROM " + nameTableMain;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool isFind = false;
                        foreach (string dbData in listdbData)
                        {
                            if (reader[nameColumnMain1].ToString() ==
                                dbData)
                            {
                                isFind = true;
                                break;
                            }
                        }
                        if (!isFind)
                            listdbData.Add(reader[nameColumnMain1].ToString());
                    }
                }
                conn.Close();
            }
            return listdbData;
        }

        bool getDbDataJvLink(DateTime datetimeTarg, List<string> listdbDate)
        {
            JVData_Struct.JV_RA_RACE jvRaRace = new JVData_Struct.JV_RA_RACE();
            JVData_Struct.JV_SE_RACE_UMA jvSeRaceUma = new JVData_Struct.JV_SE_RACE_UMA();
            string retbuff;
            long cntLoop = 0;
            bool isExistData = false;
            int size = 40000;
            int count = 256;
            int option;
            string strDate;
            double bunbo;
            DateTime DateTimeStart;

            if (listdbDate.Count > 0)
            {
                strDate = listdbDate[listdbDate.Count-1];
                DateTimeStart = DateTime.Parse(listdbDate[listdbDate.Count - 1].Insert(4, "/").Insert(7, "/"));
                option = 1;
            }
            else
            {
                TimeSpan timeSpan = new TimeSpan(365 * 4, 0, 0, 0); //365*4
                strDate = (datetimeTarg - timeSpan).ToString("yyyyMMdd");
                DateTimeStart = datetimeTarg - timeSpan;
                option = 4;
            }
            bunbo = (datetimeTarg - DateTimeStart).TotalDays;

            _form1.prgJVRead.Maximum = 100;
            _form1.prgJVRead.Value = 0;
            if (!cCommon.isJVOpen("RACE", strDate, option))
                return false;

            do
            {
                retbuff = cCommon.loopJVRead(size, count, true);
                if (retbuff == "" || retbuff == "END")
                    break;
                if (retbuff.Substring(0, 2) == "RA")
                {
                    jvRaRace.SetDataB(ref retbuff);

                    // 中央競馬以外は除外
                    int intJyoCD;
                    if (!int.TryParse(jvRaRace.id.JyoCD, out intJyoCD))
                        continue;
                    else if (intJyoCD < 1 || intJyoCD > 10)
                        continue;

                    // プログレスバー更新
                    DateTime dateTimeJv = DateTime.Parse((retbuff.Substring(11, 8)).Insert(4, "/").Insert(7, "/"));
                    if(dateTimeJv > datetimeTarg)
                        continue;
                    double bunshi = (dateTimeJv - DateTimeStart).TotalDays;
                    if (bunshi < 0)
                        bunshi = 0;
                    _form1.prgJVRead.Value = (int)(bunshi / bunbo * 100);
                    if(_form1.prgJVRead.Value < _form1.prgJVRead.Maximum)
                    {
                        _form1.prgJVRead.Value++;
                        _form1.prgJVRead.Value--;
                    }
                    _form1.rtbData.Text = jvRaRace.id.Year + jvRaRace.id.MonthDay;
                    _form1.rtbData.Refresh();

                    // DBにデータがある場合は除外
                    if (isExistDateList(listdbDate, jvRaRace.id.Year + jvRaRace.id.MonthDay))
                    {
                        isExistData = true;
                        //_form1.axJVLink1.JVSkip();
                        continue;
                    }

                    isExistData = false;

                    // テキストボックス更新
                    string codeName = this.objCodeConv.GetCodeName("2001", jvRaRace.id.JyoCD, 1);
                    _form1.rtbData.Text = jvRaRace.id.Year +
                        jvRaRace.id.MonthDay + " " +
                        codeName + " " +
                        "取得中";
                    _form1.rtbData.Refresh();

                    listRaceInfo.Add(new clsDbInfo()
                    {
                        strdate = jvRaRace.id.Year + jvRaRace.id.MonthDay,
                        nameJyo = codeName,
                        racenum = jvRaRace.id.RaceNum,
                        raceId = jvRaRace.id.Year + jvRaRace.id.MonthDay +
                        jvRaRace.id.JyoCD + jvRaRace.id.RaceNum,
                    });
                }
                else if (retbuff.Substring(0, 2) == "SE")
                {
                    if (isExistData)
                    {
                        _form1.axJVLink1.JVSkip();
                        continue;
                    }
                    jvSeRaceUma.SetDataB(ref retbuff);

                    // 中央競馬以外は除外
                    int intJyoCD;
                    if (!int.TryParse(jvSeRaceUma.id.JyoCD, out intJyoCD))
                        continue;
                    else if (intJyoCD < 1 || intJyoCD > 10)
                        continue;

                    string codeName = objCodeConv.GetCodeName("2001", jvSeRaceUma.id.JyoCD, 1);
                    foreach (clsDbInfo RaceInfo in listRaceInfo)
                    {
                        if (RaceInfo.strdate ==
                            jvSeRaceUma.id.Year + jvSeRaceUma.id.MonthDay &&
                            RaceInfo.nameJyo == codeName &&
                            RaceInfo.racenum == jvSeRaceUma.id.RaceNum)
                        {
                            listdbInfo.Add(new clsDbInfo()
                            {
                                strdate = RaceInfo.strdate,
                                nameJyo = RaceInfo.nameJyo,
                                racenum = RaceInfo.racenum,
                                Bamei = jvSeRaceUma.Bamei.Trim(),
                                Umaban = jvSeRaceUma.Umaban,
                                raceId = RaceInfo.raceId,
                            });
                            break;
                        }
                    }
                }
                else
                {
                    _form1.axJVLink1.JVSkip();
                }
                cntLoop++;
            }
            while (cntLoop <= 1000000);
            _form1.prgJVRead.Maximum++;
            _form1.prgJVRead.Value =
                _form1.prgJVRead.Maximum;
            _form1.prgJVRead.Maximum--;

            int retJVClose = _form1.axJVLink1.JVClose();
            if (retJVClose != 0)
                return false;

            return true;
        }

        bool isExistDateList(List<string> listdbData, string DateTarg)
        {
            foreach (string dbData in listdbData)
            {
                if (dbData == DateTarg)
                {
                    return true;
                }
            }
            return false;
        }


        bool putJvLinkData()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + filenameDB))
                {
                    conn.Open();
                    using (SQLiteTransaction trans = conn.BeginTransaction())
                    {
                        SQLiteCommand cmd = conn.CreateCommand();
                        string strsql = "INSERT INTO " +
                                nameTableMain + " (" +
                                nameColumnMain1 + "," +
                                nameColumnMain2 + "," +
                                nameColumnMain3 + "," +
                                nameColumnMain4 + ") VALUES (" +
                                "@" + nameColumnMain1 + ", " +
                                "@" + nameColumnMain2 + ", " +
                                "@" + nameColumnMain3 + ", " +
                                "@" + nameColumnMain4 + ")";
                        cmd.CommandText = strsql;
                        cmd.Parameters.Add(nameColumnMain1, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnMain2, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnMain3, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnMain4, System.Data.DbType.String);
                        foreach (clsDbInfo RaceInfo in listRaceInfo)
                        {

                            cmd.Parameters[nameColumnMain1].Value = RaceInfo.strdate;
                            cmd.Parameters[nameColumnMain2].Value = RaceInfo.nameJyo;
                            cmd.Parameters[nameColumnMain3].Value = RaceInfo.racenum;
                            cmd.Parameters[nameColumnMain4].Value = RaceInfo.raceId;
                            cmd.ExecuteNonQuery();
                        }

                        strsql = "INSERT INTO " +
                                nameTableSub + " (" +
                                nameColumnSub1 + "," +
                                nameColumnSub2 + "," +
                                nameColumnSub3 + ") VALUES (" +
                                "@" + nameColumnSub1 + ", " +
                                "@" + nameColumnSub2 + ", " +
                                "@" + nameColumnSub3 + ")";
                        cmd.CommandText = strsql;
                        cmd.Parameters.Add(nameColumnSub1, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnSub2, System.Data.DbType.String);
                        cmd.Parameters.Add(nameColumnSub3, System.Data.DbType.String);
                        foreach (clsDbInfo dbInfo in listdbInfo)
                        {

                            cmd.Parameters[nameColumnSub1].Value = dbInfo.raceId;
                            cmd.Parameters[nameColumnSub2].Value = dbInfo.Umaban;
                            cmd.Parameters[nameColumnSub3].Value = dbInfo.Bamei;
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                return false;
            }

            return true;
        }

        public bool getDbDataDate(string strDateTarg)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + filenameDB))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " + "*" +
                    " FROM " + nameTableMain +
                    " WHERE " + nameColumnMain1 + "=" + strDateTarg;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    string tmpPlace = "";
                    while (reader.Read())
                    {
                        if (tmpPlace != reader[nameColumnMain2].ToString())
                        {
                            _form1.listBox1.Items.Add(reader[nameColumnMain2].ToString());
                        }
                        tmpPlace = reader[nameColumnMain2].ToString();
                    }
                }
                conn.Close();
            }

            if (_form1.listBox1.Items.Count > 0)
                return true;
            else
                return false;

        }

        public bool getDbDataPlace(string strDateTarg, string strPlace)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + filenameDB))
            {
                conn.Open();
                string codeJyo = cCommon.JyogyakuCord(
                    _form1.listBox1.SelectedItem.ToString().Substring(0, 2));
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " + "*" +
                    " FROM " + nameTableMain +
                    " WHERE " + nameColumnMain1 + " = " + strDateTarg +
                    " AND " + nameColumnMain2 + " = " +
                    "'" + strPlace + "'";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _form1.listBox2.Items.Add(reader[nameColumnMain3].ToString());
                    }
                }
                conn.Close();
            }
            if (_form1.listBox2.Items.Count > 0)
                return true;
            else
                return false;
        }

        public bool getDbDataBamei(string strDateTarg, string strPlace, string strRace)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + filenameDB))
            {
                conn.Open();
                string codeJyo = cCommon.JyogyakuCord(strPlace.ToString().Substring(0, 2));
                string raceId = strDateTarg + codeJyo + strRace;
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT " + "*" +
                    " FROM " + nameTableSub +
                    " WHERE " + nameColumnSub1 + " = " + raceId;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _form1.listBox3.Items.Add(reader[nameColumnSub2].ToString() +
                            " " + reader[nameColumnSub3].ToString());
                    }
                }
                conn.Close();
            }
            if (_form1.listBox3.Items.Count > 0)
                return true;
            else
                return false;

        }
    }


}
