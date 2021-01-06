using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows.Forms;

namespace GetUmatanOdds
{
    public class OperateForm
    {
        Form1 _form1;
        private clsDatabase cDatabase;
        clsCommon cCommon;

        public OperateForm(Form1 form1, clsDatabase cDatabase1, clsCommon cCommon1)
        {
            _form1 = form1;
            cDatabase = cDatabase1;
            cCommon = cCommon1;
        }
        public void enableButton()
        {
            _form1.button1.Enabled = true;
            _form1.btnGetJVData.Enabled = true;
        }

        public void disableButton()
        {
            _form1.button1.Enabled = false;
            _form1.btnGetJVData.Enabled = false;
        }

        public void readFolder()
        {
            CommonOpenFileDialog commonOpenFileDialog =
                new CommonOpenFileDialog("保存するフォルダを選択してください");
            commonOpenFileDialog.InitialDirectory =
                 AppDomain.CurrentDomain.BaseDirectory;
            commonOpenFileDialog.IsFolderPicker = true;
            if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;
            _form1.textBox1.Text = commonOpenFileDialog.FileName + "\\";
        }

        public void selectHistory(clsDbInfo cDbInfo)
        {
            // 日付の選択
            _form1.dateTimePicker1.Value = 
                DateTime.Parse(cDbInfo.strdate.Insert(4, "/").Insert(7, "/"));

            // 会場の取得
            if (!cDatabase.getDbDataDate(cDbInfo.strdate))
            {
            }
            // 会場の選択
            if (selectListBox1(cDbInfo.nameJyo))
            {
            }
            // レースの取得
            string strPlace = _form1.listBox1.SelectedItem.ToString();
            if (!cDatabase.getDbDataPlace(cDbInfo.strdate, strPlace))
            {
            }
            // レースの選択
            if (selectListBox2(cDbInfo.racenum))
            {
            }
            // 馬名の取得
            if (!cDatabase.getDbDataBamei(cDbInfo.strdate, strPlace, cDbInfo.racenum))
            {
            }
        }

        bool selectListBox1(string strPlaceRaceId)
        {
            int idx = 0;
            foreach(string item in _form1.listBox1.Items)
            {
                if (item.Contains(strPlaceRaceId))
                {
                    _form1.listBox1.SetSelected(idx, true);
                    return true;
                }
                idx++;
            }
            return false;
        }

        bool selectListBox2(string strRaceRaceId)
        {
            int idx = 0;
            foreach (string item in _form1.listBox2.Items)
            {
                if (item.Contains(strRaceRaceId))
                {
                    _form1.listBox2.SetSelected(idx, true);
                    return true;
                }
                idx++;
            }
            return false;
        }
    }
}
