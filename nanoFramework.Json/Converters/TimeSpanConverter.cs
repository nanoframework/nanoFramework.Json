namespace nanoFramework.Json.Converters
{
    internal sealed class TimeSpanConverter : IConverter
    {
        public string ToJson(object value)
        {
            return "\"" + value.ToString() + "\"";
        }

        public object ToType(object value)
        {
            return TimeSpanExtensions.ConvertFromString(value.ToString());
        }
    }
}