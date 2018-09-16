using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{   
    public class CallbackMultiplexer
    {
        public delegate void PluginUpdateHandler(int index);

        //private DeviceBinding.ValueChanged _mappingUpdate;
        private PluginUpdateHandler _mappingUpdate;
        private readonly int _index;
        private readonly List<long> _cache;

        //public CallbackMultiplexer(List<long> cache, int index, DeviceBinding.ValueChanged mappingUpdate)
        public CallbackMultiplexer(List<long> cache, int index, PluginUpdateHandler mappingUpdate)
        {
            _mappingUpdate = mappingUpdate;
            _index = index;
            _cache = cache;
        }

        public void Update(long value)
        {
            _cache[_index] = value;
            _mappingUpdate(_index);
        }

        ~CallbackMultiplexer()
        {
            _mappingUpdate = null;
        }
    }
}
