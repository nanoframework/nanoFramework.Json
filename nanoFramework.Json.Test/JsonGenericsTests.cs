//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Json.Resolvers;
using nanoFramework.TestFramework;
using System;
using System.Reflection;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonGenericsTests
    {
        public class Item
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public class Box<T>
        {
            public T Value { get; set; }
        }

        public class FieldBox<T>
        {
            public T Value;
        }

        public class ArrayBox<T>
        {
            public T[] Items { get; set; }
        }

        public class Wrapper<T>
        {
            public Box<T> Inner { get; set; }
        }

        public class Twin
        {
            public Box<int> Number { get; set; }
            public Box<string> Text { get; set; }
        }

        public class Counter<T>
        {
            public static int Created;
            public static string Tag = "initial";

            public int Id { get; set; }

            public Counter()
            {
                Created++;
                Id = Created;
            }
        }

        public class WithStaticConstructor<T>
        {
            public static readonly string Name;

            static WithStaticConstructor()
            {
                Name = typeof(T).Name;
            }

            public T Value { get; set; }
        }

        #region Round trips: generic class with T

        [TestMethod]
        public void SerializeBoxOfInt()
        {
            var json = JsonConvert.SerializeObject(new Box<int> { Value = 5 });

            Assert.AreEqual("{\"Value\":5}", json);
        }

        [TestMethod]
        public void RoundTripBoxOfInt()
        {
            var json = JsonConvert.SerializeObject(new Box<int> { Value = 42 });

            var result = (Box<int>)JsonConvert.DeserializeObject(json, typeof(Box<int>));

            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        public void RoundTripBoxOfString()
        {
            var json = JsonConvert.SerializeObject(new Box<string> { Value = "hello" });

            var result = (Box<string>)JsonConvert.DeserializeObject(json, typeof(Box<string>));

            Assert.AreEqual("hello", result.Value);
        }

        [TestMethod]
        public void RoundTripBoxOfClass()
        {
            var source = new Box<Item> { Value = new Item { Name = "nano", Count = 3 } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<Item>)JsonConvert.DeserializeObject(json, typeof(Box<Item>));

            Assert.IsNotNull(result.Value);
            Assert.AreEqual("nano", result.Value.Name);
            Assert.AreEqual(3, result.Value.Count);
        }

        [TestMethod]
        public void RoundTripBoxOfIntArray()
        {
            var source = new Box<int[]> { Value = new[] { 1, 2, 3 } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<int[]>)JsonConvert.DeserializeObject(json, typeof(Box<int[]>));

            Assert.AreEqual(3, result.Value.Length);
            Assert.AreEqual(1, result.Value[0]);
            Assert.AreEqual(2, result.Value[1]);
            Assert.AreEqual(3, result.Value[2]);
        }

        [TestMethod]
        public void RoundTripBoxOfStringArray()
        {
            var source = new Box<string[]> { Value = new[] { "a", "b" } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<string[]>)JsonConvert.DeserializeObject(json, typeof(Box<string[]>));

            Assert.AreEqual(2, result.Value.Length);
            Assert.AreEqual("a", result.Value[0]);
            Assert.AreEqual("b", result.Value[1]);
        }

        [TestMethod]
        public void RoundTripFieldBoxOfInt()
        {
            var result = (FieldBox<int>)JsonConvert.DeserializeObject("{\"Value\":7}", typeof(FieldBox<int>));

            Assert.AreEqual(7, result.Value);
        }

        [TestMethod]
        public void RoundTripFieldBoxOfString()
        {
            var result = (FieldBox<string>)JsonConvert.DeserializeObject("{\"Value\":\"seven\"}", typeof(FieldBox<string>));

            Assert.AreEqual("seven", result.Value);
        }

        [TestMethod]
        public void RoundTripFieldBoxOfClass()
        {
            var result = (FieldBox<Item>)JsonConvert.DeserializeObject("{\"Value\":{\"Name\":\"x\",\"Count\":9}}", typeof(FieldBox<Item>));

            Assert.AreEqual("x", result.Value.Name);
            Assert.AreEqual(9, result.Value.Count);
        }

        #endregion

        #region Round trips: T[] and arrays of generic types

        [TestMethod]
        public void RoundTripArrayBoxOfInt()
        {
            var json = JsonConvert.SerializeObject(new ArrayBox<int> { Items = new[] { 10, 20, 30 } });

            var result = (ArrayBox<int>)JsonConvert.DeserializeObject(json, typeof(ArrayBox<int>));

            Assert.AreEqual(3, result.Items.Length);
            Assert.AreEqual(10, result.Items[0]);
            Assert.AreEqual(30, result.Items[2]);
        }

        [TestMethod]
        public void RoundTripArrayBoxOfString()
        {
            var json = JsonConvert.SerializeObject(new ArrayBox<string> { Items = new[] { "x", "y" } });

            var result = (ArrayBox<string>)JsonConvert.DeserializeObject(json, typeof(ArrayBox<string>));

            Assert.AreEqual(2, result.Items.Length);
            Assert.AreEqual("x", result.Items[0]);
            Assert.AreEqual("y", result.Items[1]);
        }

        [TestMethod]
        public void RoundTripArrayBoxOfClass()
        {
            var source = new ArrayBox<Item>
            {
                Items = new[] { new Item { Name = "a", Count = 1 }, new Item { Name = "b", Count = 2 } }
            };

            var json = JsonConvert.SerializeObject(source);
            var result = (ArrayBox<Item>)JsonConvert.DeserializeObject(json, typeof(ArrayBox<Item>));

            Assert.AreEqual(2, result.Items.Length);
            Assert.AreEqual("a", result.Items[0].Name);
            Assert.AreEqual(2, result.Items[1].Count);
        }

        [TestMethod]
        public void DeserializeArrayOfBoxOfInt()
        {
            const string json = "[{\"Value\":1},{\"Value\":2},{\"Value\":3}]";

            var result = (Box<int>[])JsonConvert.DeserializeObject(json, typeof(Box<int>[]));

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1, result[0].Value);
            Assert.AreEqual(2, result[1].Value);
            Assert.AreEqual(3, result[2].Value);
        }

        [TestMethod]
        public void DeserializeArrayOfBoxOfString()
        {
            const string json = "[{\"Value\":\"a\"},{\"Value\":\"b\"}]";

            var result = (Box<string>[])JsonConvert.DeserializeObject(json, typeof(Box<string>[]));

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("a", result[0].Value);
            Assert.AreEqual("b", result[1].Value);
        }

        [TestMethod]
        public void SerializeArrayOfBoxOfInt()
        {
            var json = JsonConvert.SerializeObject(new[] { new Box<int> { Value = 1 }, new Box<int> { Value = 2 } });

            Assert.AreEqual("[{\"Value\":1},{\"Value\":2}]", json);
        }

        #endregion

        #region Nested generics

        [TestMethod]
        public void RoundTripBoxOfBoxOfInt()
        {
            var source = new Box<Box<int>> { Value = new Box<int> { Value = 11 } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<Box<int>>)JsonConvert.DeserializeObject(json, typeof(Box<Box<int>>));

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(11, result.Value.Value);
        }

        [TestMethod]
        public void RoundTripBoxOfBoxOfString()
        {
            var source = new Box<Box<string>> { Value = new Box<string> { Value = "deep" } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<Box<string>>)JsonConvert.DeserializeObject(json, typeof(Box<Box<string>>));

            Assert.AreEqual("deep", result.Value.Value);
        }

        [TestMethod]
        public void RoundTripBoxOfBoxOfClass()
        {
            var source = new Box<Box<Item>> { Value = new Box<Item> { Value = new Item { Name = "n", Count = 4 } } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<Box<Item>>)JsonConvert.DeserializeObject(json, typeof(Box<Box<Item>>));

            Assert.AreEqual("n", result.Value.Value.Name);
            Assert.AreEqual(4, result.Value.Value.Count);
        }

        [TestMethod]
        public void RoundTripBoxOfBoxOfIntArray()
        {
            var source = new Box<Box<int[]>> { Value = new Box<int[]> { Value = new[] { 5, 6 } } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Box<Box<int[]>>)JsonConvert.DeserializeObject(json, typeof(Box<Box<int[]>>));

            Assert.AreEqual(2, result.Value.Value.Length);
            Assert.AreEqual(5, result.Value.Value[0]);
            Assert.AreEqual(6, result.Value.Value[1]);
        }

        [TestMethod]
        public void RoundTripWrapperOfInt()
        {
            var source = new Wrapper<int> { Inner = new Box<int> { Value = 8 } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Wrapper<int>)JsonConvert.DeserializeObject(json, typeof(Wrapper<int>));

            Assert.AreEqual(8, result.Inner.Value);
        }

        [TestMethod]
        public void RoundTripWrapperOfString()
        {
            var source = new Wrapper<string> { Inner = new Box<string> { Value = "inner" } };

            var json = JsonConvert.SerializeObject(source);
            var result = (Wrapper<string>)JsonConvert.DeserializeObject(json, typeof(Wrapper<string>));

            Assert.AreEqual("inner", result.Inner.Value);
        }

        #endregion

        #region Two instantiations in the same graph

        [TestMethod]
        public void RoundTripTwoInstantiationsInOneGraph()
        {
            var source = new Twin
            {
                Number = new Box<int> { Value = 99 },
                Text = new Box<string> { Value = "ninety-nine" }
            };

            var json = JsonConvert.SerializeObject(source);
            var result = (Twin)JsonConvert.DeserializeObject(json, typeof(Twin));

            Assert.AreEqual(99, result.Number.Value);
            Assert.AreEqual("ninety-nine", result.Text.Value);
        }

        [TestMethod]
        public void InstantiationsDoNotShareTypeIdentity()
        {
            Assert.IsTrue(typeof(Box<int>) != typeof(Box<string>));
            Assert.IsTrue(typeof(Box<int>) == new Box<int>().GetType());
            Assert.IsTrue(typeof(Box<string>) == new Box<string>().GetType());
            Assert.IsTrue(typeof(Box<Box<int>>) != typeof(Box<Box<string>>));
            Assert.IsTrue(typeof(Box<int[]>) != typeof(Box<string[]>));
        }

        [TestMethod]
        public void DeserializeInterleavedInstantiations()
        {
            var number = (Box<int>)JsonConvert.DeserializeObject("{\"Value\":1}", typeof(Box<int>));
            var text = (Box<string>)JsonConvert.DeserializeObject("{\"Value\":\"one\"}", typeof(Box<string>));
            var number2 = (Box<int>)JsonConvert.DeserializeObject("{\"Value\":2}", typeof(Box<int>));

            Assert.AreEqual(1, number.Value);
            Assert.AreEqual("one", text.Value);
            Assert.AreEqual(2, number2.Value);
        }

        #endregion

        #region Reflection with right and wrong generic argument

        [TestMethod]
        public void FieldSetValueWithMatchingArgument()
        {
            var instance = new FieldBox<int>();
            FieldInfo field = typeof(FieldBox<int>).GetField("Value");

            field.SetValue(instance, 12);

            Assert.AreEqual(12, instance.Value);
            Assert.AreEqual(12, (int)field.GetValue(instance));
        }

        [TestMethod]
        public void FieldSetValueWithWrongArgumentThrows()
        {
            var instance = new FieldBox<int>();
            FieldInfo field = typeof(FieldBox<int>).GetField("Value");

            Assert.IsTrue(Throws(() => field.SetValue(instance, "not an int")));
            Assert.AreEqual(0, instance.Value);
        }

        [TestMethod]
        public void FieldSetValueOnStringInstantiationWithIntThrows()
        {
            var instance = new FieldBox<string>();
            FieldInfo field = typeof(FieldBox<string>).GetField("Value");

            Assert.IsTrue(Throws(() => field.SetValue(instance, 5)));
            Assert.IsNull(instance.Value);
        }

        [TestMethod]
        public void FieldSetValueOnClassInstantiationWithOtherClassThrows()
        {
            var instance = new FieldBox<Item>();
            FieldInfo field = typeof(FieldBox<Item>).GetField("Value");

            Assert.IsTrue(Throws(() => field.SetValue(instance, new Twin())));
            Assert.IsNull(instance.Value);
        }

        [TestMethod]
        public void FieldSetValueOnArrayInstantiation()
        {
            var instance = new FieldBox<int[]>();
            FieldInfo field = typeof(FieldBox<int[]>).GetField("Value");

            field.SetValue(instance, new[] { 1, 2 });
            Assert.AreEqual(2, instance.Value.Length);

            Assert.IsTrue(Throws(() => field.SetValue(instance, new[] { "a" })));
            Assert.AreEqual(2, instance.Value.Length);
        }

        [TestMethod]
        public void FieldSetValueAcrossInstantiationsThrows()
        {
            var instance = new FieldBox<int>();
            FieldInfo wrongField = typeof(FieldBox<string>).GetField("Value");

            Assert.IsTrue(Throws(() => wrongField.SetValue(instance, "text")));
        }

        [TestMethod]
        public void PropertySetterInvokeWithMatchingArgument()
        {
            var instance = new Box<string>();
            MethodInfo setter = typeof(Box<string>).GetMethod("set_Value");

            setter.Invoke(instance, new object[] { "ok" });

            Assert.AreEqual("ok", instance.Value);
        }

        [TestMethod]
        public void PropertySetterInvokeWithWrongArgumentThrows()
        {
            var instance = new Box<int>();
            MethodInfo setter = typeof(Box<int>).GetMethod("set_Value");

            Assert.IsTrue(Throws(() => setter.Invoke(instance, new object[] { "not an int" })));
            Assert.AreEqual(0, instance.Value);
        }

        [TestMethod]
        public void PropertySetterInvokeOnWrongInstantiationThrows()
        {
            var instance = new Box<int>();
            MethodInfo setter = typeof(Box<string>).GetMethod("set_Value");

            Assert.IsTrue(Throws(() => setter.Invoke(instance, new object[] { "text" })));
        }

        [TestMethod]
        public void PropertyGetterInvokeReturnsStoredValue()
        {
            var instance = new Box<Item> { Value = new Item { Name = "g" } };
            MethodInfo getter = typeof(Box<Item>).GetMethod("get_Value");

            var value = (Item)getter.Invoke(instance, null);

            Assert.AreEqual("g", value.Name);
        }

        [TestMethod]
        public void ConstructorInvokeCreatesRequestedInstantiation()
        {
            ConstructorInfo ctorInt = typeof(Box<int>).GetConstructor(new Type[] { });
            ConstructorInfo ctorString = typeof(Box<string>).GetConstructor(new Type[] { });

            var asInt = ctorInt.Invoke(null);
            var asString = ctorString.Invoke(null);

            Assert.IsTrue(asInt is Box<int>);
            Assert.IsFalse(asInt is Box<string>);
            Assert.IsTrue(asString is Box<string>);
            Assert.IsFalse(asString is Box<int>);
        }

        [TestMethod]
        public void MemberResolverResolvesGenericFieldAndProperty()
        {
            var sut = new MemberResolver();

            var propertyOfInt = sut.Get("Value", typeof(Box<int>), new JsonSerializerOptions());
            var propertyOfString = sut.Get("Value", typeof(Box<string>), new JsonSerializerOptions());
            var fieldOfInt = sut.Get("Value", typeof(FieldBox<int>), new JsonSerializerOptions());
            var fieldOfString = sut.Get("Value", typeof(FieldBox<string>), new JsonSerializerOptions());

            Assert.AreEqual(typeof(int).FullName, propertyOfInt.ObjectType.FullName);
            Assert.AreEqual(typeof(string).FullName, propertyOfString.ObjectType.FullName);
            Assert.AreEqual(typeof(int).FullName, fieldOfInt.ObjectType.FullName);
            Assert.AreEqual(typeof(string).FullName, fieldOfString.ObjectType.FullName);
        }

        [TestMethod]
        public void MemberResolverSetValueWithMatchingArgument()
        {
            var sut = new MemberResolver();
            var box = new Box<int>();
            var fieldBox = new FieldBox<string>();

            sut.Get("Value", typeof(Box<int>), new JsonSerializerOptions()).SetValue(box, 3);
            sut.Get("Value", typeof(FieldBox<string>), new JsonSerializerOptions()).SetValue(fieldBox, "three");

            Assert.AreEqual(3, box.Value);
            Assert.AreEqual("three", fieldBox.Value);
        }

        [TestMethod]
        public void MemberResolverSetValueWithWrongArgumentThrows()
        {
            var sut = new MemberResolver();
            var box = new Box<int>();
            var fieldBox = new FieldBox<int>();

            var propertySet = sut.Get("Value", typeof(Box<int>), new JsonSerializerOptions());
            var fieldSet = sut.Get("Value", typeof(FieldBox<int>), new JsonSerializerOptions());

            Assert.IsTrue(Throws(() => propertySet.SetValue(box, "text")));
            Assert.IsTrue(Throws(() => fieldSet.SetValue(fieldBox, "text")));
            Assert.AreEqual(0, box.Value);
            Assert.AreEqual(0, fieldBox.Value);
        }

        [TestMethod]
        public void DeserializeStringValueIntoBoxOfIntThrows()
        {
            Assert.IsTrue(Throws(() => JsonConvert.DeserializeObject("{\"Value\":\"abc\"}", typeof(Box<int>))));
        }

        #endregion

        #region Static state of generic types

        [TestMethod]
        public void StaticFieldsAreNotSharedBetweenInstantiations()
        {
            Counter<int>.Created = 0;
            Counter<string>.Created = 0;
            Counter<Item>.Created = 0;

            var a = new Counter<int>();
            var b = new Counter<int>();
            var c = new Counter<string>();

            Assert.AreEqual(2, Counter<int>.Created);
            Assert.AreEqual(1, Counter<string>.Created);
            Assert.AreEqual(0, Counter<Item>.Created);
            Assert.AreEqual(2, b.Id);
            Assert.AreEqual(1, c.Id);
            Assert.AreEqual(1, a.Id);
        }

        [TestMethod]
        public void StaticReferenceFieldsAreNotSharedBetweenInstantiations()
        {
            Counter<int>.Tag = "int";
            Counter<string>.Tag = "string";

            Assert.AreEqual("int", Counter<int>.Tag);
            Assert.AreEqual("string", Counter<string>.Tag);
        }

        [TestMethod]
        public void StaticFieldsReadAndWrittenThroughReflection()
        {
            Counter<int>.Created = 5;
            Counter<string>.Created = 7;

            FieldInfo intField = typeof(Counter<int>).GetField("Created");
            FieldInfo stringField = typeof(Counter<string>).GetField("Created");

            Assert.AreEqual(5, (int)intField.GetValue(null));
            Assert.AreEqual(7, (int)stringField.GetValue(null));

            intField.SetValue(null, 50);

            Assert.AreEqual(50, Counter<int>.Created);
            Assert.AreEqual(7, Counter<string>.Created);
        }

        [TestMethod]
        public void StaticFieldReflectionWithWrongArgumentThrows()
        {
            FieldInfo field = typeof(Counter<int>).GetField("Created");

            Assert.IsTrue(Throws(() => field.SetValue(null, "text")));
        }

        [TestMethod]
        public void StaticConstructorRunsPerInstantiation()
        {
            Assert.AreEqual("Int32", WithStaticConstructor<int>.Name);
            Assert.AreEqual("String", WithStaticConstructor<string>.Name);
            Assert.AreEqual("Item", WithStaticConstructor<Item>.Name);
            Assert.AreEqual("Int32", WithStaticConstructor<int>.Name);
        }

        [TestMethod]
        public void StaticMembersDoNotAffectSerialization()
        {
            Counter<int>.Created = 0;

            var json = JsonConvert.SerializeObject(new Counter<int>());

            Assert.AreEqual("{\"Id\":1}", json);
        }

        [TestMethod]
        public void RoundTripGenericWithStaticConstructor()
        {
            var json = JsonConvert.SerializeObject(new WithStaticConstructor<string> { Value = "static" });

            var result = (WithStaticConstructor<string>)JsonConvert.DeserializeObject(json, typeof(WithStaticConstructor<string>));

            Assert.AreEqual("static", result.Value);
            Assert.AreEqual("String", WithStaticConstructor<string>.Name);
        }

        [TestMethod]
        public void DeserializeCreatesInstancesThroughConstructor()
        {
            Counter<int>.Created = 0;
            Counter<string>.Created = 0;

            JsonConvert.DeserializeObject("{\"Id\":1}", typeof(Counter<int>));
            JsonConvert.DeserializeObject("{\"Id\":1}", typeof(Counter<int>));
            JsonConvert.DeserializeObject("{\"Id\":1}", typeof(Counter<string>));

            Assert.AreEqual(2, Counter<int>.Created);
            Assert.AreEqual(1, Counter<string>.Created);
        }

        #endregion

        private static bool Throws(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                return true;
            }

            return false;
        }
    }
}
