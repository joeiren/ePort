using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ILogger log = LoggerFactory.Instance.GetLogger("LoggerDB2", typeof(Form1));
            log.Debug("Test");
            try
            {
                DataSet dataSet = new DataSet();

                XRMEntity xrmEntity = EntityAdapterManager.Instance.CreateXRMEntityByFullName("");

            
                BusinessKey businessKey = BusinessKey.CreateKey("Test.Blh1", TransactionMode.Default);

                BusinessKey.SetKey(ref dataSet, businessKey);

                DataConverter.FillDataSet(xrmEntity, ref dataSet);

                dataSet = BusinessDelegate.Process(businessKey, dataSet);
                log.Info("处理完成");
            }
            catch (Exception ex)
            {
                log.Error("出错了", ex);
            }
            finally
            {
                log.Warn("结束");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string content = "userId=chedui01&userPassword=123456&loginType=1";

            byte[] contentData = System.Text.Encoding.ASCII.GetBytes(content);
            //string url = "http://login.4plmarket.com/sso/4plLogin/out";
            //string url = "http://6.1.8.191:8080/Login/sso/4plLogin/out";
            string url = "http://localhost:2328/Default.aspx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Referer = "http://localhost:2328/Default.aspx";
            //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Method = "POST";
            request.ContentLength = contentData.Length;
            //request.ContentType = "text/xml";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream stream = request.GetRequestStream();
            stream.Write(contentData, 0, contentData.Length);
            stream.Close();


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream1 = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream1);
            string text = reader.ReadToEnd();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IDataAccess da = DataAccessFactory.Instance.GetDataAccess();

            IDbDataAdapter adapter = da.CreateDataAdapter();

            da = DataAccessFactory.Instance.GetSlotDataAccess();

            Entity.Gps.TxServer.OrderAgent entity = new Entity.Gps.TxServer.OrderAgent();
            
            EntityAdapter entityAdapter = EntityAdapterManager.Instance.GetAdapterByClassType(typeof(Entity.Gps.TxServer.OrderAgent));

            EntityAccess<Entity.Gps.TxServer.OrderAgent> ea = da.GetEntityAccess<Entity.Gps.TxServer.OrderAgent>(entityAdapter);


            da.Close();

            //entity.OrderAgentId = 7;
            //ea.Load(ref entity);

            //entity.OrderAgentName = "OrderAgentName179";
            //entity.Remark = "Rmark179";

            //int count = ea.Save(entity);

            //count = ea.Exists(entity);

            //count = ea.Update(entity);

            ///// 测试 Update 方法
            ////count = ea.SaveOrUpdate(entity);


            //count = ea.Delete(entity);

            //count = ea.Exists(entity);

            string wherestr = "orderAgentId=7";
            string orderstr = "orderAgentId asc";

            List<Entity.Gps.TxServer.OrderAgent> list = ea.QueryList(wherestr, orderstr);

            list = ea.QueryListByPage(wherestr, orderstr, 0, 1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IDataAccess da = DataAccessFactory.Instance.GetDataAccess("Odac");


            EntityAdapter entityAdapter = EntityAdapterManager.Instance.CreateAdapter(typeof(ORMEntity));


            EntityAccess<ORMEntity> ea = da.GetEntityAccess<ORMEntity>(entityAdapter);

            List<ORMEntity> list = ea.QueryListByPage("", "", 1, 100);

            

            DataSet dataSet = da.QueryDataSet("SELECT * FROM VEHICLE_BASICS");

            this.dataGridView1.DataSource = dataSet.Tables[0];

            ///// 测试 Load 方法
            //entity.Items.Add("OrderAgentId", 7);
            //ea.Load(ref entity);

            ///// 测试 Save 方法
            //entity.Items.Remove("OrderAgentId");
            //entity.Items.Remove("Remark");
            //entity.Items.Add("Remark", "保存测试");
            ////entity.Items.Update("Remark", "保存测试");
            //ea.Save(entity);

            //ea.Update(entity);

            ///// 测试 Update 方法
            //ea.SaveOrUpdate(entity);


            //ea.Delete(entity);

            //ea.Exists(entity);

            //string wherestr = "where address='北仑'";//
            //string orderstr = "order by orderAgentId asc";

            //List<XRMEntity> list = null;
            //list = ea.QueryList(wherestr, orderstr);

            //list = ea.QueryListByPage(wherestr, orderstr, 2, 5);
        }
    }
}
