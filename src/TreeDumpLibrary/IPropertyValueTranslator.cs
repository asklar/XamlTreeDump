// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace TreeDumpLibrary
{
    /// <summary>
    /// Translates values into a serialized format
    /// </summary>
    internal interface IPropertyValueTranslator
    {
        /// <summary>
        /// Converts a property name and value to a serialization format
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyObject"></param>
        /// <returns></returns>
        string PropertyValueToString(string propertyName, object propertyObject);
    }
}