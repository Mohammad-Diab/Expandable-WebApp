using System;
using System.Collections.Generic;
using System.Text;

namespace MyAppDB
{
    public class Converter
    {
        public static int ToInt32(object v)
        {
            if (v is DBNull)
                return 0;
            else
            {
                int result;
                int.TryParse(v.ToString(), out result);
                return result;
            }
        }

        public static byte ToByte(object v)
        {
            if (v is DBNull)
                return 0;
            else
            {
                byte result;
                byte.TryParse(v.ToString(), out result);
                return result;
            }
        }
    }
}
