using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq;

using Explorer.Framework.Core;
using Explorer.Framework.Core.Base;
using Explorer.Framework.Business.BusinessProxy;

namespace Explorer.Framework.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessProxyFactory : BaseFactory<BusinessProxyFactory, BaseBusinessProxy>
    {
        private Dictionary<string, SortedDictionary<string, BaseBusinessProxy>> _cacheProxy = new Dictionary<string, SortedDictionary<string, BaseBusinessProxy>>();

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            /// 初始化 Proxy
            DataSet contextSet = AppContext.Instance.GetAppContextSet();
            DataTable table = contextSet.Tables["Blh"];
            foreach (DataRow row in table.Rows)
            {
                string name = row["Name"].ToString();
                string mode = row.GetParentRow("Bla_Blh")["Mode"].ToString().ToUpper();
                string version = "";
                if (row.Table.Columns.Contains("Version"))
                {
                    version = row.GetParentRow("Bla_Blh")["Version"].ToString();
                }
                BaseBusinessProxy proxy = null;

                if (mode.StartsWith("LOCAL"))
                {
                    proxy = new LocalBusinessProxy(row);
                }
                else if (mode.StartsWith("REMOTE.CLIENT"))
                {
                    proxy = new RemoteClientBusinessProxy(row);
                }
                
                if (proxy != null)
                {
                    AddBusinessProxy(proxy);
                }
            }
            base.Initialize();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessProxy"></param>
        public void AddBusinessProxy(BaseBusinessProxy businessProxy)
        {
            if (this._cacheProxy.ContainsKey(businessProxy.Name))
            {
                SortedDictionary<string, BaseBusinessProxy> versionMap = this._cacheProxy[businessProxy.Name];
                string version = "Default";
                if (!string.IsNullOrEmpty(businessProxy.Version))
                {
                    version = businessProxy.Version;
                }
                if (versionMap.ContainsKey(version))
                {
                    versionMap.Remove(version);
                }
                versionMap.Add(version, businessProxy);
            }
            else
            {
                SortedDictionary<string, BaseBusinessProxy> versionMap = new SortedDictionary<string, BaseBusinessProxy>();
                versionMap.Add(businessProxy.Version, businessProxy);
                this._cacheProxy.Add(businessProxy.Name, versionMap);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public BaseBusinessProxy GetBusinessProxy(string name, string version)
        {
            BaseBusinessProxy proxy = null;
            if (this._cacheProxy.ContainsKey(name))
            {
                SortedDictionary<string, BaseBusinessProxy> versionMap = this._cacheProxy[name];
                if (string.IsNullOrEmpty(version))
                {
                    version = "auto";
                }
                if (version.ToLower().Equals("last") || version.ToLower().Equals("auto"))
                {
                    proxy = versionMap.Last().Value;
                }
                else if (versionMap.ContainsKey(version))
                {
                    proxy = versionMap[version];
                }
            }
            return proxy;
        }

    }
}
