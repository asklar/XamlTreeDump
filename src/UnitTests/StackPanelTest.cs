using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Controls;

namespace UnitTests
{
    [TestClass]
    public class StackPanelTest
    {
        [TestMethod]
        public void CompareByString()
        {
            var dump = Helper.GetDump(() =>
            {
                var sp = new StackPanel();
                CheckBox cb = new CheckBox() { IsChecked = true };
                sp.Children.Add(cb);
                return sp;
            });
            string expected = @"{
  'XamlType': 'Windows.UI.Xaml.Controls.StackPanel',
  'Background': null,
  'BorderBrush': null,
  'BorderThickness': '0,0,0,0',
  'Clip': null,
  'CornerRadius': '0,0,0,0',
  'FlowDirection': 'LeftToRight',
  'HorizontalAlignment': 'Stretch',
  'Margin': '0,0,0,0',
  'Padding': '0,0,0,0',
  'RenderSize': [0, 0],
  'VerticalAlignment': 'Stretch',
  'Visibility': 'Visible',
  'children': [
  {
    'XamlType': 'Windows.UI.Xaml.Controls.CheckBox',
    'Background': null,
    'BorderBrush': null,
    'BorderThickness': '0,0,0,0',
    'Clip': null,
    'CornerRadius': '0,0,0,0',
    'FlowDirection': 'LeftToRight',
    'Foreground': '#FF000000',
    'HorizontalAlignment': 'Stretch',
    'Margin': '0,0,0,0',
    'Padding': '0,0,0,0',
    'RenderSize': [0, 0],
    'VerticalAlignment': 'Stretch',
    'Visibility': 'Visible'
  }
  ]
}
".Replace("'", "\"");
            Assert.IsTrue(TreeDumpLibrary.TreeDumpHelper.DumpsAreEqual(expected, dump));
        }
    }
}
