using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Controls;

namespace UnitTests
{
    [TestClass]
    public class TextBlockTest
    {

        [TestMethod]
        public void ByString()
        {
            var dump = Helper.GetDump(() =>
            {
                return new TextBlock() { Text = "test" };
            });
            string expected = @"{
    'XamlType': 'Windows.UI.Xaml.Controls.TextBlock',
    'Clip': null,
    'FlowDirection': 'LeftToRight',
    'Foreground': '#FF000000',
    'HorizontalAlignment': 'Stretch',
    'Margin': '0,0,0,0',
    'Padding': '0,0,0,0',
    'RenderSize': [0, 0],
    'Text': 'test',
    'VerticalAlignment': 'Stretch',
    'Visibility': 'Visible'
}
".Replace("'", "\"");
            Assert.IsTrue(TreeDumpLibrary.TreeDumpHelper.DumpsAreEqual(expected, dump));
        }

        [TestMethod]
        public void ByJson()
        {
            var obj = new JObject {
                { "XamlType", "Windows.UI.Xaml.Controls.TextBlock" },
                { "Clip", null },
                { "FlowDirection", "LeftToRight" },
                { "Foreground", "#FF000000" },
                { "HorizontalAlignment", "Stretch" },
                { "Margin", "0,0,0,0" },
                { "Padding", "0,0,0,0" },
                { "RenderSize", new JArray { 0, 0 } },
                { "Text", "test" },
                { "VerticalAlignment", "Stretch" },
                { "Visibility", "Visible" }
            };
            var dump = Helper.GetDump(() => new TextBlock() { Text = "test" });
            JObject dumpObject = JObject.Parse(dump);
            Assert.AreEqual(obj.ToString(), dumpObject.ToString());
        }

    }
}
