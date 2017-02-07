using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class HelperNullable
    {
        public static bool ConvertBoolean(bool? vBoolNullable)
        {
            bool vBool = false;
            if (vBoolNullable == null)
                vBool = false;
            else
                vBool = (bool)vBoolNullable;

            return vBool;
        }

        public static int ConvertInt(int? vIntNullable)
        {
            int vInt = 0;
            if (vIntNullable == null)
                vInt = 0;
            else
                vInt = (int)vIntNullable;

            return vInt;
        }
    }
}
