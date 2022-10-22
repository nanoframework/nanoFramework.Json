using nanoFramework.TestFramework;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
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

            // Abrot all threads
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
            var json = "{\"TestString\":\"towel\"}";
            int count = 0;
            while (true)
            {
                try
                {
                    JsonConvert.DeserializeObject(json, typeof(TestObject));
                    Debug.Write($"{count++}; ");
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
