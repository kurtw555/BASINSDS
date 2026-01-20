using atcTimeseriesNdayHighLow;
using atcIDF;
using atcData;
using atcTimeseriesRDB;
using System.Diagnostics;

namespace USGSHydroToolbox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string file = @"D:\dotnet-stuff\DFLOW-Net\DFLOW-Net\NWIS_discharge_01117500.rdb";
            atcTimeseriesRDB.atcTimeseriesRDB rdbTS = new atcTimeseriesRDB.atcTimeseriesRDB();
            
            rdbTS.Open(file);
            //atcDataSet ds = new atcDataSet();
            //rdbTS.ReadData(ds);

            //atcData.atcDataSource dataSource = new atcData.atcDataSource();
            
            //dataSource.Open(file);
            atcTimeseriesNdayHighLow.atcTimeseriesNdayHighLow atcTSNDayHL = new atcTimeseriesNdayHighLow.atcTimeseriesNdayHighLow();
            atcDataAttributes ops = atcTSNDayHL.AvailableOperations;
            foreach (var op in ops) {
                Console.WriteLine(op);
                Debug.WriteLine(op);
            }
            atcTSNDayHL.AddDataSet(rdbTS.DataSets[0]);
            atcTSNDayHL.Open("7Q10");
            
            int i = 1;
            
        }
    }
}
