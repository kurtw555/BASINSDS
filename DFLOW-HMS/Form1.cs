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
    }
}