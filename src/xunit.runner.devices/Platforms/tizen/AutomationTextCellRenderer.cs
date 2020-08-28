using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xunit.Runners;
using Xunit.Runners.Utilities;

[assembly: ExportRenderer(typeof(AutomationTextCell), typeof(AutomationTextCellRenderer))]
namespace Xunit.Runners
{
    public class AutomationTextCellRenderer : TextCellRenderer
    {
    }
}
