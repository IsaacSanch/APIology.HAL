namespace Halcyon.HAL
{
    public class HALModelConfig : IHALModelConfig
    {
        public string LinkBase { get; set; }
        public bool ForceHAL { get; internal set; }
    }
}