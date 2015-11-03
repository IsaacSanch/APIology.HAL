using System;

namespace Halcyon.HAL.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class HalIgnoreAttribute : Attribute
    {
    }
}
