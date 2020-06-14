# XamlTreeDump
A UWP library to produce and compare XAML tree dumps useful for visual end-to-end testing

XamlTreeDump can be used to produce a data representation of a UWP XAML UI ("tree dump") and compare it to a checked-in version ("master") to detect regressions in visual output.

There are two main entrypoints to the library:
```cs
public static string VisualTreeDumper.DumpTree(
  DependencyObject root, 
  DependencyObject excludeNode,
  IVector<string> additionalProperties,
  DumpTreeMode mode)
```
This will traverse the tree starting at the root, skipping the optional `excludeNode`, and collect a set of standard properties - which you can add to through `additionalProperties`. The format of the dump can be switched between `Default` (a `key=value` text format), or the recommended `Json` format.

The string you receive from `DumpTree` can be saved to a file so that your test application can later use `TreeDumpHelper.DumpsAreEqual` to check for differences.

The second entrypoint:
```cs
public static bool TreeDumpHelper.DumpsAreEqual(string expected, string actual)
```
will perform a semantic comparison of the two dumps by following these rules:
1) any property with a string value of `<ANYTHING>` in the master will match any value in the output regardless of its type.
2) numbers are compared to within a precision of 1.0f
3) `DependencyObject`s with a `Visibility == Collapsed` will be compared as if they do not exist
4) `ScrollBar` types will be ignored (since UI automation can sometimes hover over them and trigger them on accident)

See [UnitTests](https://github.com/asklar/XamlTreeDump/tree/master/src/UnitTests) for more usage and examples.
