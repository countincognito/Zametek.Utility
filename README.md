# Zametek.Utility

[![CI](https://github.com/countincognito/Zametek.Utility/actions/workflows/ci.yml/badge.svg)](https://github.com/countincognito/Zametek.Utility/actions/workflows/ci.yml)
[![NuGet Version](https://img.shields.io/nuget/v/Zametek.Utility.svg)](https://www.nuget.org/packages/Zametek.Utility "NuGet Version")

The package has utility classes for .NET Standard 2.0. They include comparers that take lambdas, ambient and tracking contexts, safe enum strings, and type and value switches. They also include a collection that gives tasks in the order in which they complete. Extension methods cover conversions, dates, enums, GUIDs, lists and types.

## Release notes

### 3.0.1

A fix to `OrderedTaskCollection<T>`, newer dependencies, and symbols for the package. The API of 3.0.0 does not change.

- **`OrderedTaskCollection<T>` gives the tasks in the order in which they complete.** A continuation from the queue of the thread pool recorded each completed task. When the pool was busy, a task could come after a task that completed later. This was also possible for a task that was complete before the collection was made. The collection now records each task on the thread that completes it.

- **The constructor reads the sequence of tasks once.** It read the sequence twice. With a lazy sequence, the collection then watched other tasks than the ones that it waited for, and the enumeration did not stop.

- **The code that awaits a task of the collection runs later.** Before, it could run inside the lock of the collection, on the thread that completed a task.

- **Dependencies.** Newtonsoft.Json 13.0.4 (was 13.0.3) and System.ValueTuple 4.6.2 (was 4.5.0).

- **Symbols.** A symbol package (`.snupkg`) with Source Link comes with the package. The paths in the symbols start with `/_/`, and not with the folders of the machine that built the package.
