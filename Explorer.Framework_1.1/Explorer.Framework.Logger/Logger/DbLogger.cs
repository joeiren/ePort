using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Threading;

using Explorer.Framework.Data;
using Explorer.Framework.Data.DataAccess;

namespace Explorer.Framework.Logger
{
    /// <summary>
    /// 将日志存入数据库
    /// </summary>
    public class DbLogger : BaseLogger
    {
        private IDataAccess _da = null;
        private IDbCommand _dbCommand = null;
        private Dictionary<string, string> _params = null;
        
        public DbLogger(string name, DataRow rowLogger)
            : base(name, rowLogger)
        {
            if (rowLogger != null)
            {
                DataRow[] rows = rowLogger.GetChildRows("Logger_Property");
                foreach (DataRow rowProperty in rows)
                {
                    PropertyInfo propertyInfo = this.GetType().GetProperty(rowProperty["Name"].ToString());
                    if (propertyInfo != null)
                    {
                        object value;
                        if (rowProperty["Name"].ToString().ToLower().Equals("databasetype"))
                        {
                            value = Enum.Parse(propertyInfo.PropertyType, rowProperty["Property_Text"].ToString());
                        }
                        else
                        {
                            value = Convert.ChangeType(rowProperty["Property_Text"], propertyInfo.PropertyType);

                        }
                        propertyInfo.SetValue(this, value, null);
                    }
                }
            }

            this._da = GetDataAccess();

            this._params = new Dictionary<string, string>();

            this._dbCommand = _da.CreateCommand();
            this._dbCommand.CommandType = CommandType.Text;

            string tempText = this.Template;
            string[] fields = tempText.Split(new char[] { ',' });

            StringBuilder sbColumns = new StringBuilder();
            foreach (string field in fields)
            {
                string[] text = field.Split(new char[] { '=' });
                sbColumns.Append(",");
                sbColumns.Append(text[0]);
                IDataParameter dataParameter = this._dbCommand.CreateParameter();
                dataParameter.ParameterName = this._da.ParameterToken + text[0];
                this._dbCommand.Parameters.Add(dataParameter);
                this._params.Add(text[0], text[1]);
            }
            string sqlText = "INSERT INTO {0} ({1}) VALUES ({2})";

            string names = sbColumns.ToString();
            this._dbCommand.CommandText = string.Format(sqlText, this.Position, names.Substring(1), names.Replace(",", "," + this._da.ParameterToken).Substring(1));

            this._da.Close();            
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DataBaseType { get; set; }

        /// <summary>
        /// 数据库引用名
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 数据库链接串
        /// </summary>
        public string ConnectionString { get; set; }

        public override void LogWrite(string level, string message, Exception exception)
        {
            lock (LoggerFactory.Instance)
            {
                if (this.Level.ToUpper().IndexOf(level.ToUpper()) < 0)
                {
                    return;
                }

                Dictionary<string, string> fields = new Dictionary<string, string>();
                foreach (string fieldName in this._params.Keys)
                {
                    string fieldValue = this._params[fieldName];
                    fieldValue = fieldValue.Replace("'", "");

                    switch (fieldValue)
                    {
                        case "${Message}":
                            fieldValue = message;
                            break;
                        case "${Exception}":
                            fieldValue = message;
                            break;
                        case "${Level}":
                            fieldValue = level;
                            break;
                        default:
                            fieldValue = ConvertVar(fieldValue);
                            break;
                    }   
                    fields.Add(fieldName, fieldValue);
                }

                LogOutput(fields);
            }
        }

        protected override void LogOutput(object input)
        {
            Dictionary<string, string> p = input as Dictionary<string, string>;

            try
            {
                this._da.Open();

                this._dbCommand.Connection = this._da.DbConnection;
                //this._dbCommand.Prepare();

                foreach (IDataParameter dataParameter in this._dbCommand.Parameters)
                {
                    object value = p[dataParameter.ParameterName.Substring(1)];
                    dataParameter.Value = value == null ? DBNull.Value : value;
                }

                this._dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string text = ex.Message;
            }
            finally
            {
                try
                {
                    if (this._da != null)
                    {
                        this._da.Close();
                    }
                }
                catch (Exception ex) { }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private IDataAccess GetDataAccess()
        {
            IDataAccess da = null;
            if (!string.IsNullOrEmpty(this.DataSource))
            {
                da = DataAccessFactory.Instance.GetDataAccess(this.DataSource);
            }
            else
            {
                da = DataAccessFactory.Instance.CreateDataAccess(this.Name, this.DataBaseType, this.ConnectionString);
            }
            return da;
        }
    }
}

