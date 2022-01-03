using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    // Simulator에게 필요한 메서드를 제공한다.
    abstract class IF_Simulator : IF_DataHandler
    {
        public abstract void registerParser(Parser _Parser);
        public abstract void registerCommunicator(IF_Communicator _Communicator);
        public abstract Object recvEventHadler(Byte[] _recvData);
        // IF_DataHandler
        public abstract void handleData(String _order, Object _data);
        public abstract void registerPublishEvent(Func<String, Object, Object> _event);
        public abstract bool checkSourceAndDestinationValid(String _source, String _destination);
    }
}