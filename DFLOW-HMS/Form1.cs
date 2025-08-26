using atcData;
using atcTimeseriesRDB;
using DFLOWAnalysis;

namespace DFLOW_HMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DFLOWCalcs dFLOWCalcs = new DFLOWCalcs();
            frmDFLOWArgs frmDFLOWArgs = new frmDFLOWArgs();
            frmDFLOWArgs.ShowDialog();



        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmBatchMap frmBatchMap = new frmBatchMap();
            frmBatchMap.Initiate(null);
            frmBatchMap.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmDFLOWResults frmDFLOWResults = new frmDFLOWResults();
            frmDFLOWResults.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            atcTimeseriesRDB.atcTimeseriesRDB rdb = new atcTimeseriesRDB.atcTimeseriesRDB();
            //atcTimeseries ts = new atcTimeseries();
            atcDataAttributes dataAtt = new atcDataAttributes();

            //string file = @"E:\BASINSDS\DFLOW-HMS\02226000.rdb";
            string file = @"E:\BASINSDS\DFLOW-HMS\NWIS_discharge_TestData.rdb";
            bool bOpen = rdb.Open(file);
            
            atcTimeseries ts = rdb.DataSets[0];
            

            DFLOWCalcs.xQy(7, 10, ts);
        }
    }
}