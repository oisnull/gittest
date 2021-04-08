using Operation.Factory.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Operation.Factory
{
    public class AssemblyProxyManager
    {
        public AppDomain CurrentAppDomain { get; private set; }
        private string _AppWorkDirectory;

        public AssemblyProxyManager(string appWorkDirectory = null)
        {
            this._AppWorkDirectory = appWorkDirectory;
            if (string.IsNullOrEmpty(this._AppWorkDirectory))
                this._AppWorkDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");

            this.Init();
        }

        private void Init()
        {
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationName = "OperationFactoryTemp";
            setup.ApplicationBase = this._AppWorkDirectory;
            setup.PrivateBinPath = this._AppWorkDirectory;
            //setup.ShadowCopyFiles = "true";
            //setup.ShadowCopyDirectories = this._AppWorkDirectory;
            //setup.CachePath = this._AppWorkDirectory;
            //setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            //Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
            //PermissionSet grantSet = new PermissionSet(PermissionState.Unrestricted);
            this.CurrentAppDomain = AppDomain.CreateDomain($"{setup.ApplicationName}_Domain_{Guid.NewGuid()}", null, setup);
        }

        public AssemblyManager CreateInstance(string assemblyFullPath, string classFullName)
        {
            Type type = typeof(AssemblyManager);
            AssemblyManager assembly = this.CurrentAppDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true, BindingFlags.Default, null, new object[] { assemblyFullPath, classFullName }, null, null) as AssemblyManager;
            return assembly;
        }

        public void UnloadInstance()
        {
            if (this.CurrentAppDomain != null)
            {
                AppDomain.Unload(this.CurrentAppDomain);
                this.CurrentAppDomain = null;
            }
        }
    }

    //public class AssemblyProxyManager
    //{
    //    public static AssemblyManager CreateInstance(string assemblyFullPath, string className)
    //    {
    //        //string basePath = Path.GetDirectoryName(assemblyFullPath);
    //        string currentBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
    //        AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
    //        setup.ApplicationName = "OperationFactoryTemp";
    //        //setup.ApplicationBase = basePath;
    //        //setup.PrivateBinPath = basePath;
    //        setup.ApplicationBase = currentBinPath;
    //        setup.PrivateBinPath = currentBinPath;
    //        //setup.ShadowCopyFiles = "true";
    //        //setup.ShadowCopyDirectories = currentBinPath;
    //        //setup.CachePath = currentBinPath;
    //        //setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

    //        //Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
    //        //PermissionSet grantSet = new PermissionSet(PermissionState.Unrestricted);
    //        AppDomain sandboxDomain = AppDomain.CreateDomain($"{setup.ApplicationName}_Domain_{Guid.NewGuid()}", null, setup);
    //        Type type = typeof(AssemblyManager);
    //        return sandboxDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true, BindingFlags.Default, null, new object[] { assemblyFullPath, className }, null, null) as AssemblyManager;
    //    }
    //}
}
