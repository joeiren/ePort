using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;


namespace Explorer.Framework.Common.Filler
{
    public class WinFormFiller
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public static Dictionary<string, Control> GetControls(Control control)
        {
            Dictionary<string, Control> dict = new Dictionary<string, Control>();
            SearchControl(control, ref dict);
            return dict;
        }

        #region 将 Form 里的数据填到指定位置
        /// <summary>
        /// 将 Form 里或是指定容器下的所有子控件名和值装入 Hashtable
        /// </summary>
        /// <param name="control"></param>
        /// <param name="table"></param>
        public static void FormFillHashtable(Control control, ref Hashtable table)
        {
            Dictionary<string, Control> dictControl = GetControls(control);
            foreach (string key in dictControl.Keys)
            {
                Control targetControl = (Control)dictControl[key];
                object value = GetControlValue(targetControl);
                if (value != null)
                {
                    table.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 将 Form 里或是指定容器下的所有子控件名和值装入 DataTable
        /// </summary>
        /// <param name="control"></param>
        /// <param name="table"></param>
        public static void FormFillDataTable(Control control, ref DataTable table)
        {
            Dictionary<string, Control> dictControl = GetControls(control);
            table.Columns.Clear();
            foreach (string key in dictControl.Keys)
            {
                table.Columns.Add(new DataColumn(key));
            }
            DataRow row = table.NewRow();
            foreach (string key in dictControl.Keys)
            {
                Control targetControl = (Control)dictControl[key];
                object value = GetControlValue(targetControl);
                if (value != null)
                {
                    row[key] = value;
                }
            }
            table.Rows.Add(row);
        }

        /// <summary>
        /// 将 Form 里或是指定容器下的所有子控件名和值装入 Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="t"></param>
        public static void FormFillEntity<T>(Control control, ref T t)
        {
            Dictionary<string, Control> dictControl = GetControls(control);
            foreach (string key in dictControl.Keys)
            {
                PropertyInfo propertyInfo = t.GetType().GetProperty(key);
                if (propertyInfo != null)
                {
                    Control targetControl = (Control)dictControl[key];
                    object value = GetControlValue(targetControl);
                    propertyInfo.SetValue(t, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                }
            }
        }
        #endregion

        #region 将指定位置的数据填到 Form 里
        /// <summary>
        /// 将 Entity 对象填回到指定容器下的所有同名控件里
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="control"></param>
        public static void EntityFillForm<T>(T t, Control control)
        {
            Dictionary<string, Control> dictControl = GetControls(control);
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (dictControl.ContainsKey(propertyInfo.Name))
                {
                    object value = propertyInfo.GetValue(t, null);
                    Control targetControl = (Control)dictControl[propertyInfo.Name];
                    if (value != null)
                    {
                        SetControlValue(targetControl, value);
                    }
                }
            }
        }

        /// <summary>
        /// 将 DataTable 对象里的指定 DataRow 里的数据填回到指定容器下的所有同名控件里
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowIndex"></param>
        /// <param name="control"></param>
        public static void DataTableFillForm(DataTable dataTable, int rowIndex, Control control)
        {
            DataTableFillForm(dataTable.Rows[rowIndex], control);
        }

        /// <summary>
        /// 将 DataRow 对象里的数据填到指定容器下的所有同名控件里
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="control"></param>
        public static void DataTableFillForm(DataRow dataRow, Control control)
        {
            Dictionary<string, Control> dictControl = GetControls(control);
            foreach (string name in dictControl.Keys)
            {
                object value = dataRow[name];
                if (value != null)
                {
                    SetControlValue(dictControl[name], value);
                }
            }
        }
        #endregion

        /// <summary>
        /// 查找容器下所有控件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="dict"></param>
        private static void SearchControl(Control control, ref Dictionary<string, Control> dict)
        {
            foreach (Control childControl in control.Controls)
            {
                PropertyInfo propertyInfo = childControl.GetType().GetProperty("BindingContainer");
                if (propertyInfo != null)
                {
                    object value = propertyInfo.GetValue(childControl, null);
                    if (value is Control && value != null && !(string.IsNullOrEmpty(((Control)value).Name)) && ((Control)value).Name.Length > 3 && !dict.ContainsKey(((Control)value).Name.Substring(3)))
                    {
                        dict.Add(((Control)value).Name.Substring(3), (Control)value);
                    }
                }
                if (childControl.Controls.Count > 0)
                {
                    SearchControl(childControl, ref dict);
                }
                else
                {
                    if (!string.IsNullOrEmpty(childControl.Name) && childControl.Name.Length > 3 && !dict.ContainsKey(childControl.Name.Substring(3)))
                    {
                        dict.Add(childControl.Name.Substring(3), childControl);
                    }
                }
            }
        }

        private static object GetControlValue(Control control)
        {
            PropertyInfo propertyInfo;
            object value = null;
            switch (control.GetType().FullName)
            {
                case "DevExpress.Web.ASPxEditors.ASPxLabel":
                case "DevExpress.Web.ASPxEditors.ASPxButton":
                case "DevExpress.Web.ASPxEditors.ASPxTextBox":
                case "DevExpress.Web.ASPxEditors.ASPxMemo":
                case "System.Web.UI.WebControls.Label":
                case "System.Web.UI.WebControls.Button":
                case "System.Web.UI.WebControls.TextBox":
                    propertyInfo = control.GetType().GetProperty("Text");
                    value = propertyInfo.GetValue(control, null);
                    break;
                case "DevExpress.Web.ASPxEditors.CheckBox":
                case "System.Web.UI.WebControls.CheckBox":
                    propertyInfo = control.GetType().GetProperty("Checked");
                    value = propertyInfo.GetValue(control, null);
                    break;
                default:
                    break;
            }
            return value;
        }

        private static void SetControlValue(Control control, object value)
        {
            PropertyInfo propertyInfo;
            switch (control.GetType().FullName)
            {
                case "DevExpress.Web.ASPxEditors.ASPxLabel":
                case "DevExpress.Web.ASPxEditors.ASPxButton":
                case "DevExpress.Web.ASPxEditors.ASPxTextBox":
                case "DevExpress.Web.ASPxEditors.ASPxMemo":
                case "System.Web.UI.WebControls.Label":
                case "System.Web.UI.WebControls.Button":
                case "System.Web.UI.WebControls.TextBox":
                    propertyInfo = control.GetType().GetProperty("Text");
                    propertyInfo.SetValue(control, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    break;
                case "DevExpress.Web.ASPxEditors.CheckBox":
                case "System.Web.UI.WebControls.CheckBox":
                    propertyInfo = control.GetType().GetProperty("Checked");
                    propertyInfo.SetValue(control, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                    break;
                default:
                    break;
            }
        }
    }
}
