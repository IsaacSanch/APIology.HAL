using System.Collections.Generic;

namespace Halcyon.HAL
{
    public interface IHalModelConfig
    {
        string RelativePathBase { get; set; }
        string RequestPathBase { get; set; }
        bool IsRoot { get; set; }
        string Expands { get; set; }
        Dictionary<string, dynamic> ExpandMap { get; set; }
    }
}