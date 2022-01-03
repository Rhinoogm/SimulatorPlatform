using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    public interface IF_DataHandler
    {
        void handleData(String _order, Object _data);
        void registerPublishEvent(Func<String, Object, Object> _event);
        bool checkSourceAndDestinationValid(String _source, String _destination);
    }
}