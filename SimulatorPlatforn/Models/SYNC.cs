using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    // Sync serial 통신 관련 사항을 관리한다.
    class SYNC : IF_Communicator
    {
        public SYNC(String _commName)
        {
        }
        /*멤버변수=========================================*/
        private delegate Object delegateCallReceiveEventHandlers(Byte[] _data);
        private event delegateCallReceiveEventHandlers callReceiveEventHandlers;
        /*=================================================*/
        /*멤버함수 (상속)==================================*/
        // IF_Communicator
        public void createComm()
        {
        }
        public void deleteComm()
        {
        }
        public void sendData(Byte[] _data)
        {
        }
        public void receiveData(Byte[] _data)
        {
        }
        public void registerReceiveEventHandlers(Func<Byte[], Object> _eventHandler)
        {
            callReceiveEventHandlers += new delegateCallReceiveEventHandlers(_eventHandler);
        }
        /*=================================================*/
    }
}