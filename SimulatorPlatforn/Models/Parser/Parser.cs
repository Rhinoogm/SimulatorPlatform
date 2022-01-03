using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SimulatorPlatform
{
    // 송수신 메시지, XML parsing 등을 관리한다.
    class Parser
    {
        private Marshaller cMarshaller;
        private Unmarshaller cUnmarshaller;
        public Parser()
        {
            cMarshaller = new Marshaller();
            cUnmarshaller = new Unmarshaller();
        }
        public Byte[] marshalData(Object _header)
        {
            return cMarshaller.marshalData(_header);
        }
        public Byte[] unmarshalData(Byte[] _data)
        {
            return cUnmarshaller.unmarshalData(_data);
        }
    }
}