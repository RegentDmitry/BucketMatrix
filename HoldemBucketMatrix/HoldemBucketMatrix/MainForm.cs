using HoldemBucketing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoldemBucketMatrix
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Data.BucketsLibrary.Init();
        }

        private DataTable GetDataTable(DetailedSituationList data, List<string> actions)
        {
            var dt = new DataTable();
            var activeDict = new Dictionary<int, string>();

            foreach (var a in actions)
                dt.Columns.Add(new DataColumn(a, typeof(string)));

            dt.Columns.Add("Confidence", typeof(string));
            dt.Columns.Add("Weight", typeof(string));
            dt.Columns.Add("Combos", typeof(double));

            for (int i = 0; i < Data.BucketsLibrary.Count(); i++)
            {
                if (data.Any(t => t.Values[i] != CustomBoolean.Ignore))
                {
                    var bucketName = Data.BucketsLibrary[i].Bucket;
                    activeDict.Add(i, bucketName);
                    dt.Columns.Add(new DataColumn(bucketName, typeof(string)));
                }
            }

            var totalWeigh = data.Sum(t => t.Actions.Sum(tt => tt.Value));

            foreach (var d in data)
            {
                var newrow = dt.NewRow();
                var bucketWeigth = d.Actions.Sum(t => t.Value);
                foreach (var a in actions)
                {
                    if (!d.Actions.ContainsKey(a))
                        newrow[a] = 0;
                    else
                        newrow[a] = Math.Round(d.Actions[a] / bucketWeigth * 100, 2).ToString() + "%";
                }

                for (int i = 0; i < d.Values.Count; i++)
                {
                    if (!activeDict.ContainsKey(i))
                        continue;

                    newrow[activeDict[i]] = d.Values[i].ToString();
                }

                newrow["Confidence"] = Math.Round(d.Confidence() * 100, 2).ToString() + "%";
                newrow["Weight"] = Math.Round(bucketWeigth * 100 / totalWeigh, 2).ToString() + "%";
                newrow["Combos"] = Math.Round(d.Actions.Values.Sum(), 2)/100;

                dt.Rows.Add(newrow);

            }

            return dt;
        }

        private void WriteDataTable2File(DataTable dt, string filename)
        {
            var strList = new List<string>();
            var colNames = new List<string>();
            foreach (DataColumn c in dt.Columns)
                colNames.Add(c.ColumnName);

            strList.Add(string.Join(",", colNames));

            foreach (DataRow row in dt.Rows)
            {
                var strRowList = new List<string>();
                for (int i = 0; i < colNames.Count; i++)
                    strRowList.Add(row[i].ToString());

                strList.Add(string.Join(",", strRowList));
            }

            File.WriteAllLines(filename, strList);
        }


        private async void btnStart_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() =>
            {
                var situation = new Situation();
                situation.Board = boardTextBox.Text;
                situation.Ranges = Utils.GetRangesFromCsv(inputTextBox.Text);
                var matrix = situation.FindBuckets();
                matrix.Wait();

                var actions = matrix.Result.Select(t => t.BestAction).Distinct().ToList();
                var dt = GetDataTable(matrix.Result, actions);

                WriteDataTable2File(dt, outputTextBox.Text);

                { }

            });
        }
    }
}
