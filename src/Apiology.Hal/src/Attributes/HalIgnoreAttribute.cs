using System;

namespace Apiology.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class HalIgnoreAttribute : Attribute
    {
    }
}
