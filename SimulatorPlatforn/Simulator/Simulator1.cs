using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
namespace SimulatorPlatform
{
    class Simulator1 : IF_Simulator
    {
        [DllImport("WrapperLcuSim.dll")]
        static extern void sendMsg(ushort _msgId);
        public Simulator1()
        {
            registerEventToDelegate();
        }
        /*멤버변수=========================================*/
        // Model instances
        private Parser cParser;
        private IF_Communicator cCommunicator;
        // Mediator
        public delegate Object delegatePublishData(String _order, Object _data);
        public event delegatePublishData callPublishDataEvent;
        // Data Handler
        private ConcurrentDictionary<String, delegateDataHandler> dDataHandler;
        private delegate void delegateDataHandler(Object _data);
        /*=================================================*/
        /*멤버함수 (상속)==================================*/
        // IF_Simulator
        public override void registerParser(Parser _Parser)
        {
            cParser = _Parser;
        }
        public override void registerCommunicator(IF_Communicator _Communicator)
        {
            cCommunicator = _Communicator;
        }
        public override Object recvEventHadler(Byte[] _recvData)
        {
            ushort msgId = (ushort)(((_recvData[1] << 8) & 0xff00) | (_recvData[0] & 0x00ff));
            Form1.showMsgId($"0x{msgId.ToString("X4")}");

            if (msgId == 0xA033)
            {
                showMslSetInfo(_recvData);
            }
            else if (msgId == 0xA040)
            {
                showTargetSetInfo(_recvData);
            }
            else if (msgId == 0xA035)
            {
                flag_recvFLA033 = false;
                flag_recvFLA040 = false;
            }
            else if (msgId == 0xA016)
            {
                Form1.showText($"발사 장비 점검 요청 수신");
            }
            else if (msgId == 0xA018)
            {
                ushort command = (ushort)(((_recvData[13] << 8) & 0xff00) | (_recvData[12] & 0x00ff));
                if (command == 0x0010)
                {
                    Form1.showText($"발사대 전원차단 명령 수신");
                }
            }
            return null;
        }
        // IF_DataHandler
        // Colleague로부터 publish된 데이터를 처리한다.
        public override void handleData(String _order, Object _data)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String[] orderArray = _order.Split(':');
            String sourceName = orderArray[0];
            String destination = orderArray[1];
            String order = orderArray[2];
            if ((dDataHandler.ContainsKey(order) == true) && (checkSourceAndDestinationValid(sourceName, destination) == true))
            {
                dDataHandler[order]?.DynamicInvoke(_data);
            }
            else
            {
                if (destination.Contains("MODEL") == true)
                {
                    Console.WriteLine("[ERROR] :: Controller_Simulator1 :: handleData, order = {0}", order);
                }
            }
        }
        public override void registerPublishEvent(Func<String, Object, Object> _eventHandler)
        {
            callPublishDataEvent += new delegatePublishData(_eventHandler);
        }
        public override bool checkSourceAndDestinationValid(String _source, String _destination)
        {
            if (_source == "CONTROLLER" && _destination == "MODEL")
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
            dDataHandler.TryAdd("SEND", new delegateDataHandler(handler_sendMsg));
        }
        // 통신을 개통한다.
        private void handler_connectComm(Object _data)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String order = "MODEL:VIEW:TEST";
            Object data = 0;
            //callPublishDataEvent?.Invoke(order, data);
            cCommunicator.createComm();
        }
        // 통신을 종료한다.
        private void handler_disconnectComm(Object _data)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String order = "CONTROLLER:MODEL:DISCONNECT";
            Object data = null;
            //callPublishDataEvent?.Invoke(order, data);
            cCommunicator.deleteComm();
        }
        // 메시지를 송신한다.
        private void handler_sendMsg(Object _data)
        {
            // order = "SOURCE:DESTINATION:ORDER"
            String order = "CONTROLLER:MODEL:SEND";
            Object data = _data;
            //callPublishDataEvent?.Invoke(order, data);
            sendMsg((ushort)data);
        }
        private bool flag_recvFLA033 = false;
        private void showMslSetInfo(Byte[] _data)
        {
            if (flag_recvFLA033 == false)
            {
                flag_recvFLA033 = true;
                // 0. 수신 메시지 형변환
                ushort[] arrUshortTmp = ConvertMethods.BytearrayToUshortarray(_data);
                for (int i = 0; i < arrUshortTmp.Length; i++)
                {
                    ConvertMethods.swap2Bytes(ref arrUshortTmp[i]);
                }
                Form1.showText($"*/-- 유도탄설정정보 --/*");
                /*-- LG21 --*/
                // MFR Position
                //unsigned short mfrLatH;
                //unsigned short mfrLatL;
                double mfrLat = (UInt32)((arrUshortTmp[6] << 16) | (arrUshortTmp[7]));
                mfrLat = mfrLat * 90.0 / (2147483648 - 1);
                Form1.showText($"MFR 위도 : {mfrLat}");
                //unsigned short mfrLonH;
                //unsigned short mfrLonL;
                double mfrLon = (UInt32)((arrUshortTmp[8] << 16) | (arrUshortTmp[9]));
                mfrLon = mfrLon * 180.0 / (2147483648 - 1);
                Form1.showText($"MFR 경도 : {mfrLon}");
                //unsigned short mfrHgtH;
                //unsigned short mfrHgtL;
                double mfrHgt = (UInt32)((arrUshortTmp[10] << 16) | (arrUshortTmp[11]));
                mfrHgt = mfrHgt * 0.0078125;
                Form1.showText($"MFR 고도 : {mfrHgt}");
                //unsigned short mfrAzi;
                double mfrAzi = arrUshortTmp[12];
                mfrAzi = mfrAzi * 0.0055;
                Form1.showText($"방위각 : {mfrAzi}");
                //unsigned short mfrEle;
                double mfrEle = arrUshortTmp[13];
                mfrEle = mfrEle * 0.0014;
                Form1.showText($"고각 : {mfrEle}");
                //unsigned short reserved1[3]; // 14 15 16
                //unsigned short mfrAziLimitUB;
                double mfrAziLimitUB = arrUshortTmp[17];
                mfrAziLimitUB = mfrAziLimitUB * 0.0055;
                Form1.showText($"방위각UB : {mfrAziLimitUB}");
                //unsigned short mfrAziLimitLB;
                double mfrAziLimitLB = arrUshortTmp[18];
                mfrAziLimitLB = mfrAziLimitLB * 0.0055;
                Form1.showText($"방위각LB : {mfrAziLimitLB}");
                //unsigned short mfrEleLimitUB;
                double mfrEleLimitUB = arrUshortTmp[19];
                mfrEleLimitUB = mfrEleLimitUB * 0.0014;
                Form1.showText($"고각UB : {mfrEleLimitUB}");
                //unsigned short mfrEleLimitLB;
                double mfrEleLimitLB = arrUshortTmp[20];
                mfrEleLimitLB = mfrEleLimitLB * 0.0014;
                Form1.showText($"고각LB : {mfrEleLimitLB}");
                //unsigned short edCode;
                UInt16 edCode = arrUshortTmp[21];
                Form1.showText($"ED코드 : 0x{edCode.ToString("X4")}");
                //unsigned short mslAddr;
                UInt16 mslAddr = arrUshortTmp[22];
                Form1.showText($"Msl Addr : 0x{mslAddr.ToString("X4")}");
                //unsigned short settingWdl;
                UInt16 settingWdl = arrUshortTmp[23];
                Form1.showText($"WDL : 0x{settingWdl.ToString("X4")}");
                //unsigned short rfsFreqId;
                UInt16 rfsFreqId = arrUshortTmp[24];
                Form1.showText($"RFS Freq ID : 0x{rfsFreqId.ToString("X4")}");
                //unsigned short tarSpaceInfo;
                UInt16 tarSpaceInfo = arrUshortTmp[25];
                Form1.showText($"예상명중지점 : 0x{tarSpaceInfo.ToString("X4")}");
                //unsigned short forcedFiring;
                UInt16 forcedFiring = arrUshortTmp[26];
                Form1.showText($"강제사격 : 0x{forcedFiring.ToString("X4")}");
                //unsigned short reserved2; // 27
                //unsigned short year_Info; // 28
                //unsigned short month_day_Info; // 29
                //unsigned short hour_min_Info; // 30
                //unsigned short sencond_Info; // 31
                //unsigned short encodingKey[25]; // 32 ~ 56
                //unsigned short RFS_EON;
                UInt16 RFS_EON = arrUshortTmp[57];
                //unsigned short padding[2];
            }

        }
        private bool flag_recvFLA040 = false;
        private void showTargetSetInfo(Byte[] _data)
        {
            if (flag_recvFLA040 == false)
            {
                flag_recvFLA040 = true;
                // 0. 수신 메시지 형변환
                ushort[] arrUshortTmp = ConvertMethods.BytearrayToUshortarray(_data);
                for (int i = 0; i < arrUshortTmp.Length; i++)
                {
                    ConvertMethods.swap2Bytes(ref arrUshortTmp[i]);
                }

                Form1.showText($"*/-- 표적정보 --/*");
                //unsigned short targetInfo;       // 표적 종류
                UInt16 targetInfo = arrUshortTmp[6];
                Form1.showText($"표적 종류 : 0x{targetInfo.ToString("X4")}");
                //unsigned short tar_N_Pos_H;    // 표적 위치 N (MSW)
                //unsigned short tar_N_Pos_L;    // 표적 위치 N (LSW)
                double tar_N_Pos = (UInt32)((arrUshortTmp[7] << 16) | (arrUshortTmp[8]));
                Form1.showText($"표적위치X : {checkMinus(tar_N_Pos)}");
                //unsigned short tar_E_Pos_H;    // 표적 위치 E (MSW)
                //unsigned short tar_E_Pos_L;    // 표적 위치 E (LSW)
                double tar_E_Pos = (UInt32)((arrUshortTmp[9] << 16) | (arrUshortTmp[10]));
                Form1.showText($"표적위치Y : {checkMinus(tar_E_Pos)}");
                //unsigned short tar_D_Pos_H;    // 표적 위치 D (MSW)
                //unsigned short tar_D_Pos_L;    // 표적 위치 D (LSW)
                double tar_D_Pos = (UInt32)((arrUshortTmp[11] << 16) | (arrUshortTmp[12]));
                tar_D_Pos = checkMinus(tar_D_Pos) * 0.5;
                Form1.showText($"표적위치Z : {tar_D_Pos}");
                //unsigned short tar_N_Vel;      // 표적 속도 N
                double tar_N_Vel = arrUshortTmp[13];
                tar_N_Vel = checkMinus2(tar_N_Vel) * 4092 / (32768 - 1);
                Form1.showText($"표적속도X : {tar_N_Vel}");
                //unsigned short tar_E_Vel;      // 표적 속도 E
                double tar_E_Vel = arrUshortTmp[14];
                tar_E_Vel = checkMinus2(tar_E_Vel) * 4092 / (32768 - 1);
                Form1.showText($"표적속도Y : {tar_E_Vel}");
                //unsigned short tar_D_Vel;      // 표적 속도 D
                double tar_D_Vel = arrUshortTmp[15];
                tar_D_Vel = checkMinus2(tar_D_Vel) * 4092 / (32768 - 1);
                Form1.showText($"표적속도Z : {tar_D_Vel}");
                //unsigned short ip_N_Pos;       // 예상 명중점 위치 N
                double ip_N_Pos = arrUshortTmp[16];
                ip_N_Pos = checkMinus2(ip_N_Pos) * 8;
                Form1.showText($"예상 명중점 위치 X : {ip_N_Pos}");
                //unsigned short ip_E_Pos;       // 예상 명중점 위치 E
                double ip_E_Pos = arrUshortTmp[17];
                ip_N_Pos = checkMinus2(ip_E_Pos) * 8;
                Form1.showText($"예상 명중점 위치 Y : {ip_N_Pos}");
                //unsigned short ip_D_Pos;       // 예상 명중점 위치 D
                double ip_D_Pos = arrUshortTmp[18];
                ip_N_Pos = checkMinus2(ip_D_Pos) * 2;
                Form1.showText($"예상 명중점 위치 Z : {ip_N_Pos}");
                //unsigned short ip_TAR_N_Vel;   // 예상 명중지점의 표적 속도 N
                double ip_TAR_N_Vel = arrUshortTmp[19];
                ip_TAR_N_Vel = checkMinus2(ip_TAR_N_Vel) * 4092 / (32768 - 1);
                Form1.showText($"예상 명중지점의 표적 속도 X : {ip_TAR_N_Vel}");
                //unsigned short ip_TAR_E_Vel;   // 예상 명중지점의 표적 속도 E
                double ip_TAR_E_Vel = arrUshortTmp[20];
                ip_TAR_E_Vel = checkMinus2(ip_TAR_E_Vel) * 4092 / (32768 - 1);
                Form1.showText($"예상 명중지점의 표적 속도 Y : {ip_TAR_E_Vel}");
                //unsigned short ip_TAR_D_Vel;   // 예상 명중지점의 표적 속도 D
                double ip_TAR_D_Vel = arrUshortTmp[21];
                ip_TAR_D_Vel = checkMinus2(ip_TAR_D_Vel) * 4092 / (32768 - 1);
                Form1.showText($"예상 명중지점의 표적 속도 Z : {ip_TAR_D_Vel}");
                //unsigned short tar_N_Pos_STD;  // 표적 위치 표준편차 N
                double tar_N_Pos_STD = arrUshortTmp[22];
                tar_N_Pos_STD = checkMinus2(tar_N_Pos_STD) * 2000.0 / (65536 - 1);
                Form1.showText($"표적 위치 표준편차 X : {tar_N_Pos_STD}");
                //unsigned short tar_E_Pos_STD;  // 표적 위치 표준편차 E
                double tar_E_Pos_STD = arrUshortTmp[23];
                tar_E_Pos_STD = checkMinus2(tar_E_Pos_STD) * 2000.0 / (65536 - 1);
                Form1.showText($"표적 위치 표준편차 Y : {tar_E_Pos_STD}");
                //unsigned short tar_D_Pos_STD;  // 표적 위치 표준편차 D
                double tar_D_Pos_STD = arrUshortTmp[24];
                tar_D_Pos_STD = checkMinus2(tar_D_Pos_STD) * 2000.0 / (65536 - 1);
                Form1.showText($"표적 위치 표준편차 Z : {tar_D_Pos_STD}");
                //unsigned short tar_N_Vel_STD;  // 표적 속도 표준편차 N
                double tar_N_Vel_STD = arrUshortTmp[25];
                tar_D_Pos_STD = checkMinus2(tar_D_Pos_STD) * 200.0 / (65536 - 1);
                Form1.showText($"표적 속도 표준편차 X : {tar_N_Vel_STD}");
                //unsigned short tar_E_Vel_STD;  // 표적 속도 표준편차 E
                double tar_E_Vel_STD = arrUshortTmp[26];
                tar_D_Pos_STD = checkMinus2(tar_D_Pos_STD) * 200.0 / (65536 - 1);
                Form1.showText($"표적 속도 표준편차 Y : {tar_E_Vel_STD}");
                //unsigned short tar_D_Vel_STD;  // 표적 속도 표준편차 D
                double tar_D_Vel_STD = arrUshortTmp[27];
                tar_D_Pos_STD = checkMinus2(tar_D_Pos_STD) * 200.0 / (65536 - 1);
                Form1.showText($"표적 속도 표준편차 Z : {tar_D_Vel_STD}");
                //unsigned short delayTime;     // Delay time 측정/송신)
                ushort delayTime = arrUshortTmp[28];
                Form1.showText($"표적Delay time : {delayTime}");
                //unsigned short padding[3];
            }

        }
        private double checkMinus(double _data)
        {
            long tmp = (long)_data;
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
        private double checkMinus2(double _data)
        {
            double dTmp = _data;
            Int16 tmp = (Int16)_data;
            if (dTmp >= 0x8000)
            {
                tmp = (Int16)~tmp;
                tmp = (Int16)(tmp + 0x0001);
                dTmp = tmp;
                dTmp = (-1 * dTmp);
            }
            else
            {
                return dTmp;
            }
            return dTmp;
        }
        private double checkMinus3(double _data, double _range, double _lsb)
        {
            double dTmp = _data;
            Int16 tmp = (Int16)_data;
            if (dTmp >= 0x8000)
            {
                tmp = (Int16)~tmp;
                tmp = (Int16)(tmp + 0x0001);
                dTmp = (double)(tmp * _range / _lsb);
                dTmp = (-1 * dTmp);
            }
            else
            {
                return dTmp;
            }
            return dTmp;
        }
        /*=================================================*/
    }
}