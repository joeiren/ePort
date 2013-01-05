using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;

using Explorer.Framework.Core;
using Explorer.Framework.Core.Base;
using Explorer.Framework.Core.ExtendAttribute;
using Explorer.Framework.Data.XRM;

namespace Explorer.Framework.Data.EntityAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityAdapterManager : BaseFactory<EntityAdapterManager, EntityAdapter>
    {
        private static object lockObject = new object();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, EntityAdapter> ItemsByFullName
        {
            get
            {
                if (this._itemsByFullName == null)
                {
                    lock (lockObject)
                    {
                        if (this._itemsByFullName == null)
                        {
                            this._itemsByFullName = new Dictionary<string, EntityAdapter>();
                        }
                    }
                }
                return this._itemsByFullName;
            }
            private set { this._itemsByFullName = value; }
        }
        Dictionary<string, EntityAdapter> _itemsByFullName;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, EntityAdapter> ItemsByTableName
        {
            get { return this._itemsByTableName; }
            private set { this._itemsByTableName = value; }
        }
        Dictionary<string, EntityAdapter> _itemsByTableName;

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            this.ItemsByTableName = new Dictionary<string, EntityAdapter>();

            DataSet dataSet = AppContext.Instance.GetAppContextSet();
            if (dataSet != null)
            {
                DataTable table = dataSet.Tables["Entity"];
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string path = row["Path"].ToString();
                        path = AppContext.ConvertEnvironmentVariable(path);
                        LoadAdapter(path);
                    }
                }
                else
                {
                    LoadAdapter(AppDomain.CurrentDomain.BaseDirectory + @"\Entity");
                }
            }
            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public void LoadAdapter(string filename)
        {
            string[] filenames = new string[] { filename };
            if (Directory.Exists(filename))
            {
                filenames = Directory.GetFiles(filename, "*.xml", SearchOption.AllDirectories);
            }
            foreach (string file in filenames)
            {
                if (File.Exists(file))
                {
                    EntityAdapter ea = CreateAdapter(new Uri(file));
                    this.AddAdapter(ea);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityAdapter"></param>
        public void AddAdapter(EntityAdapter entityAdapter)
        {
            lock (this.ItemsByFullName)
            {
                if (this.ItemsByFullName.ContainsKey(entityAdapter.FullName))
                {
                    this.ItemsByFullName.Remove(entityAdapter.FullName);
                }
                this.ItemsByFullName.Add(entityAdapter.FullName, entityAdapter);
            }
            lock (this.ItemsByTableName)
            {
                if (this.ItemsByTableName.ContainsKey(entityAdapter.TableName))
                {
                    this.ItemsByTableName.Remove(entityAdapter.TableName);
                }
                this.ItemsByTableName.Add(entityAdapter.TableName, entityAdapter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EntityAdapter GetAdapterByFullName(string name)
        {
            EntityAdapter entityAdapter = null;
            if (this.ItemsByFullName.ContainsKey(name))
            {
                entityAdapter = this.ItemsByFullName[name];
            }
            else
            {
                Type classType = Type.GetType(name);
                if (classType != null)
                {
                    entityAdapter = CreateAdapter(classType);
                    AddAdapter(entityAdapter);
                }
            }
            return entityAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EntityAdapter GetAdapterByTableName(string name)
        {
            EntityAdapter entityAdapter = null;
            if (this.ItemsByTableName.ContainsKey(name))
            {
                entityAdapter = this.ItemsByTableName[name];
            }
            return entityAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public EntityAdapter GetAdapterByClassType(Type classType)
        {
            EntityAdapter entityAdapter = null;
            if (this.ItemsByFullName.ContainsKey(classType.FullName))
            {
                entityAdapter = this.ItemsByFullName[classType.FullName];
            }
            else
            {
                entityAdapter = CreateAdapter(classType);
                AddAdapter(entityAdapter);
            }
            return entityAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public EntityAdapter CreateAdapter(Uri uri)
        {
            EntityAdapter entityAdapter = new EntityAdapter(uri);
            return entityAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public EntityAdapter CreateAdapter(Stream stream)
        {
            EntityAdapter entityAdapter = new EntityAdapter(stream);
            return entityAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public EntityAdapter CreateAdapter(Type classType)
        {
            EntityAdapter entityAdapter = new EntityAdapter(classType);
            return entityAdapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XRMEntity CreateXRMEntityByFullName(string name)
        {
            XRMEntity entity = new XRMEntity();
            entity.FullName = name;
            entity.EntityAdapter = this.ItemsByFullName[name];
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XRMEntity CreateXRMEntityByTableName(string name)
        {
            XRMEntity entity = new XRMEntity();
            entity.EntityAdapter = this.ItemsByTableName[name];
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.ItemsByTableName.Clear();
            this.ItemsByTableName = null;

            this.ItemsByFullName.Clear();
            this.ItemsByFullName = null;

            base.Dispose();
        }
    }
}
