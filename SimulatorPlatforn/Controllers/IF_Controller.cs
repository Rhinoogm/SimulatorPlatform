using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    abstract class IF_Controller : IF_DataHandler
    {
        // IF_DataHandler
        public abstract void handleData(String _order, Object _data);
        public abstract void registerPublishEvent(Func<String, Object, Object> _event);
        public abstract bool checkSourceAndDestinationValid(String _source, String _destination);
    }
}