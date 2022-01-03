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
    // Udp ethernet 통신 관련 사항을 관리한다.
    class UDP : IF_Communicator
    {
        private static UDP cUDP;
        /*-- DLL 함수 --*/
        public delegate void delegateSendEventHandler(IntPtr _tmp, int _size);
        public static delegateSendEventHandler MyDel = sendEventHandler;
        [DllImport("WrapperLcuSim.dll")]
        static extern void registerSendCallbackEventHandler(delegateSendEventHandler _callbackEventHandler);
        [DllImport("WrapperLcuSim.dll")]
        static extern void sendMsg(ushort _msgId);
        [DllImport("WrapperLcuSim.dll")]
        static extern void recvMsg(Byte[] _buff);

        public UDP(String _commName)
        {
            commName = _commName;
            udpState = new stUDPSTATE();
            cUDP = this;
        }
        /*멤버변수=========================================*/
        private delegate Object delegateCallReceiveEventHandlers(Byte[] _data);
        private event delegateCallReceiveEventHandlers callReceiveEventHandlers;
        // 통신 노드 이름
        private String commName;
        // XML 파일 경로
        //private String commSettingPath = @"..\..\XML\CommSetting.xml";
        private String commSettingPath = @".\XML\CommSetting.xml";
        // IP
        private String clientIP;
        private String serverIP;
        // PORT
        private Int16 clientPort;
        private Int16 serverPort;
        // UDP 구조체
        private struct stUDPSTATE
        {
            public UdpClient u;
            public IPEndPoint e;
        }
        private stUDPSTATE udpState;
        /*=================================================*/
        /*멤버함수 (상속)==================================*/
        // IF_Communicator
        public void createComm()
        {
            try
            {
                getCommInfoFromXML();
                UdpClient u = new UdpClient(serverPort);
                IPEndPoint e = new IPEndPoint(IPAddress.Any, clientPort);
                udpState = new stUDPSTATE();
                udpState.u = u;
                udpState.e = e;
                // 수신 콜백 메서드 등록
                udpState.u.BeginReceive(new AsyncCallback(callbackReceive), udpState);
                registerSendCallbackEventHandler(MyDel);

                Form1.showText("Success Creating UDP comm!");
                Form1.showText($"LCU IP : {serverIP}"); ;
                Form1.showText($"ECS IP : {clientIP}"); ;
            }
            catch (Exception _ex)
            {
                Form1.showText("Fail to Create UDP comm!");
                Form1.showText(_ex.ToString());
            }
        }

        // parsing XML
        private void getCommInfoFromXML()
        {
            // Xml 설정값 입력
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(commSettingPath);
            XmlNode rootNode = xmlDoc.DocumentElement;
            XmlNode udpNode = rootNode.SelectSingleNode("UDP");

            XmlNode commNode = udpNode.SelectSingleNode(commName);
            XmlNode ipNode = commNode.SelectSingleNode("IP");
            clientIP = ipNode.SelectSingleNode("CLIENT").InnerText;
            serverIP = ipNode.SelectSingleNode("SERVER").InnerText;
            XmlNode portNode = commNode.SelectSingleNode("PORT");
            clientPort = Convert.ToInt16(portNode.SelectSingleNode("CLIENT").InnerText);
            serverPort = Convert.ToInt16(portNode.SelectSingleNode("SERVER").InnerText);
        }
        public void deleteComm()
        {
            try
            {
                if (udpState.u != null)
                {
                    udpState.u.Close();
                    udpState.u = null;
                }
                Form1.showText("Success Closing UDP comm!");
            }
            catch (Exception _ex)
            {
                Form1.showText("Fail to Close UDP comm!");
                Form1.showText(_ex.ToString());
            }
        }
        public void sendData(Byte[] _data)
        {
            if (udpState.u != null)
            {
                udpState.u.Send(_data, _data.Length, clientIP, clientPort);
            }
            else
            {
                Console.WriteLine("[ERROR] Ethernet :: transmitData = stUdpState.u is null");
            }
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

        /*멤버함수 ========================================*/
        private static Byte[] rcvData;
        private void callbackReceive(IAsyncResult ar)
        {
            try
            {
                UdpClient u = ((stUDPSTATE)(ar.AsyncState)).u;
                IPEndPoint e = ((stUDPSTATE)(ar.AsyncState)).e;
                Byte[] receivedData = u.EndReceive(ar, ref e);
                rcvData = receivedData;
                // To Simulator
                receiveData(receivedData);
                // To DLL
                recvMsg(receivedData);
                GC.Collect();
                udpState.u.BeginReceive(new AsyncCallback(callbackReceive), udpState);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] UDP 통신 종료!! :: {0}", ex.ToString());
            }
        }
        /*=================================================*/
        // DLL에서 메시지 수신 시 해당 EventHandler로 수신 메시지가 전달된다.
        public static void sendEventHandler(IntPtr _tmp, int _size)
        {
            byte[] data = new byte[_size];
            Marshal.Copy(_tmp, data, 0, _size);
            cUDP.sendData(data);
        }
    }
}