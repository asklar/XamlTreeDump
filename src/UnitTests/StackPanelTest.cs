﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class StackPanelTest
    {
        private static IList<string> extraProps = new List<string>() { "IsChecked" };

        [TestMethod]
        public void AnythingTag()
        {
            var dump = Helper.GetDump(() =>
            {
                var sp = new StackPanel();
                CheckBox cb = new CheckBox() { IsChecked = true };
                sp.Children.Add(cb);
                return sp;
            }, extraProps);
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
  'children': '<ANYTHING>'
}".Replace("'", "\"");
            var r = TreeDumpLibrary.TreeDumpHelper.DumpsAreEqual(expected, dump);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void Collapsed()
        {
            var dump = Helper.GetDump(() =>
            {
                var sp = new StackPanel();
                CheckBox cb = new CheckBox() { IsChecked = true, Visibility = Windows.UI.Xaml.Visibility.Collapsed };
                sp.Children.Add(cb);
                return sp;
            }, extraProps);
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
  'children': []
}".Replace("'", "\"");
            var r = TreeDumpLibrary.TreeDumpHelper.DumpsAreEqual(expected, dump);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void CollapsedAndVisible()
        {
            var dump = Helper.GetDump(() =>
            {
                var sp = new StackPanel();
                CheckBox cb = new CheckBox() { IsChecked = true, Visibility = Windows.UI.Xaml.Visibility.Collapsed };
                sp.Children.Add(cb);
                cb = new CheckBox() { IsChecked = true };
                sp.Children.Add(cb);
                return sp;
            }, extraProps);
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
    'IsChecked': true,
    'Margin': '0,0,0,0',
    'Padding': '0,0,0,0',
    'RenderSize': [0, 0],
    'VerticalAlignment': 'Stretch',
    'Visibility': 'Visible'
  }
  ]
}
".Replace("'", "\"");
            var r = TreeDumpLibrary.TreeDumpHelper.DumpsAreEqual(expected, dump);
            Assert.IsTrue(r);
        }


        [TestMethod]
        public void ByString()
        {
            var dump = Helper.GetDump(() =>
            {
                var sp = new StackPanel();
                CheckBox cb = new CheckBox() { IsChecked = true };
                sp.Children.Add(cb);
                return sp;
            }, extraProps);
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
    'IsChecked': true,
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
