using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Squick
{
    public class PluginManager : IPluginManager
    {
        public PluginManager()
        {
        }
        //------------- 接口 -------------------//

        public override void Awake()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.Awake();
                }
            }
        }

        public override void Start()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.Start();
                }
            }
        }

        public override void AfterStart()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.AfterStart();
                }
            }
        }
        public override void Update(long time)
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
					plugin.Update(time);
                }
            }
        }

        public override void BeforeDestroy()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.BeforeDestroy();
                }
            }
        }

        public override void Destroy()
        {
            foreach (IPlugin plugin in mPlugins.Values)
            {
                if (plugin != null)
                {
                    plugin.Destroy();
                }
            }
        }


        public override T _FindModule<T>()
        {
            IModule module = _FindModule(typeof(T).ToString());

            return (T)module;
        }

        public override IModule _FindModule(string strModuleName)
        {
            IModule module;
            mModules.TryGetValue(strModuleName, out module);
            return module;
        }
        public override void Registered(IPlugin plugin)
        {
            mPlugins.Add(plugin.GetPluginName(), plugin);
            plugin.Install();
        }
        public override void UnRegistered(IPlugin plugin)
        {
            mPlugins.Remove(plugin.GetPluginName());
            plugin.Uninstall();
        }
        public override void AddModule(string strModuleName, IModule pModule)
        {
            mModules.Add(strModuleName, pModule);
        }
        public override void RemoveModule(string strModuleName)
        {
            mModules.Remove(strModuleName);
        }

		public override void _RemoveModule<T>()
        {
			RemoveModule(typeof(T).ToString());
        }
        protected Dictionary<string, IPlugin> mPlugins = new Dictionary<string, IPlugin>();
        protected Dictionary<string, IModule> mModules = new Dictionary<string, IModule>();
    };
}