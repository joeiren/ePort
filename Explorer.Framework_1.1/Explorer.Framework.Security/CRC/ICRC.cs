using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Framework.Security.CRC
{
    public interface ICRC
    {
        byte Check(byte[] data);

        byte Check(byte[] data, int index, int length);
    }
}
