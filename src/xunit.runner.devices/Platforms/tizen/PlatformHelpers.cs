using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.Applications;


namespace Xunit.Runners
{
    static class PlatformHelpers
    {
        public static void TerminateWithSuccess()
        {
            Application.Current.Exit();
        }
    }
}
