using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Json;

namespace QuickTest
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            string testJson3 = JsonConvert.SerializeObject(new TestObject() { Value = "I:\\nanoApp\\rNFTestApp2.pe" });
            Console.WriteLine("testJson3: " + testJson3);
            TestObject testObject3 = (TestObject)JsonConvert.DeserializeObject(testJson3, typeof(TestObject));
            Console.WriteLine("testJsonObject3: " + testObject3?.Value);

            string teststring = "I:\\nanoApp\\rNFTestApp2.pe";
            Console.WriteLine("teststring: " + teststring);
            string testJson = JsonConvert.SerializeObject(teststring);
            Console.WriteLine("testJson: " + testJson);
            string testStringDeserialized = (string)JsonConvert.DeserializeObject(testJson, typeof(string));
            Console.WriteLine("testJsonObject: " + testStringDeserialized);

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }

    public class TestObject
    {
        public string Value { get; set; }
    }
}
