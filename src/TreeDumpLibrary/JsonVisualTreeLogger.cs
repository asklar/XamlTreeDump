// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using Windows.UI.Xaml;

namespace TreeDumpLibrary
{
    /// <summary>
    /// A Json-based tree format
    /// </summary>
    internal sealed class JsonVisualTreeLogger : IVisualTreeLogger
    {
        bool isFirstElement = true;
        private readonly StringBuilder _logger = new StringBuilder();
        public void BeginArray(int indent, string propertyName)
        {
            AppendLogger(indent, $"\"{propertyName}\": [\n", false);
            isFirstElement = true;
        }

        public void BeginNode(int indent, string nodeName, DependencyObject obj)
        {
            AppendLogger(indent, "{\n", false);
            isFirstElement = true;
            LogProperty(indent + 2, "XamlType", JsonPropertyValueTranslator.Quote(nodeName));
        }

        public void EndArray(int indent, string propertyName)
        {
            AppendLogger(indent, "]", true);
            isFirstElement = false;
        }

        public void EndNode(int indent, string nodeName, DependencyObject obj)
        {
            AppendLogger(indent, "}", true);
            isFirstElement = false;
        }

        public void LogProperty(int indent, string propertyName, object propertyValue)
        {
            AppendLogger(indent, $"\"{propertyName}\": {propertyValue}", false);
        }

        public override string ToString()
        {
            return _logger.ToString();
        }

        private void AppendLogger(int indent, string s, bool isClosing)
        {
            bool first = isFirstElement;
            isFirstElement = false;
            if (!first && !isClosing)
            {
                _logger.Append(",\n");
            }
            if (isClosing)
            {
                _logger.Append("\n");
            }

            _logger.Append(s.PadLeft(indent + s.Length));
        }
    }
}
