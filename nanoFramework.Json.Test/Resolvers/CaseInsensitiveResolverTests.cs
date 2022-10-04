using nanoFramework.Json.Resolvers;
using nanoFramework.TestFramework;
using System;

namespace nanoFramework.Json.Test.Resolvers
{
    [TestClass]
    public class CaseInsensitiveResolverTests
    {
        private class TestClass
        {
            public int TestField;
            public int TestProperty { get; set; }
            public int SkipProperty { private get; set; }
            public int ThrowProperty { get; }
        }

        [TestMethod]
        [DataRow("TestField")]
        [DataRow("testfield")]
        [DataRow("TESTFIELD")]
        [DataRow("TesTfIEld")]
        public void CaseInsensitiveResolver_Get_ShouldResolveField(string fieldName)
        {
            var resolver = new CaseInsensitiveResolver();
            var classInstance = new TestClass();
            var valueToSet = 5;

            var member = resolver.Get(fieldName, typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.Equal(classInstance.TestField, valueToSet);
            Assert.Equal(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        [DataRow("TestProperty")]
        [DataRow("testproperty")]
        [DataRow("TESTPROPERTY")]
        [DataRow("tEsTPROperty")]
        public void CaseInsensitiveResolver_Get_ShouldResolveProperty(string propertyName)
        {
            var resolver = new CaseInsensitiveResolver();
            var classInstance = new TestClass();
            var valueToSet = 6;

            var member = resolver.Get(propertyName, typeof(TestClass));
            member.SetValue(classInstance, valueToSet);

            Assert.Equal(classInstance.TestProperty, valueToSet);
            Assert.Equal(member.ObjectType.FullName, typeof(int).FullName);
        }

        [TestMethod]
        public void CaseInsensitiveResolver_Get_ShouldSkipPropertyWithoutGet()
        {
            var resolver = new CaseInsensitiveResolver();

            var member = resolver.Get(nameof(TestClass.SkipProperty), typeof(TestClass));

            Assert.True(member.Skip);
        }

        [TestMethod]
        public void CaseInsensitiveResolver_Get_ShouldThrowForPropertyWithNoSet()
        {
            var resolver = new CaseInsensitiveResolver();

            try
            {
                var member = resolver.Get(nameof(TestClass.ThrowProperty), typeof(TestClass));
            }
            catch (DeserializationException)
            {
                // Intended. Method should throw this type of exception when no set method.
                return;
            }

            throw new Exception($"Should throw {nameof(DeserializationException)}.");
        }
    }
}
