using System;
using System.Collections.Generic;

namespace PaperPusher
{
    public class WindowManager : Caliburn.Micro.WindowManager
    {
        public override bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return base.ShowDialog(rootModel, context, settings);
        }
    }
}