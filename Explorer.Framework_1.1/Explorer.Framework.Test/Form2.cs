using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Explorer.Framework.Data.DataAccess;
using Explorer.Framework.Data.ORM;
using Explorer.Framework.Data.XRM;
using Explorer.Framework.Data;
using Explorer.Framework.Core;
using Explorer.Framework.Logger;
using Explorer.Framework.Business;
using Explorer.Framework.Data.EntityAccess;

using Entity.Gps;


namespace Explorer.Framework.Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet dataSet = new DataSet();
            BusinessKey businessKey = BusinessKey.CreateKey("Test.Blh1", TransactionMode.Default);
            businessKey.Version = "auto";
            BusinessKey.SetKey(ref dataSet, businessKey);

            dataSet = BusinessDelegate.Process(businessKey, dataSet);

        }
    }
}
