using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL
{
    public class HalSerializerSettings : JsonSerializerSettings
    {
        public HalSerializerSettings()
        {
            ContractResolver = new HalContractResolver();
        }

        public new HalContractResolver ContractResolver {
            get
            {
                return base.ContractResolver as HalContractResolver;
            }
            set
            {
                base.ContractResolver = value;
            }
        }
    }
}
