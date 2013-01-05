using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;
using System.Collections;

using Explorer.Framework.Core.Base;
using Explorer.Framework.Core;


namespace Explorer.Framework.Business
{
    public class BusinessAssembly : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WorkPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }

        private AppDomain _appDomain;

        internal BusinessAssembly(DataRow row)
        {
            this.Name = row["Name"].ToString();
            this.Mode = row["Mode"].ToString();
            if (row.Table.Columns.Contains("Version"))
            {
                this.Version = row["Version"].ToString();
            }
            if (row.Table.Columns.Contains("Port"))
            {
                this.Port = row["Port"].ToString();
            }
            if (row.Table.Columns.Contains("Provider"))
            {
                this.Provider = row["Provider"].ToString();
            }
            //else 
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + this.Name))
            {
                this.Filename = AppDomain.CurrentDomain.BaseDirectory + this.Name;
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\" + this.Name))
            {
                this.Filename = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + this.Name;
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Bla\\" + this.Name))
            {
                this.Filename = AppDomain.CurrentDomain.BaseDirectory + "Bla\\" + this.Name;
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory + "Bla\\";
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\Bla\\" + this.Name))
            {
                this.Filename = AppDomain.CurrentDomain.BaseDirectory + "bin\\Bla\\" + this.Name;
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\Bla\\" + this.Name;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Bla\\" + this.Name + "\\" + this.Version + "\\" + this.Name))
            {
                this.Filename = AppDomain.CurrentDomain.BaseDirectory + "Bla\\" + this.Name + "\\" + this.Version + "\\" + this.Name;
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory + "Bla\\" + this.Name + "\\" + this.Version + "\\" + this.Name;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\Bla\\" + this.Name + "\\" + this.Version + "\\" + this.Name))
            {
                this.Filename = AppDomain.CurrentDomain.BaseDirectory + "bin\\Bla\\" + this.Name + "\\" + this.Version + "\\" + this.Name;
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory + "bin\\Bla\\" + this.Name + "\\" + this.Version + "\\" + this.Name;
            }

            if (!this.Mode.ToUpper().Equals("LOCAL"))
            {
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationName = this.Name;
                setup.ShadowCopyFiles = "false";
                setup.LoaderOptimization = LoaderOptimization.SingleDomain;
                setup.DisallowBindingRedirects = false;
                setup.DisallowCodeDownload = true;

                //setup.ShadowCopyDirectories = path;
                setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                setup.PrivateBinPath = this.WorkPath;
                setup.CachePath = setup.ApplicationBase;
                setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                this._appDomain = AppDomain.CreateDomain(this.Name + "@" + this.Version, null, setup);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public object CreateInstance(string className)
        {
            object obj = null;
            if (!string.IsNullOrEmpty(this.Filename) && File.Exists(this.Filename))
            {
                string name = Assembly.GetExecutingAssembly().GetName().FullName;
                if (!this.Mode.ToUpper().Equals("LOCAL"))
                {
                    BusinessAssemblyLoader loader = (BusinessAssemblyLoader)this._appDomain.CreateInstanceAndUnwrap(name, "Explorer.Framework.Business.BusinessAssemblyLoader");
                    obj = loader.CreateInstance(this.Filename, className);
                }
                else
                {
                    Assembly assembly = Assembly.LoadFile(this.Filename);
                    obj = assembly.CreateInstance(className);
                }
            }
            return obj;
        }

        private void RegisterRemoting()
        {
            if (!string.IsNullOrEmpty(this.Filename) && File.Exists(this.Filename))
            {
                string name = Assembly.GetExecutingAssembly().GetName().FullName;
                BusinessAssemblyLoader loader = (BusinessAssemblyLoader)this._appDomain.CreateInstanceAndUnwrap(name, "Explorer.Framework.Business.BusinessAssemblyLoader");
                
                //loader.RegisterRemoting(this.Filename, className, this.Name, this.Port, 
            }

        }
        #region IDisposable 成员

        public void Dispose()
        {
            AppDomain.Unload(this._appDomain);
            this._appDomain = null;
            System.GC.Collect();
        }

        #endregion
    }
}
