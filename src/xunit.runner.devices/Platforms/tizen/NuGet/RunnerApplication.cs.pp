using System;
using System.Reflection;
using test.xunit.pcltestlib;

using Xamarin.Forms.Platform.Tizen;

using Xunit.Runners.UI;
using Xunit.Sdk;


namespace $rootnamespace$
{
    class Program : RunnerApplication
    {
        protected override void OnCreate()
        {
            AddTestAssembly(Assembly.GetExecutingAssembly());

            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            // or in any reference assemblies			

            AddTestAssembly(typeof(PortableTests).Assembly);

            base.OnCreate();
        }

        static void Main(string[] args)
        {
            var app = new Program();
            //Forms.Init(app);
            app.Run(args);
        }
    }
}
