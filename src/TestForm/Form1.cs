using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestForm
{
    public partial class Form1 : Form
    {
        const string URL = "http://192.168.1.150:2900";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            BizSocketEventNet.Subscribe();
        }

        private void btnEnqueue_Click(object sender, EventArgs e)
        {
            BizSocketEventNet.EnqueuePriceChanged();
        }

        private void btnEnqueueSalesState_Click(object sender, EventArgs e)
        {
            BizSocketEventNet.EnqueuePublishSalesState();
        }
    }
}
