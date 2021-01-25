using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

        [TestMethod]
        public void TextHighlighter()
        {
            var dump = Helper.GetDump(() => {
                var tb = new TextBlock();
                var th = new Windows.UI.Xaml.Documents.TextHighlighter()
                {
                    Background = new SolidColorBrush(Colors.Red)
                };
                th.Ranges.Add(new Windows.UI.Xaml.Documents.TextRange() { Length = 3, StartIndex = 1 });
                tb.TextHighlighters.Add(th);
                return tb;
            }, new string[] { "TextHighlighters" });

            JObject dumpObject = JObject.Parse(dump);
            JObject obj = new JObject {
                { "XamlType", "Windows.UI.Xaml.Controls.TextBlock" },
                { "Clip", null },
                { "FlowDirection", "LeftToRight" },
                { "Foreground", "#FF000000" },
                { "HorizontalAlignment", "Stretch" },
                { "Margin", "0,0,0,0" },
                { "Padding", "0,0,0,0" },
                { "RenderSize", new JArray { 0, 0 } },
                { "Text", "" },
                { "TextHighlighters", new JArray {
                    new JObject {
                        { "Background", "#FFFF0000"},
                        { "Ranges", new JArray {
                            new JObject {
                                { "StartIndex", 1 },
                                { "Length", 3}
                            }
                        } }
                    }
                } },
                { "VerticalAlignment", "Stretch" },
                { "Visibility", "Visible" }
                };
            Assert.AreEqual(dumpObject.ToString(), obj.ToString());
        }
    }
}
