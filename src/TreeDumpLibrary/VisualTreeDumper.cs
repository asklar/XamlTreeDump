// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace TreeDumpLibrary
{
    /// <summary>
    /// The mode to use (Default, or Json)
    /// </summary>
    public enum DumpTreeMode
    {
        /// <summary>
        /// a Key=Value format
        /// </summary>
        Default,
        /// <summary>
        /// Json format
        /// </summary>
        Json
    }

    /// <summary>
    /// The main class in this library
    /// </summary>
    public sealed class VisualTreeDumper
    {
        class Visitor
        {
            private readonly IVisualTreeLogger _logger;
            private int _indent;
            private readonly DefaultFilter _filter;
            private readonly IPropertyValueTranslator _translator;
            public Visitor(DefaultFilter filter, IPropertyValueTranslator translator, IVisualTreeLogger logger)
            {
                _indent = 0;
                _filter = filter;
                _translator = translator;
                _logger = logger;
            }
            public void EndVisitNode(DependencyObject obj)
            {
                _indent--;
                _logger.EndNode(_indent, obj.GetType().FullName, obj);
            }

            public void BeginVisitNode(DependencyObject obj)
            {
                _logger.BeginNode(_indent, obj.GetType().FullName, obj);
                _indent++;
            }

            public override string ToString()
            {
                return _logger.ToString();
            }

            public bool ShouldVisitPropertiesForNode(DependencyObject node)
            {
                var fe = node as FrameworkElement;
                string[] excludedNames = new string[] { "VerticalScrollBar", "HorizontalScrollBar", "ScrollBarSeparator" };
                if (fe != null && excludedNames.Contains(fe.Name)) { return false; }
                return true;
            }

            public bool ShouldVisitProperty(string propertyName)
            {
                return _filter.ShouldVisitProperty(propertyName);
            }

            public void VisitProperty(string propertyName, object value)
            {
                var v = _translator.PropertyValueToString(propertyName, value);
                _logger.LogProperty(_indent + 1, propertyName, v);
            }

            public void BeginChildren()
            {
                _logger.BeginArray(++_indent, "children");
            }

            public void EndChildren()
            {
                _logger.EndArray(_indent--, "children");
            }

            public bool ShouldVisitPropertyValue(string propertyName, object value)
            {
                string s = _translator.PropertyValueToString(propertyName, value);
                if (propertyName == "Name")
                {
                    string name = value as string;
                    return !name.StartsWith("<reacttag>:") &&
                        name != "";
                }
                return _filter.ShouldVisitPropertyValue(s);
            }
        }

        /// <summary>
        /// Finds the element by automation identifier.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="automationId">The automation identifier.</param>
        /// <returns></returns>
        public static JsonObject FindElementByAutomationId(JsonObject obj, string automationId)
        {
            if (obj.Keys.Contains("AutomationId") && obj["AutomationId"].GetString() == automationId)
            {
                return obj;
            }
            if (obj.Keys.Contains("children"))
            {
                var array = obj.GetNamedArray("children");
                foreach (var i in array)
                {
                    var element = FindElementByAutomationId(i.GetObject(), automationId);
                    if (element != null)
                    {
                        return element;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The main entry point to the library. Produces a tre dump starting at <paramref name="root"/>
        /// </summary>
        /// <param name="root">The node to start the walk from</param>
        /// <param name="excludedNode">A node to exclude, or null</param>
        /// <param name="additionalProperties">a list of additional properties to extract from each node</param>
        /// <param name="mode">The format to use (Json or default)</param>
        /// <returns></returns>
        public static string DumpTree(DependencyObject root, DependencyObject excludedNode, IList<string> additionalProperties, DumpTreeMode mode)
        {
            var propertyFilter = new DefaultFilter();
            ((List<string>)propertyFilter.PropertyNameAllowList).AddRange(additionalProperties);

            IPropertyValueTranslator translator = (mode == DumpTreeMode.Json ?
                new JsonPropertyValueTranslator() as IPropertyValueTranslator :
                new DefaultPropertyValueTranslator());
            IVisualTreeLogger logger = (mode == DumpTreeMode.Json ?
                new JsonVisualTreeLogger() as IVisualTreeLogger :
                new DefaultVisualTreeLogger());
            Visitor visitor = new Visitor(propertyFilter, translator, logger);

            WalkThroughTree(root, excludedNode, visitor);

            return visitor.ToString();
        }

        private class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
        {
            public bool Equals(PropertyInfo x, PropertyInfo y)
            {
                return x.Name + "Property" == y.Name || x.Name == y.Name + "Property";
            }

            public int GetHashCode(PropertyInfo obj)
            {
                throw new NotImplementedException();
            }
        }

        private static void WalkThroughProperties(DependencyObject node, Visitor visitor, bool hasChildren)
        {
            if (visitor.ShouldVisitPropertiesForNode(node))
            {
                var selfProps = (from property in node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  where visitor.ShouldVisitProperty(property.Name) &&
                                        visitor.ShouldVisitPropertyValue(property.Name,
                                            GetObjectProperty(node, property))
                                  orderby property.Name
                                  select property);

                var attachedPropInfo = (from property in node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static)
                                        where property.Name.EndsWith("Property") &&
                                              visitor.ShouldVisitProperty(property.Name.Substring(0, property.Name.Length - "Property".Length)) &&
                                              !selfProps.Contains(property, new PropertyInfoComparer()) &&
                                              typeof(DependencyProperty).IsAssignableFrom(property.PropertyType)
                                        orderby property.Name
                                        select property);
                
                var automationId = node.GetValue(AutomationProperties.AutomationIdProperty);

                foreach (var prop in selfProps)
                {
                    object value = null;
                    value = GetObjectProperty(node, prop);
                    visitor.VisitProperty(prop.Name, value);
                }

                foreach (var prop in attachedPropInfo)
                {
                    var attachedDP = prop.GetValue(null) as DependencyProperty;
                    var name = prop.Name.Substring(0, prop.Name.Length - "Property".Length);
                    var value = node.GetValue(attachedDP);
                    visitor.VisitProperty(name, value);
                }

                if (automationId != null)
                {
                    visitor.VisitProperty("AutomationId", automationId);
                }
            }
        }

        private static object GetObjectProperty(DependencyObject node, PropertyInfo property)
        {
            object value;
            try
            {
                value = property.GetValue(node);
            }
            catch (Exception e)
            {
                value = "Exception when reading " + property.Name + e.ToString();
            }

            return value;
        }

        private static DependencyObject[] GetChildren(DependencyObject node, Visitor visitor)
        {
            DependencyObject[] dos = new DependencyObject[VisualTreeHelper.GetChildrenCount(node)];
            for (int i = 0; i < dos.Length; i++)
            {
                dos[i] = VisualTreeHelper.GetChild(node, i);
            }
            return dos.Where((n) => visitor.ShouldVisitPropertiesForNode(n)).ToArray();
        }

        private static void WalkThroughTree(DependencyObject node, DependencyObject excludedNode, Visitor visitor)
        {
            if (node != null && visitor.ShouldVisitPropertiesForNode(node))
            {
                // Assume that if we have a UIElement, we'll have some properties
                var children = GetChildren(node, visitor);

                visitor.BeginVisitNode(node);

                WalkThroughProperties(node, visitor, children.Length != 0);
                if (children.Length != 0)
                {
                    visitor.BeginChildren();
                    for (int i = 0; i < children.Length; i++)
                    {
                        var child = children[i];
                        if (child != excludedNode)
                        {
                            WalkThroughTree(child, excludedNode, visitor);
                        }
                    }
                    visitor.EndChildren();
                }
                visitor.EndVisitNode(node);
            }
        }
    }
    
    /// <summary>
    /// The set of properties to report
    /// </summary>
    internal sealed class DefaultFilter
    {
        public IList<string> PropertyNameAllowList { get; set; }

        public DefaultFilter()
        {
            PropertyNameAllowList = new List<string>
            {
                "Foreground",
                "Background",
                "Padding",
                "Margin",
                "RenderSize",
                "Visibility",
                "CornerRadius",
                "BorderThickness",
                "Width",
                "Height",
                "BorderBrush",
                "VerticalAlignment",
                "HorizontalAlignment",
                "Clip",
                "FlowDirection",
                "Name",
                "Text",
                /*"ActualOffset" 19h1*/
            };
        }

        public bool ShouldVisitPropertyValue(string propertyValue)
        {
            return !string.IsNullOrEmpty(propertyValue) && !propertyValue.Equals("NaN") && !propertyValue.StartsWith("Exception");
        }

        public bool ShouldVisitProperty(string propertyName)
        {
            return (PropertyNameAllowList.Contains(propertyName));
        }
    }
    internal sealed class DefaultPropertyValueTranslator : IPropertyValueTranslator
    {
        public string PropertyValueToString(string propertyName, object propertyObject)
        {
            if (propertyObject == null)
            {
                return "[NULL]";
            }

            if (propertyObject is SolidColorBrush)
            {
                return (propertyObject as SolidColorBrush).Color.ToString();
            }
            else if (propertyObject is Size)
            {
                // comparing doubles is numerically unstable so just compare their integer parts
                Size size = (Size)propertyObject;
                int width = (int)size.Width;
                int height = (int)size.Height;
                return $"[{width}, {height}]";
            }
            return propertyObject.ToString();
        }
    }

    internal sealed class JsonPropertyValueTranslator : IPropertyValueTranslator
    {
        public string PropertyValueToString(string propertyName, object propertyObject)
        {
            if (propertyObject == null)
            {
                return "null";
            }
            else if (propertyObject is int || propertyObject is bool || propertyObject is double)
            {
                return propertyObject.ToString();
            }
            else if (propertyObject is SolidColorBrush)
            {
                return Quote((propertyObject as SolidColorBrush).Color.ToString());
            }
            else if (propertyObject is Size)
            {
                // comparing doubles is numerically unstable so just compare their integer parts
                Size size = (Size)propertyObject;
                return $"[{(int)size.Width}, {(int)size.Height}]";
            }
            else if (propertyObject is TextHighlighter)
            {
                var th = propertyObject as TextHighlighter;
                return $"{{\n\"Background\": {PropertyValueToString(null, th.Background)},\n\"Ranges\": {PropertyValueToString(null, th.Ranges)}\n}}\n";
            }
            else if (propertyObject is TextRange)
            {
                var tr = (TextRange)propertyObject;
                return new JsonObject
                {
                    { "StartIndex", JsonValue.CreateNumberValue(tr.StartIndex) },
                    { "Length", JsonValue.CreateNumberValue(tr.Length) },
                }.ToString();
            }
            else if (propertyObject is string)
            {
                return Quote(propertyObject.ToString());
            }
            else if (propertyObject is IEnumerable)
            {
                var sb = new StringBuilder();
                sb.Append("[");
                bool first = true;
                foreach (var i in (propertyObject as IEnumerable))
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        first = false;
                    }
                    sb.Append(PropertyValueToString(null, i));
                }
                sb.Append("]");
                return sb.ToString();
            }
            return Quote(propertyObject.ToString());
        }

        public static string Quote(string s)
        {
            s = s.Replace('\t', ' ').Replace("\n", @"\n");
            s = Regex.Replace(s, @"\p{Cs}", ""); // remove surrogate pairs e.g. emojis
            return '"' + s.Replace("\"", "\\\"") + '"';
        }
    }
}
