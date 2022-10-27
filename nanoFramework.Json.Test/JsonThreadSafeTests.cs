//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace nanoFramework.Json.Test
{
    [TestClass]
    public class JsonThreadSafeTests
    {
        private class TestObject
        {
            public string TestString { get; set; }
        }

        private class ThreadLogger
        {
            public int ErrorCount { get; set; }
        }


        [TestMethod]
        public void Deserialize_Should_BeThreadSafe()
        {
            ArrayList threads = new ArrayList();
            ArrayList threadsLogs = new ArrayList();
            var threadCount = 2;

            for (int i = 0; i < threadCount; i++)
            {
                var threadLoggger = new ThreadLogger();
                threadsLogs.Add(threadLoggger);
                var newThread = new Thread(() => ThreadMethod(threadLoggger));
                newThread.Start();
                threads.Add(newThread);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            // Let's wait a little bit so threads can do their job
            Thread.Sleep(TimeSpan.FromSeconds(30));

            // Abort all threads
            for (int i = 0; i < threadCount; i++)
            {
                ((Thread)threads[i]).Abort();
            }

            // Check for any errors in threads
            for (int i = 0; i < threadCount; i++)
            {
                Assert.Equal(0, ((ThreadLogger)threadsLogs[i]).ErrorCount);
            }
        }

        private static void ThreadMethod(ThreadLogger threadLogger)
        {
            var testStringValue = "towel";
            var json = $"{{\"TestString\":\"{testStringValue}\"}}";
            int count = 0;
            while (true)
            {
                try
                {
                    var result = (TestObject)JsonConvert.DeserializeObject(json, typeof(TestObject));
                    Debug.Write($"{count++}; ");
                    Assert.Equal(result.TestString, testStringValue);
                }
                catch (ThreadAbortException)
                {
                    // Intentionally, we are aborting thread in main test to clean up
                    break;
                }
                catch
                {
                    threadLogger.ErrorCount++;
                }
                Thread.Sleep(10);
            }
        }
    }
}
