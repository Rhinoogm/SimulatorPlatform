using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace SimulatorPlatform
{
    class Factory_Controller
    {
        private ConcurrentDictionary<String, IF_Controller> flyweigt_Controller = new ConcurrentDictionary<String, IF_Controller>();
        public IF_Controller getInstance(String _instanceName)
        {
            if (flyweigt_Controller.ContainsKey(_instanceName) == false)
            {
                if (_instanceName.Contains("PARSER") == true)
                {
                    flyweigt_Controller.TryAdd(_instanceName, new Controller_Parser());
                }
                else if (_instanceName.Contains("SIMULATOR") == true)
                {
                    flyweigt_Controller.TryAdd(_instanceName, new Controller_Simulator());
                }
                else if (_instanceName.Contains("COMMUNICATOR") == true)
                {
                    flyweigt_Controller.TryAdd(_instanceName, new Controller_Communicator());
                }
                else
                {
                    Console.WriteLine("[ERROR] Factory_Controller :: getInstance. {}", _instanceName);
                }
            }
            else
            {
                return flyweigt_Controller[_instanceName];
            }

            return flyweigt_Controller[_instanceName];
        }

        public void deleteInstance(String _instanceName)
        {
            IF_Controller tmp;
            if (flyweigt_Controller.ContainsKey(_instanceName) == true)
            {
                flyweigt_Controller.TryRemove(_instanceName, out tmp);
            }
            else
            {
                Console.WriteLine("[ERROR] Factory_Controller :: deleteInstance. {}", _instanceName);
            }
        }
    }
}