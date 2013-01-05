using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Core.Base;
using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Data.EntityAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityAccessFactory : BaseSingletonInstance<EntityAccessFactory>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        public EntityAccess<T> CreateEntityAccess<T>(IDataAccess dataAccess) where T : new()
        {
            Type classType = typeof(T);
            if (classType.FullName.Equals("Explorer.Framework.Data.XRM.XRMEntity"))
            {
                return new XRMEntityAccess<T>(dataAccess, null);
            }
            else
            {
                EntityAdapter entityAdapter = EntityAdapterManager.Instance.GetAdapterByClassType(classType);
                return new ORMEntityAccess<T>(dataAccess, entityAdapter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataAccess"></param>
        /// <param name="entityAdapter"></param>
        /// <returns></returns>
        public EntityAccess<T> CreateEntityAccess<T>(IDataAccess dataAccess, EntityAdapter entityAdapter) where T : new()
        {
            Type classType = typeof(T);
            if (classType.FullName.Equals("Explorer.Framework.Data.XRM.XRMEntity"))
            {
                return new XRMEntityAccess<T>(dataAccess, entityAdapter);
            }
            else
            {
                return new ORMEntityAccess<T>(dataAccess, entityAdapter);
            }       
        }
    }
}
