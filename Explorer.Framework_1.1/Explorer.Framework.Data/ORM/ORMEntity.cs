using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Data.ORM
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ORMEntity : MarshalByRefObject
    {
        private List<string> _changedPropertyies = new List<string>();
        internal bool _reportPropertyChangeSwitch = true;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PropertyChangeEventArg> PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        protected void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangeEventArg { Name = property });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        protected void ReportPropertyChanged(string property)
        {
            if (this._reportPropertyChangeSwitch)
            {
                if (!this._changedPropertyies.Contains(property))
                {
                    this._changedPropertyies.Add(property);
                }
                this.OnPropertyChanged(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetChangedProperties()
        {
            return this._changedPropertyies;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearChangedProperties()
        {
            this._changedPropertyies.Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PropertyChangeEventArg : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}
