using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Runners.UI;

namespace Xunit.Runners.ResultChannels
{
    public class TextWriterResultChannel : IResultChannel
    {
        int failed;
        int passed;
        int skipped;
        TextWriter writer;
        readonly object lockOjb = new object();

        public TextWriterResultChannel(TextWriter writer)
        {
            this.writer = writer;
        }

        public void RecordResult(TestResultViewModel result)
        {
            lock (lockOjb)
            {

                if (writer == null)
                    return;

                if (result.TestCase.Result == TestState.Passed)
                {
                    writer.Write("\t[PASS] ");
                    passed++;
                }
                else if (result.TestCase.Result == TestState.Skipped)
                {
                    writer.Write("\t[SKIPPED] ");
                    skipped++;
                }
                else if (result.TestCase.Result == TestState.Failed)
                {
                    writer.Write("\t[FAIL] ");
                    failed++;
                }
                else
                {
                    writer.Write("\t[INFO] ");
                }
                writer.Write(result.TestCase.DisplayName);

                var message = result.ErrorMessage;
                if (!string.IsNullOrEmpty(message))
                {
                    writer.Write(" : {0}", message.Replace("\r\n", "\\r\\n"));
                }
                writer.WriteLine();

                var stacktrace = result.ErrorStackTrace;
                if (!string.IsNullOrEmpty(result.ErrorStackTrace))
                {
                    var lines = stacktrace.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                        writer.WriteLine("\t\t{0}", line);
                }
            }
        }

        public Task<bool> OpenChannel(string message = null)
        {
            lock (lockOjb)
            {
                var r = OpenWriter(message);
                if (r)
                {
                    failed = passed = skipped = 0;
                }
                return Task.FromResult(r);
            }
        }

        public Task CloseChannel()
        {
            lock (lockOjb)
            {
                var total = passed + failed; // ignored are *not* run
                writer.WriteLine("Tests run: {0} Passed: {1} Failed: {2} Skipped: {3}", total, passed, failed, skipped);
                writer.Dispose();
                writer = null;
                return Task.FromResult(true);
            }
        }


        bool OpenWriter(string message)
        {
            if (writer == null)
            {
                // TODO: Add options support and use TcpTextWriter
                writer = new StringWriter();
            }
            return true;
        }

        static string SelectHostName(IReadOnlyList<string> names, int port)
        {
            if (names.Count == 0)
                return null;

            if (names.Count == 1)
                return names[0];

            var lock_obj = new object();
            string result = null;
            var failures = 0;

            using (var evt = new ManualResetEventSlim(false))
            {
                for (var i = names.Count - 1; i >= 0; i--)
                {
                    var name = names[i];
                    Task.Run(() =>
                             {
                                 try
                                 {
                                     var client = new TcpClient(name, port);
                                     using (var writer = new StreamWriter(client.GetStream()))
                                     {
                                         writer.WriteLine("ping");
                                     }
                                     lock (lock_obj)
                                     {
                                         if (result == null)
                                             result = name;
                                     }
                                     evt.Set();
                                 }
                                 catch (Exception)
                                 {
                                     lock (lock_obj)
                                     {
                                         failures++;
                                         if (failures == names.Count)
                                             evt.Set();
                                     }
                                 }
                             });
                }

                // Wait for 1 success or all failures
                evt.Wait();
            }

            return result;
        }
    }
}
