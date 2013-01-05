using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Explorer.Framework.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceProxy : IServiceItem, IDisposable
    {
        private AppDomain _appDomain;
        private Dictionary<string, string> _params;

        /// <summary>
        /// 
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WorkPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceSpace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ServiceState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StateMessage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IServiceItem ServiceItem { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceSpace"></param>
        /// <param name="name"></param>
        /// <param name="isAutoService"></param>
        /// <param name="className"></param>
        /// <param name="filename"></param>
        public ServiceProxy(string serviceSpace, string name, bool isAutoService, string className, string filename)
        {
            string fullName = serviceSpace + "@" + name;

            this.ServiceSpace = serviceSpace;
            this.Name = name;
            this.IsAutoService = isAutoService;
            this.Filename = filename;
            this.ClassName = className;
            
            if (File.Exists(this.Filename))
            {
                this.WorkPath = Path.GetDirectoryName(this.Filename);
            }
            else
            {
                this.WorkPath = AppDomain.CurrentDomain.BaseDirectory + @"\ServiceItem\" + fullName;
            }
           
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = fullName;
            setup.ShadowCopyFiles = "false";
            setup.LoaderOptimization = LoaderOptimization.SingleDomain;
            setup.DisallowBindingRedirects = false;
            setup.DisallowCodeDownload = true;

            //setup.ShadowCopyDirectories = path;
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.PrivateBinPath = this.WorkPath;
            setup.CachePath = setup.ApplicationBase;
            setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            this._appDomain = AppDomain.CreateDomain(fullName, null, setup);

            this.State = ServiceState.Install;
            this.StateMessage = "服务安装完成";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        public void Initialize(Dictionary<string, string> param)
        {
            this._params = param;
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            if (!string.IsNullOrEmpty(this.Filename) && File.Exists(this.Filename))
            {
                try
                {
                    string name = Assembly.GetExecutingAssembly().GetName().FullName;
                    ServiceLoader loader = (ServiceLoader)this._appDomain.CreateInstanceAndUnwrap(name, "Explorer.Framework.Service.ServiceLoader");
                    this.ServiceItem = (IServiceItem)loader.CreateInstance(this.Filename, this.ClassName);

                    this.ServiceItem.ServiceSpace = this.ServiceSpace;
                    this.ServiceItem.Name = this.Name;
                    this.ServiceItem.IsAutoService = this.IsAutoService;
                    this.ServiceItem.Initialize(this._params);
                    this.ServiceItem.State = ServiceState.Ready;
                    this.State = ServiceState.Ready;
                    this.StateMessage = "服务初始化完成!";
                }
                catch (Exception ex)
                {
                    this.State = ServiceState.Error;
                    this.StateMessage = ex.Message;
                }
            }
            else
            {
                this.State = ServiceState.Error;
                this.StateMessage = "服务初始化时缺少关键文件" + this.Filename;
            }
        }

        /// <summary>
        /// 重设参数
        /// </summary>
        /// <param name="param"></param>
        public void SetParameter(Dictionary<string, string> param)
        {
            this._params = param;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            try
            {
                this.ServiceItem.Start();
                this.ServiceItem.State = ServiceState.Run;
                this.State = ServiceState.Run;
                this.StateMessage = "服务正在运行!";
            }
            catch (Exception ex)
            {
                this.State = ServiceState.Error;
                this.StateMessage = "服务启动时错误:" + ex.Message;                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            try
            {
                this.ServiceItem.Pause();
                this.ServiceItem.State = ServiceState.Pause;
                this.State = ServiceState.Pause;
            }
            catch (Exception ex)
            {
                this.State = ServiceState.Error;
                this.StateMessage = "服务暂停时错误:" + ex.Message;                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            try
            {
                this.ServiceItem.Stop();
                this.ServiceItem.State = ServiceState.Stop;
                this.State = ServiceState.Stop;
            }
            catch (Exception ex)
            {
                this.State = ServiceState.Error;
                this.StateMessage = "服务停止时错误:" + ex.Message;  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this.ServiceItem != null)
            {
                if (this.ServiceItem is IDisposable)
                {
                    IDisposable disposable = this.ServiceItem as IDisposable;
                    disposable.Dispose();
                }
                this.ServiceItem = null;
            }
            if (this._params != null)
            {
                this._params.Clear();
            }
            if (this._appDomain != null)
            {
                AppDomain.Unload(this._appDomain);
                this._appDomain = null;
            }
            System.GC.Collect();
        }
    }
}
