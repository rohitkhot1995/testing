using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gaming.Predictor.Admin.App_Code
{
    public static class Extension
    {
        public static Tuple<int, string> DefaultInput()
        {
            return new Tuple<int, string>(-30, "");
        }

        public static Tuple<int, string> InsufficientInput()
        {
            return new Tuple<int, string>(-20, "");
        }
    }
}
