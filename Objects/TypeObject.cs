using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer.Objects
{
    public enum TypeObject
    {
        Null = 0xFF,

        Int32 = 0x01,
        Int64 = 0x02,
        Double = 0x03,
        Decimal = 0x04,

        String = 0x05,

        Binary = 0x06,
        Guid = 0x07,

        Boolean = 0x08,
        DateTime = 0x09,

        Dictionary = 0x0A,
        Array = 0x0B
    }
}
