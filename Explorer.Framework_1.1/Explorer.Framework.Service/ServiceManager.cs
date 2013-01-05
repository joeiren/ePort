using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Core.Base;
using System.Xml;

namespace Explorer.Framework.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceManager : BaseManager<ServiceManager, IServiceItem>, IServiceManager
    {
        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            Initialize(AppDomain.CurrentDomain.BaseDirectory + "AppContext.xml");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void Initialize(string filename)
        {
            this.Filename = filename;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            XmlNodeList nodeList = xmlDoc.SelectNodes("AppContext/AutoService/ServiceItem");

            foreach (XmlNode node in nodeList)
            {
                string serviceSpace = node.Attributes["ServiceSpace"].Value;
                string name = node.Attributes["Name"].Value;

                bool isAutoService = false;
                if (node.Attributes.GetNamedItem("IsAutoService") != null)
                {
                    isAutoService = bool.Parse(node.Attributes["IsAutoService"].Value);
                }

                string fileName = "";
                if (node.Attributes.GetNamedItem("FileName") != null)
                {
                    fileName = node.Attributes["FileName"].Value;
                }
                else if (node.Attributes.GetNamedItem("Filename") != null)
                {
                    fileName = node.Attributes["Filename"].Value;
                }

                string className = "";
                if (node.Attributes.GetNamedItem("ClassName") != null)
                {
                    fileName = node.Attributes["ClassName"].Value;
                }
                else if (node.Attributes.GetNamedItem("Classname") != null)
                {
                    fileName = node.Attributes["Classname"].Value;
                }

                XmlNodeList nodeParamList = node.SelectNodes("Parameter");
                Dictionary<string, string> paramMap = new Dictionary<string, string>();
                if (nodeParamList.Count > 0)
                {
                    foreach (XmlNode nodeParam in nodeParamList)
                    {
                        paramMap.Add(nodeParam.Attributes["Name"].Value, nodeParam.Value);
                    }
                }
                ServiceProxy serviceProxy = new ServiceProxy(serviceSpace, name, isAutoService, className, fileName);
                serviceProxy.Initialize(paramMap);
                AddServiceItem(serviceProxy);
            }

            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceSpace"></param>
        /// <param name="name"></param>
        public void LoadService(string serviceSpace, string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.Filename);
            XmlNode xnServiceItem = xmlDoc.SelectSingleNode("AppContext/AutoService/ServiceItem[@ServiceSpace='" + serviceSpace + "' and @Name='" + name + "']");

            bool isAutoService = false;
            if (xnServiceItem.Attributes.GetNamedItem("IsAutoService") != null)
            {
                isAutoService = bool.Parse(xnServiceItem.Attributes["IsAutoService"].Value);
            }

            string fileName = "";
            if (xnServiceItem.Attributes.GetNamedItem("FileName") != null)
            {
                fileName = xnServiceItem.Attributes["FileName"].Value;
            }
            else if (xnServiceItem.Attributes.GetNamedItem("Filename") != null)
            {
                fileName = xnServiceItem.Attributes["Filename"].Value;
            }

            string className = "";
            if (xnServiceItem.Attributes.GetNamedItem("ClassName") != null)
            {
                fileName = xnServiceItem.Attributes["ClassName"].Value;
            }
            else if (xnServiceItem.Attributes.GetNamedItem("Classname") != null)
            {
                fileName = xnServiceItem.Attributes["Classname"].Value;
            }

            XmlNodeList nodeParamList = xnServiceItem.SelectNodes("Parameter");
            Dictionary<string, string> paramMap = new Dictionary<string, string>();
            if (nodeParamList.Count > 0)
            {
                foreach (XmlNode nodeParam in nodeParamList)
                {
                    paramMap.Add(nodeParam.Attributes["Name"].Value, nodeParam.Value);
                }
            }

            ServiceProxy serviceProxy = new ServiceProxy(serviceSpace, name, isAutoService, className, fileName);
            serviceProxy.SetParameter(paramMap);
            AddServiceItem(serviceProxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceSpace"></param>
        /// <param name="name"></param>
        public void InitService(string serviceSpace, string name)
        {
            string fullName = serviceSpace + "@" + name;
            if (this.Items.ContainsKey(fullName))
            {
                ServiceProxy proxy = (ServiceProxy)this.Items[fullName];
                proxy.Initialize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceSpace"></param>
        /// <param name="name"></param>
        public void UnloadService(string serviceSpace, string name)
        {
            lock (this.Items)
            {
                string fullName = serviceSpace + "@" + name;
                if (this.Items.ContainsKey(fullName))
                {
                    ServiceProxy item = (ServiceProxy)this.Items[fullName];
                    item.Stop();
                    item.Dispose();
                    this.Items.Remove(fullName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void UnloadService(IServiceItem item)
        {
            UnloadService(item.ServiceSpace, item.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceItem"></param>
        public void AddServiceItem(IServiceItem serviceItem)
        {
            string fullName = serviceItem.ServiceSpace + "@" + serviceItem.Name;
            if (this.Items.ContainsKey(fullName))
            {
                ServiceProxy item = (ServiceProxy)this.Items[fullName];
                item.Stop();
                item.Dispose();
                this.Items.Remove(fullName);
            }
            this.Items.Add(fullName, serviceItem);
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginAutoService()
        {
            lock (this.Items)
            {
                foreach (IServiceItem item in this.Items.Values)
                {
                    if (item.IsAutoService && item.State == ServiceState.Ready)
                    {
                        item.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void StartService(IServiceItem item)
        {
            string fullName = item.ServiceSpace + "@" + item.Name;
            if (!this.Items.ContainsKey(fullName))
            {
                this.Items.Add(fullName, item);
            }
            item.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceSpace"></param>
        /// <param name="name"></param>
        public void StartService(string serviceSpace, string name)
        {
            string fullName = serviceSpace + "@" + name;
            StartService(fullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        public void StartService(string fullName)
        {
            if (this.Items.ContainsKey(fullName))
            {
                IServiceItem item = this.Items[fullName];
                item.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void StopService(IServiceItem item)
        {
            string fullName = item.ServiceSpace + "@" + item.Name;
            if (!this.Items.ContainsKey(fullName))
            {
                this.Items.Add(fullName, item);
            }
            item.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceSpace"></param>
        /// <param name="name"></param>
        public void StopService(string serviceSpace, string name)
        {
            string fullName = serviceSpace + "@" + name;
            StopService(fullName);
        }

        public void StopService(string fullName)
        {
            if (this.Items.ContainsKey(fullName))
            {
                IServiceItem item = this.Items[fullName];
                item.Stop();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartServiceAll()
        {
            lock (this.Items)
            {
                foreach (IServiceItem item in this.Items.Values)
                {
                    item.Start();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopServiceAll()
        {
            lock (this.Items)
            {
                foreach (IServiceItem item in this.Items.Values)
                {
                    item.Stop();
                }
            }
        }
    }
}
