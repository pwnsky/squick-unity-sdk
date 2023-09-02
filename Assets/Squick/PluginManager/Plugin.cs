using UnityEngine;
using System.Collections;

namespace Squick
{
    public class SquickPlugin : IPlugin
    {
        public SquickPlugin(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "SquickPlugin";
        }

        public override void Install()
        {
            AddModule<ISEventModule>(new SEventModule(mPluginManager));
            AddModule<NetModule>(new NetModule(mPluginManager));
        }
        public override void Uninstall()
        {
            mPluginManager.RemoveModule<NetModule>();
			mPluginManager.RemoveModule<ISEventModule>();
            mModules.Clear();
        }
    }
}
