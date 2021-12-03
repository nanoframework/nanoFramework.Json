//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright 2007 James Newton-King, (c) Pervasive Digital LLC
// See LICENSE file in the project root for full license information.
//

using System.Text;

namespace nanoFramework.Json
{
    internal class JsonProperty : JsonToken
    {
        public JsonProperty()
        {
        }

        public JsonProperty(string name, JsonToken value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public JsonToken Value { get; set; }

        public override string ToString()
        {
            EnterSerialization();

            StringBuilder sb = new(); //TODO: why move out of the try?

            try
            {
                //Use minimalist JSON, pretty can be handled on the client!
                sb.Append('"');
                sb.Append(Name);
                sb.Append("\":");

                JsonToken token = Value;

                if (token is JsonValue j
                    && j.Value != null
                    && j.Value.GetType().Name == "Boolean")
                {
                    // need to convert Boolean values to lower case 
                    sb.Append(Value.ToString().ToLower());

                    return sb.ToString();
                }

                sb.Append(Value.ToString());

                return sb.ToString();
            }
            finally
            {
                ExitSerialization();
            }
        }
    }
}
