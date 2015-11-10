using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apiology.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class HalEmbeddedValuesAttribute : Attribute
    {
        internal HalLink LinkTemplate { get; private set; }

        public HalEmbeddedValuesAttribute(string linkTemplate = null)
        {
            if (linkTemplate != null)
            {
                LinkTemplate = new HalLink(null, linkTemplate);
            }
        }
    }
}
