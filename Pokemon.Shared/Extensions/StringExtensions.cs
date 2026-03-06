using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pokemon.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string str)
        {
            //Första tecknet stor
            str = char.ToUpper(str[0]) + str.Substring(1);
            //Annat litet
            return str;
        }
    }
}
