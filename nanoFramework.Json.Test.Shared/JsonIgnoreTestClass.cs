//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Json.Test.Shared
{
    /// <summary>
    /// Used to test the JsonIgnore attribute.
    /// </summary>
    [JsonIgnore("MyIgnoredProperty, AnotherIgnoredProperty")]
    public class JsonIgnoreTestClass
    {
        public int TestProperty { get; set; }
        public int OtherTestProperty { get; set; }
        public int AThirdTestProperty { get; set; }
        public int MyIgnoredProperty => TestProperty + 1;
        public int AnotherIgnoredProperty => OtherTestProperty + 1;

        public int this[int index] => TestProperty + index;

        public static JsonIgnoreTestClass CreateTestClass()
        {
            return new JsonIgnoreTestClass()
            {
                TestProperty = 1,
                OtherTestProperty = 2,
                AThirdTestProperty = 3
            };
        }

        public bool IsEqual(JsonIgnoreTestClass otherInstance)
        {
            return (TestProperty == otherInstance.TestProperty
                && OtherTestProperty == otherInstance.OtherTestProperty
                && AThirdTestProperty == otherInstance.AThirdTestProperty);
        }
    }

    /// <summary>
    /// Used to 1-to-1 compare with JsonIgnoreTestClass.
    /// </summary>
    public class JsonIgnoreTestClassNoAttr
    {
        public int TestProperty { get; set; }
        public int OtherTestProperty { get; set; }
        public int AThirdTestProperty { get; set; }
        public int MyIgnoredProperty => TestProperty + 1;
        public int AnotherIgnoredProperty => OtherTestProperty + 1;

        public int this[int index] => TestProperty + index;

        public static JsonIgnoreTestClassNoAttr CreateTestClass()
        {
            return new JsonIgnoreTestClassNoAttr()
            {
                TestProperty = 1,
                OtherTestProperty = 2,
                AThirdTestProperty = 3
            };
        }
    }
}