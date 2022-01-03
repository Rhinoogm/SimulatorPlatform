using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Forms;
namespace SimulatorPlatform
{
    public class Factory_subForm
    {
        private ConcurrentDictionary<String, Form> flyweigt_subForm = new ConcurrentDictionary<String, Form>();
        public Form getInstance(String _instanceName)
        {
            if (flyweigt_subForm.ContainsKey(_instanceName) == false)
            {
                //if (_instanceName.Contains("SUBFORM1") == true)
                //{
                //    flyweigt_subForm.TryAdd(_instanceName, new subForm_SET_DEV());
                //}
                if (_instanceName.Contains("SUBFORM2") == true)
                {
                    flyweigt_subForm.TryAdd(_instanceName, new subForm2());
                }
                else
                {
                    Console.WriteLine("[ERROR] Factory_Simulator :: getInstance. {}", _instanceName);
                }
            }
            else
            {
                return flyweigt_subForm[_instanceName];
            }

            return flyweigt_subForm[_instanceName];
        }

        public void deleteInstance(String _instanceName)
        {
            Form tmp;
            if (flyweigt_subForm.ContainsKey(_instanceName) == true)
            {
                flyweigt_subForm.TryRemove(_instanceName, out tmp);
            }
            else
            {
                Console.WriteLine("[ERROR] Factory_Simulator :: deleteInstance. {}", _instanceName);
            }
        }
    }
}