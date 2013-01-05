using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Explorer.Framework.Core.Base;
using Explorer.Framework.Core;

namespace Explorer.Framework.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class BusinessAssemblyFactory : BaseFactory<BusinessAssemblyFactory, BusinessAssembly>
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            DataSet dataSet = AppContext.Instance.GetAppContextSet();

            DataTable table = dataSet.Tables["Bla"];
            BusinessAssembly businessAssembly = null;

            foreach (DataRow row in table.Rows)
            {
                businessAssembly = new BusinessAssembly(row);
                AddBusinessAssembly(businessAssembly.Name, businessAssembly.Version, businessAssembly);
            }

            base.Initialize();
        }

        public void LoadBusinessAssembly(string name, string version)
        { 
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public BusinessAssembly GetBusinessAssembly(string name, string version)
        {
            string key = name + "@" + version;
            return BusinessAssemblyManager.Instance.Items[key];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="businessAssembly"></param>
        public void AddBusinessAssembly(string name, string version, BusinessAssembly businessAssembly)
        {
            string key = name + "@" + version;
            if (BusinessAssemblyManager.Instance.Items.ContainsKey(key))
            {
                BusinessAssemblyManager.Instance.Items.Remove(key);
            }
            BusinessAssemblyManager.Instance.Items.Add(key, businessAssembly);

        }
      
    }
}
