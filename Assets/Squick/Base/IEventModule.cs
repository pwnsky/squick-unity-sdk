
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	public abstract class ISEventModule : IModule
	{

		public abstract void BindEvent(string key, int nEventID, ISEvent.EventHandler handler);
		public abstract void DoEvent(string key, int nEventID);
	}
}
