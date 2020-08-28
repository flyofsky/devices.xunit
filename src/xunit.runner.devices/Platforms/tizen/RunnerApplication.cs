using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xunit.Runners.ResultChannels;

namespace Xunit.Runners.UI
{
    public abstract class RunnerApplication : FormsApplication
    {
        readonly List<Assembly> testAssemblies = new List<Assembly>();

        FormsRunner runner;

        Assembly executionAssembly;
        protected bool Initialized { get; private set; }

        protected bool TerminateAfterExecution { get; set; }
        [Obsolete("Use ResultChannel")]
        protected TextWriter Writer { get; set; }
        protected IResultChannel ResultChannel { get; set; } 
        protected bool AutoStart { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();

            Forms.Init(this);

            RunnerOptions.Current.TerminateAfterExecution = TerminateAfterExecution;
            RunnerOptions.Current.AutoStart = AutoStart;

            runner = new FormsRunner(executionAssembly, testAssemblies, ResultChannel ?? new TextWriterResultChannel(Writer));
            LoadApplication(runner);
        }

        protected void AddExecutionAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            if (!Initialized)
            {
                executionAssembly = assembly;
            }
        }

        protected void AddTestAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            if (!Initialized)
            {
                testAssemblies.Add(assembly);
            }
        }
    }
}
