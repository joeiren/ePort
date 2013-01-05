using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Security.CRC
{
    public enum CRCMode
    { 
        CRC8,
        CRC16,
        CRC32
    }

    public class CRCFactory
    {
        public static ICRC Create(CRCMode mode)
        {
            ICRC crc = null;
            switch (mode)
            { 
                case CRCMode.CRC8:
                    crc = new CRC8();
                    break;            
            }
            return crc;
        }
    }
}
