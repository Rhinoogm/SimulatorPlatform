using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    // 데이터 변환 등의 메서드를 저장한다.
    class ConvertMethods
    {
        public Byte[] _M_UInt16ArrayToByteArray(UInt16[] _U16_Array)
        {
            Byte[] _bytes = new Byte[_U16_Array.Length * 2];
            int cnt = 0;
            Byte[] _tmp = new Byte[2];
            foreach (UInt16 _u16 in _U16_Array)
            {
                _tmp = BitConverter.GetBytes(_u16);
                _bytes[cnt] = _tmp[1];
                _bytes[cnt + 1] = _tmp[0];
                cnt = cnt + 2;
            }
            return _bytes;
        }

        public static Byte[] UshortarrayToBytearray(ushort[] _usTmp)
        {
            int iSize = _usTmp.Length * 2;
            ushort[] usTmp = new ushort[iSize / 2];
            Buffer.BlockCopy(_usTmp, 0, usTmp, 0, iSize);
            Byte[] bTmp = new Byte[iSize];
            Buffer.BlockCopy(usTmp, 0, bTmp, 0, iSize);
            Byte tmp;
            for (int i = 0; i < iSize; i = i + 2)
            {
                tmp = bTmp[i];
                bTmp[i] = bTmp[i + 1];
                bTmp[i + 1] = tmp;
            }
            return bTmp;
        }
        public static ushort[] BytearrayToUshortarray(Byte[] _bTmp)
        {
            int iSize = _bTmp.Length;
            byte[] bTmp = new byte[iSize];
            Buffer.BlockCopy(_bTmp, 0, bTmp, 0, iSize);
            ushort[] usTmp = new ushort[iSize / 2];
            Byte tmp;
            for (int i = 0; i < iSize; i = i + 2)
            {
                tmp = bTmp[i];
                bTmp[i] = bTmp[i + 1];
                bTmp[i + 1] = tmp;
            }
            Buffer.BlockCopy(bTmp, 0, usTmp, 0, iSize);
            return usTmp;
        }
        public static void swap2Bytes(ref ushort _data)
        {
            _data = (ushort)(((0x00FF & _data) << 8) | ((0xFF00 & _data) >> 8));
        }
    }
}