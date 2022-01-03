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
using System.Runtime.InteropServices;
using System.Threading;
namespace SimulatorPlatform
{
    public partial class Form1 : Form, IF_DataHandler
    {
        /**- Lcu 상태 설정 -**/
        //[DllImport("WrapperLcuSim.dll")]
        //static extern void setLcuMode(ushort _data);

        /*멤버변수=========================================*/
        private CreateInstances cCreateInstances;
        // Mediator
        public delegate Object delegatePublishData(String _order, Object _data);
        public event delegatePublishData callPublishDataEvent;
        // Data Handler
        private ConcurrentDictionary<String, delegateDataHandler> dDataHandler;
        private delegate void delegateDataHandler(Object _data);
        private Factory_subForm cFactory_subForm;
        public void setFactory_subForm(Factory_subForm _instance)
        {
            cFactory_subForm = _instance;
        }
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
        public Form1()
        {
            InitializeComponent();
            registerEventToDelegate();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            cCreateInstances = new CreateInstances(this);
            cCreateInstances.commType = "UDP_01";
            cCreateInstances.createInstances();
            initForm();
        }
        private void button_CONNECT_Click(object sender, EventArgs e)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String order = "VIEW:CONTROLLER:CONNECT";
            Object data = 0;
            callPublishDataEvent?.Invoke(order, data);
        }
        private void button_DISCONNECT_Click(object sender, EventArgs e)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String order = "VIEW:CONTROLLER:DISCONNECT";
            Object data = 0;
            callPublishDataEvent?.Invoke(order, data);
        }
        private double checkMinus(double _data)
        {
            long tmp = (long)_data;
            tmp = tmp >> 4;
            if ((tmp & 0x00080000) == 0x00080000)
            {
                tmp = ~tmp & (0x000FFFFF);
                tmp = (tmp + 0x0001) & (0x000FFFFF);
                tmp = ~tmp + 0x0001;
            }
            else
            {
                return tmp;
            }
            return tmp;
        }
        private void sendMsg(ushort _msgId)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String order = "VIEW:CONTROLLER:SEND";
            Object data = 0;
            data = _msgId;
            callPublishDataEvent?.Invoke(order, data);
        }

        private void initForm()
        {
            cForm1 = this;
        }
    
        /*-- 전역 로그 호출 --*/
        private static object lockObject = new object();
        public static Form1 cForm1;
        public static void showText(String _text)
        {
            //safetyShowTextBox(cForm1.textBox_LOG, (String)_text + "\r\n");

            //Thread thread1 = new Thread(
            //   (Object _obj) =>
            //   {
            //       Monitor.Enter(lockObject);
            //       try
            //       {
            //safetyShowTextBox(cForm1.textBox_LOG, (String)_obj + "\r\n");
            //       }
            //       finally
            //       {
            //           Monitor.Exit(lockObject);
            //       }
            //   });
            //thread1.Start(_text);
        }
        public static void showMsgId(String _text)
        {
            Thread thread1 = new Thread(
               (Object _obj) =>
               {
                   //safetyShowTextBox(cForm1.textBox_LOG_MSG, (String)_obj + "\r\n");
               });
            thread1.Start(_text);
        }
        private delegate void _SafetyTextBox(TextBox _textBox, Object _obj);
        public static void safetyShowTextBox(TextBox _textBox, Object _obj)
        {
            String msg = (String)_obj;
            if (_textBox.InvokeRequired)
                _textBox.Invoke(new _SafetyTextBox(safetyShowTextBox), _textBox, msg);
            else
            {
                _textBox.AppendText(msg);
            }
        }
    }
}