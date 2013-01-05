using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Explorer.Framework.Data.EntityAccess;

namespace Explorer.Framework.Data.XRM
{
    /// <summary>
    /// 反射XML映射(Xml Reflection Mapping)数据对象
    /// </summary>
    [Serializable]
    public class XRMEntity : MarshalByRefObject, IDisposable
    {
        private Dictionary<string, object> _items;

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EntityAdapter EntityAdapter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XRMEntity()
        {
            _items = new Dictionary<string, object>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> Items
        {
            get { return this._items; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.EntityAdapter = null;
            this.Items.Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DictionaryItems
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Update(this Dictionary<string, object> source, string key, object value)
        {
            if (source.ContainsKey(key))
            {
                source.Remove(key);
            }
            source.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(this Dictionary<string, object> source, string key)
        {
            if (source.ContainsKey(key))
            {
                return source[key];
            }
            return null;
        }
    }
}
