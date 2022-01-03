using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    // 통신 관련 interface를 제공한다.
    interface IF_Communicator
    {
        void createComm();
        void deleteComm();
        void sendData(Byte[] _data);
        void receiveData(Byte[] _data);
        void registerReceiveEventHandlers(Func<Byte[], Object> _eventHandler);
    }
}