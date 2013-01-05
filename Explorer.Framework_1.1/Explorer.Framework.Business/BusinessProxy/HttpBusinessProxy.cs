using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using System.Web;


namespace Explorer.Framework.Business.BusinessProxy
{
    public class HttpBusinessProxy : BaseBusinessProxy
    {
        internal HttpBusinessProxy(DataRow row)
            : base(row)
        { 
        
        }
        
        public override DataSet Process(DataSet dataSet)
        {
            BusinessAssembly ba = BusinessAssemblyFactory.Instance.GetBusinessAssembly(this.BusinessAssemblyName, this.Version);

            string content = "&xml=" + dataSet.GetXml();

            byte[] contentData = System.Text.Encoding.ASCII.GetBytes(content);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ba.Provider);
            //request.Referer = "http://localhost:2328/Default.aspx";
            request.Method = "POST";
            request.ContentLength = contentData.Length;
            //request.ContentType = "text/xml";
            request.ContentType = "application/x-www-form-urlencodfed";
            Stream inputStream = request.GetRequestStream();
            inputStream.Write(contentData, 0, contentData.Length);
            inputStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            Stream outputStream = response.GetResponseStream();

            DataSet result = new DataSet();
            result.ReadXml(outputStream);
            outputStream.Close();
            return result;
        }

       
        
    }
}
