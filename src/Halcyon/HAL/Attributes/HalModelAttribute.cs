using System;

namespace Halcyon.HAL.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HalModelAttribute : Attribute
    {
        public string RelativePathBase { get; set; }

        public HalModelAttribute(string relativePathBase = "~/")
        {
            RelativePathBase = relativePathBase;
        }
    }
}
