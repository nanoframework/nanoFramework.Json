using System;
using System.Text;

namespace nanoFramework.Json.Converters
{
    internal interface IConverter
    {
        object ToType(object value);
        string ToJson(object value);
    }
}
