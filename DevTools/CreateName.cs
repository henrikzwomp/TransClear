using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
    public class CreateName
    {
        public static string FromCurrentTime()
        {
            var now = DateTime.Now;

            string result = now.Year.ToString();

            if (now.Month < 10)
                result += 0;
            result += now.Month;

            if (now.Day < 10)
                result += 0;
            result += now.Day;

            if (now.Hour < 10)
                result += 0;
            result += now.Hour;

            if (now.Minute < 10)
                result += 0;
            result += now.Minute;

            if (now.Second < 10)
                result += 0;
            result += now.Second;

            if (now.Millisecond < 10)
                result += 0;
            if (now.Millisecond < 100)
                result += 0;
            result += now.Millisecond;

            result += ".lxf";

            return result;
        }
    }
}
