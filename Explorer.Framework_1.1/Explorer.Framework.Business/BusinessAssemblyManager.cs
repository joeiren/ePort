using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Core.Base;

namespace Explorer.Framework.Business
{
    internal class BusinessAssemblyManager : BaseManager<BusinessAssemblyManager, BusinessAssembly>
    {

        public override void Dispose()
        {
            lock (BusinessAssemblyManager.Instance)
            {
                foreach (BusinessAssembly businessAssembly in BusinessAssemblyManager.Instance.Items.Values)
                {
                    businessAssembly.Dispose();
                }
                base.Dispose();
            }            
        }
    }
}
