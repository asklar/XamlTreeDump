// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;

namespace TreeDumpLibrary
{
    /// <summary>
    /// Abstracts output formats for a tree logger
    /// </summary>
    internal interface IVisualTreeLogger
    {
        /// <summary>
        /// Called when starting a node visit
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="nodeName"></param>
        /// <param name="obj"></param>
        void BeginNode(int indent, string nodeName, DependencyObject obj);
        /// <summary>
        /// Called when ending a node visit
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="nodeName"></param>
        /// <param name="obj"></param>
        void EndNode(int indent, string nodeName, DependencyObject obj);
        /// <summary>
        /// Called to log a property in a node
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        void LogProperty(int indent, string propertyName, object propertyValue);
        /// <summary>
        /// Gets the output data
        /// </summary>
        /// <returns></returns>
        string ToString();
        /// <summary>
        /// Called when starting an array
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="propertyName"></param>
        void BeginArray(int indent, string propertyName);
        /// <summary>
        /// Called when ending an array
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="propertyName"></param>
        void EndArray(int indent, string propertyName);
    }
}
