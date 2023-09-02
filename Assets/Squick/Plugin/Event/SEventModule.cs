using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Squick
{
	public class SEventModule : ISEventModule
    {
        public override void Awake() {}
        public override void Start() {}
        public override void AfterStart() {}
        public override void Update(long time) { }
        public override void BeforeDestroy() { }
        public override void Destroy() {  }

        private Dictionary<string, Dictionary<int, ISEvent>> mContainer;

        public SEventModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
            mContainer = new Dictionary<string, Dictionary<int, ISEvent>>();

        }


        public override void BindEvent(string key, int nEventID, ISEvent.EventHandler handler)
        {
            if(!mContainer.ContainsKey(key))
            {
                mContainer.Add(key, new Dictionary<int, ISEvent>());
            }

            var mhtEvent = mContainer[key];
            if (!mhtEvent.ContainsKey(nEventID))
            {
				mhtEvent.Add(nEventID, new SEvent(nEventID, new DataList()));
            }

			ISEvent identEvent = (ISEvent)mhtEvent[nEventID];
            identEvent.RegisterCallback(handler);
        }

        public override void DoEvent(string key, int nEventID)
        {
            if (!mContainer.ContainsKey(key))
            {
                UnityEngine.Debug.LogError("No this key " + key + " to do evnet");
            }

            var mhtEvent = mContainer[key];
            if (mhtEvent.ContainsKey(nEventID))
            {
				ISEvent identEvent = (ISEvent)mhtEvent[nEventID];
                identEvent.DoEvent();
            }
        }

        
    }
}