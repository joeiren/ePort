using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace Explorer.Framework.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceLoader : MarshalByRefObject
    {
        /// <summary>
        /// 装载本地对象
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public object CreateInstance(string filename, string className)
        {
            object obj = null;
            Assembly assembly = Assembly.LoadFile(filename);
            obj = assembly.CreateInstance(className, false);
            return obj;
        }
    }
}
