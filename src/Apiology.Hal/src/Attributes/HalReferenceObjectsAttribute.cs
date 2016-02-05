using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apiology.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class HalReferenceObjectsAttribute : Attribute
    {
        internal HalLink LinkTemplate { get; private set; }
        public bool HideIfNotRoot { get; private set; }

        public HalReferenceObjectsAttribute(string linkTemplate = null, bool hideIfNotRoot = false)
        {
            HideIfNotRoot = hideIfNotRoot;

            if (linkTemplate != null)
            {
                LinkTemplate = new HalLink(null, linkTemplate, hideIfNotRoot: hideIfNotRoot);
            }
        }
    }
}
