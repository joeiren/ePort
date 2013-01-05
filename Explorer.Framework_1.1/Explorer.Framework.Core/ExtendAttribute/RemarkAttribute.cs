using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Core.ExtendAttribute
{
    /// <summary>
    /// Remark 属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class RemarkAttribute : Attribute
    {
        // Fields
        private string _author;
        private string _name;
        private string _description;
        private int _sortID;
                
        /// <summary>
        /// 
        /// </summary>
        public RemarkAttribute()
        {
            this._author = string.Empty;
            this._name = string.Empty;
            this._description = string.Empty;            
            this._sortID = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public RemarkAttribute(string description)
        {
            this._author = string.Empty;
            this._name = string.Empty;
            this._description = description;            
            this._sortID = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public RemarkAttribute(string name, string description)
        {
            this._author = string.Empty;
            this._name = name;
            this._description = description;
            this._sortID = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Author
        {
            get
            {
                return this._author;
            }
            set
            {
                this._author = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SortID
        {
            get
            {
                return this._sortID;
            }
            set
            {
                this._sortID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
    }
}
