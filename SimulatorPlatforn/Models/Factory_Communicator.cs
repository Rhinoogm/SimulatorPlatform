using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace SimulatorPlatform
{
    class Factory_Communicator
    {
        private ConcurrentDictionary<String, IF_Communicator> flyweigt_Communicator = new ConcurrentDictionary<String, IF_Communicator>();
        public IF_Communicator getInstance(String _instanceName)
        {
            if (flyweigt_Communicator.ContainsKey(_instanceName) == false)
            {
                if (_instanceName.Contains("UDP") == true)
                {
                    flyweigt_Communicator.TryAdd(_instanceName, new UDP(_instanceName));
                }
                else if (_instanceName.Contains("TCP") == true)
                {
                    flyweigt_Communicator.TryAdd(_instanceName, new TCP(_instanceName));
                }
                else if (_instanceName.Contains("ASYNC") == true) // SYNC보다 ASYNC가 앞에 나와야한다.
                {
                    flyweigt_Communicator.TryAdd(_instanceName, new ASYNC(_instanceName));
                }
                else if (_instanceName.Contains("SYNC") == true)
                {
                    flyweigt_Communicator.TryAdd(_instanceName, new SYNC(_instanceName));
                }
                else
                {
                    Console.WriteLine("[ERROR] Factory_Communicator :: getInstance. {}", _instanceName);
                }
            }
            else
            {
                return flyweigt_Communicator[_instanceName];
            }

            return flyweigt_Communicator[_instanceName];
        }

        public void deleteInstance(String _instanceName)
        {
            IF_Communicator tmp;
            if (flyweigt_Communicator.ContainsKey(_instanceName) == true)
            {
                flyweigt_Communicator.TryRemove(_instanceName, out tmp);
            }
            else
            {
                Console.WriteLine("[ERROR] Factory_Communicator :: deleteInstance. {}", _instanceName);
            }
        }
    }
}