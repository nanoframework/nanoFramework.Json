using System;

namespace nanoFramework.Json.Test.Shared
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public int ID { get; set; }
        public string[] ArrayProperty { get; set; }
        public Person Friend { get; set; }
    }
}
