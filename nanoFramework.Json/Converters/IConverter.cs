using System;
using System.Text;

namespace nanoFramework.Json.Converters
{
    //TODO: comments and tests
    // Tests should: Check values and types
    public interface IConverter
    {
        object ToType(object value);
        string ToJson(object value);
    }
}
