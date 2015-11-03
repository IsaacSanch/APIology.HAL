using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Halcyon.HAL
{
    public class HalModelConfig : IHalModelConfig
    {
        public string RelativePathBase { get; set; }

        public string RequestPathBase { get; set; }

        public bool IsRoot { get; set; }

        public string Expands { get; set; }

        private Dictionary<string, dynamic> _map;
        public Dictionary<string, dynamic> ExpandMap
        {
            get
            {
                if (_map == null)
                {
                    _map = Expands?.Split(',')?.ToDictionary(i => i, i => null as dynamic);
                    if (_map == null)
                        return null;

                    while (_map.Any(i => i.Key.Contains('.')))
                    {
                        var item = _map.FirstOrDefault(i => i.Key.Contains('.'));
                        _map.Remove(item.Key);

                        var idx = item.Key.LastIndexOf('.');
                        string key = item.Key.Substring(0, idx);
                        string value = item.Key.Substring(idx + 1, item.Key.Length - idx - 1);

                        if (_map.Any(i => i.Key == key))
                        {
                            var target = _map.FirstOrDefault(i => i.Key == key);
                            (target.Value as Dictionary<string, dynamic>).Add(
                                value, item.Value as dynamic
                            );
                        }
                        else
                        {
                            var dict = new Dictionary<string, dynamic>();
                            dict.Add(value, item.Value);
                            _map.Add(key, dict);
                        }
                    }
                }

                return _map;
            }
            set
            {
                _map = value;
            }
        }

        public HalModelConfig(HalModelAttribute config)
        {
            RelativePathBase = config?.RelativePathBase ?? "~/";
        }
    }
}
