using System;
using System.Collections;
using System.Text;

namespace nanoFramework.Json
{




    /// <summary>
    /// Hides properties from the json serializer
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        /// <summary>
        /// Hides properties from the json serializer
        /// </summary>
        public JsonIgnoreAttribute() {

            PropertyNames = new string[0];
        }
        /// <summary>
        /// array of property names for json serializer to ignore
        /// </summary>
        public string[] PropertyNames { get; set; }

        /// <summary>
        /// Hides properties from the json serializer
        /// </summary>
        /// <param name="getterNamesToIgnore"></param>
        public JsonIgnoreAttribute(string getterNamesToIgnore)
        {
            PropertyNames = getterNamesToIgnore.Split(',');
            for(int i = 0; i < PropertyNames.Length; i++)
            {
                PropertyNames[i] = PropertyNames[i].Trim();
            }
        }
    }

    //implementation to place above individual properties
    ///// <summary>
    ///// Hides a property from the json serializer
    ///// </summary>
    //[System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Method,
    //    Inherited = false, AllowMultiple = true)]
    //public sealed class JsonIgnoreAttribute : Attribute
    //{
    //    // See the attribute guidelines at 
    //    //  http://go.microsoft.com/fwlink/?LinkId=85236

    //    /// <summary>
    //    /// Hides a property from the json serializer
    //    /// </summary>
    //    public JsonIgnoreAttribute() { }
    //}

}
