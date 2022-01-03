using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace SimulatorPlatform
{
    public partial class subForm2 : Form, IF_DataHandler
    {
        public subForm2()
        {
            InitializeComponent();
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
        /*멤버함수 (상속)==================================*/
        // IF_DataHandler
        public void registerPublishEvent(Func<String, Object, Object> _eventHandler)
        {
            callPublishDataEvent += new delegatePublishData(_eventHandler);
        }
        // Colleague로부터 publish된 데이터를 처리한다.
        public void handleData(String _order, Object _data)
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
                if (destination.Contains("VIEW") == true)
                {
                    Console.WriteLine("[ERROR] :: Controller_Communicator :: handleData, order = {0}", order);
                }
            }
        }
        public bool checkSourceAndDestinationValid(String _source, String _destination)
        {
            if (_source == "MODEL" && _destination == "VIEW")
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

            dDataHandler.TryAdd("CONNECT", new delegateDataHandler(handler_connectComm));
            dDataHandler.TryAdd("DISCONNECT", new delegateDataHandler(handler_disconnectComm));
        }
        // 통신을 개통한다.
        private void handler_connectComm(Object _data)
        {
        }
        // 통신을 종료한다.
        private void handler_disconnectComm(Object _data)
        {
        }
        /*=================================================*/
        private void subForm2_Load(object sender, EventArgs e)
        {
        }
    }
}