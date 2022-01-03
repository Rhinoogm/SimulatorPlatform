using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace SimulatorPlatform
{
    class Factory_Simulator
    {
        private ConcurrentDictionary<String, IF_Simulator> flyweigt_Simulator = new ConcurrentDictionary<String, IF_Simulator>();
        public IF_Simulator getInstance(String _instanceName)
        {
            if (flyweigt_Simulator.ContainsKey(_instanceName) == false)
            {
                if (_instanceName.Contains("SIMULATOR1") == true)
                {
                    flyweigt_Simulator.TryAdd(_instanceName, new Simulator1());
                }
                else if (_instanceName.Contains("SIMULATOR2") == true)
                {
                    flyweigt_Simulator.TryAdd(_instanceName, new Simulator2());
                }
                else
                {
                    Console.WriteLine("[ERROR] Factory_Simulator :: getInstance. {}", _instanceName);
                }
            }
            else
            {
                return flyweigt_Simulator[_instanceName];
            }

            return flyweigt_Simulator[_instanceName];
        }

        public void deleteInstance(String _instanceName)
        {
            IF_Simulator tmp;
            if (flyweigt_Simulator.ContainsKey(_instanceName) == true)
            {
                flyweigt_Simulator.TryRemove(_instanceName, out tmp);
            }
            else
            {
                Console.WriteLine("[ERROR] Factory_Simulator :: deleteInstance. {}", _instanceName);
            }
        }
    }
}