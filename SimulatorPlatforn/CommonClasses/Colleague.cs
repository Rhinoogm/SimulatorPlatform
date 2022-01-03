using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorPlatform
{
    public class Colleague
    {
        private Mediator cMediator;
        public void setMediator(Mediator _Mediator)
        {
            cMediator = _Mediator;
        }
        public object publishData(String _order, Object _data)
        {
            cMediator.publishData(_order, _data);
            return null;
        }
        public void getData(String _order, Object _data)
        {
            // _Data를 가지고 처리할 부분을 만든다.
            if (_data != null)
            {
                messageEventHandler?.Invoke(_order, _data);
            }
            else
            {
                Console.WriteLine("[Error] {0} :: Colleague ::getData :: _data =  null");
            }
        }
        public delegate void delegateColleagueMessageHandler(String _order, Object _data);
        public event delegateColleagueMessageHandler messageEventHandler;
    }
}