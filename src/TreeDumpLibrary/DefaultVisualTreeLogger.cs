// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using Windows.UI.Xaml;

namespace TreeDumpLibrary
{
    /// <summary>
    /// A key=value -based tree logger. Deprecated.
    /// </summary>
    internal sealed class DefaultVisualTreeLogger : IVisualTreeLogger
    {
        public void BeginNode(int indent, string nodeName, DependencyObject obj)
        {
            AppendLogger(indent, $"[{nodeName}]");
        }

        public void EndNode(int indent, string nodeName, DependencyObject obj)
        { // no-op
        }

        public void LogProperty(int indent, string propertyName, object propertyValue)
        {
            AppendLogger(indent, $"{propertyName}={propertyValue}");
        }

        public override string ToString()
        {
            return _logger.ToString();
        }

        private readonly StringBuilder _logger = new StringBuilder();
        private void AppendLogger(int indent, string s)
        {
            _logger.AppendLine(s.PadLeft(2 * indent + s.Length));
        }

        public void BeginArray(int indent, string propertyName)
        { // no-op
        }

        public void EndArray(int indent, string propertyName)
        {
        }
    }
}
