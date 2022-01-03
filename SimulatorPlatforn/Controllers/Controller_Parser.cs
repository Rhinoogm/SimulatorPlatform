using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace SimulatorPlatform
{
    // Parser를 제어한다.
    class Controller_Parser : IF_Controller
    {
        public Controller_Parser()
        {
            registerEventToDelegate();
        }
        /*멤버변수=========================================*/
        // Mediator
        public delegate Object delegatePublishData(String _order, Object _data);
        public event delegatePublishData callPublishDataEvent;
        // Data Handler
        private ConcurrentDictionary<String, delegateDataHandler> dDataHandler;
        private delegate void delegateDataHandler(Object _data);
        /*=================================================*/
        /*멤버함수(상속)===================================*/
        // IF_DataHandler
        // Colleague로부터 publish된 데이터를 처리한다.
        public override void handleData(String _order, Object _data)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String[] orderArray = _order.Split(':');
            String sourceName = orderArray[0];
            String destination = orderArray[1];
            String order = orderArray[2];
            if (dDataHandler.ContainsKey(order) == true)
            {
                dDataHandler[order]?.DynamicInvoke(_data);
            }
            else
            {
                if (destination.Contains("CONTROLLER") == true)
                {
                    Console.WriteLine("[ERROR] :: Controller_Parser :: handleData, order = {0}", order);
                }
            }
        }
        public override void registerPublishEvent(Func<String, Object, Object> _eventHandler)
        {
            callPublishDataEvent += new delegatePublishData(_eventHandler);
        }
        public override bool checkSourceAndDestinationValid(String _source, String _destination)
        {
            if (_source == "VIEW" && _destination == "CONTROLLER")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /*=================================================*/

        /*멤버함수=========================================*/
        private void registerEventToDelegate()
        {
            if (dDataHandler == null)
                dDataHandler = new ConcurrentDictionary<String, delegateDataHandler>();
            dDataHandler.TryAdd("CREATE_COMM", new delegateDataHandler(handler_createComm));
            dDataHandler.TryAdd("DELETE_COMM", new delegateDataHandler(handler_deleteComm));
        }
        private void handler_createComm(Object _data)
        {
        }
        private void handler_deleteComm(Object _data)
        {
        }
        /*=================================================*/
    }
}