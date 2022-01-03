using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
namespace SimulatorPlatform
{
    // Async serial 통신 관련 사항을 관리한다.
    class ASYNC : IF_Communicator
    {
        /*DLL=====================================================*/
        public delegate void delegateRecevieEventHandler(IntPtr _tmp, int _size);
        // Initialize
        [DllImport("AsyncComm-lib.dll")]
        static extern void initAsyncCommDll();
        // C#의 delegate를 등록한다.
        [DllImport("AsyncComm-lib.dll")]
        static extern void registerCallbackHandler(delegateRecevieEventHandler _receiveEventHandler);
        /*AsyncComm Setting---------------------------------------*/
        [DllImport("AsyncComm-lib.dll")]
        static extern bool openComm(int nPortNumber, int nBaudRate, int nBufferSize);
        [DllImport("AsyncComm-lib.dll")]
        static extern void closeComm();
        [DllImport("AsyncComm-lib.dll")]
        static extern int sendData(Byte[] data, ulong nLength);
        /*========================================================*/
        public ASYNC(String _commName)
        {
            commName = _commName;
            getCommInfoFromXML();
            cASYNC = this;
        }
        /*멤버변수=========================================*/
        private delegate Object delegateCallReceiveEventHandlers(Byte[] _data);
        private event delegateCallReceiveEventHandlers callReceiveEventHandlers;
        // 통신 노드 이름
        private String commName;
        // XML 파일 경로
        //private String commSettingPath = @"..\..\XML\CommSetting.xml";
        private String commSettingPath = @".\XML\CommSetting.xml";
        // Port
        private SByte portNum = 0;
        // Baudrate
        private Int32 baudrate = 0;
        // 수신 이벤트 처리를 위한 인스턴스
        private static ASYNC cASYNC;
        /*=================================================*/
        /*멤버함수 (상속)==================================*/
        // IF_Communicator
        public void createComm()
        {
            initAsyncCommDll();
            if (openComm((int)portNum, (int)baudrate, 1024))
            {
                //MainWindow.showText($"Success setComm = [{"ASYNC_MINS"}]");
                registerCallbackHandler(receiveEventHandler);
            }
            else
            {
                //MainWindow.showText($"Fail setComm = [{"ASYNC_MINS"}]");
            }
        }
        public void deleteComm()
        {
            closeComm();
        }
        public void sendData(Byte[] _data)
        {
            sendData(_data, (ulong)_data.Length);
        }
        public void receiveData(Byte[] _data)
        {
            callReceiveEventHandlers?.Invoke(_data);
        }
        public void registerReceiveEventHandlers(Func<Byte[], Object> _eventHandler)
        {
            callReceiveEventHandlers += new delegateCallReceiveEventHandlers(_eventHandler);
        }
        /*=================================================*/
        // parsing XML
        private void getCommInfoFromXML()
        {
            // Xml 설정값 입력
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(commSettingPath);
            XmlNode rootNode = xmlDoc.DocumentElement;
            XmlNode asyncNode = rootNode.SelectSingleNode("ASYNC");
            XmlNode commNode = asyncNode.SelectSingleNode(commName);
            portNum = Convert.ToSByte(commNode.SelectSingleNode("PORT").InnerText);
            baudrate = Convert.ToInt32(commNode.SelectSingleNode("BAUDRATE").InnerText);
        }
        // DLL에서 메시지 수신 시 해당 EventHandler로 수신 메시지가 전달된다.
        public static void receiveEventHandler(IntPtr _tmp, int _size)
        {
            byte[] data = new byte[_size];
            Marshal.Copy(_tmp, data, 0, _size);
            cASYNC.receiveData(data);
        }
    }
}