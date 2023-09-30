# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## 2023-09-30 - v1.0.0

    [???]     Initial commit of a minimally cleaned up version of EnTTSharp.

    [???]     Added XML serialisation via built in XML APIs (not fast, but readable) Added binary serialisation via MessagePack (fast, but not readable) Switched to T4 generation of repetitive code

    [???]     Reworked EntityRegistry to accept a TEntityKey generic parameter. That allows users of EntityRegistries to have separate registries with either additional key-identities without the threat of mixing up keys from one registry with another.

    [???]     WIP: Serialization.

    [???]     Normalized case of namespace across projects. Implemented FlagPool that stores membership of objects in a set (based on a type) without having to store the actual placeholder item for it.

    [???]     Fixed Case/Spelling of EnTT core package. Added Not<T> pool to create systems that work on missing bits of information.

    [???]     Pools are now readonly by default, but a query method exists for getting writable pools where needed. Added readonly Not<T> pool to entity registry.

    [???]     Validating that EntityKeys nested inside components can be written correctly.

    [???]     Validated binary bulk serialization.

    [???]     Validating that serialization works.

    [???]     Cleanup

    [???]     EntityKeys are now comparable for equality. EntityRegistry's Clear command did not clear ages of keys, causing troubles with basic unit testing. Tweaked EntitySystem setup code to hopefully make the generics less verbose.

    [???]     Added a flag initializer attribute.

    [???]     Flag registration fixes.

    [???]     Fixed some warnings.

    [???]     Removed some assumptions about context implementing certain interfaces. We are better off injecting dependencies than to rely on the context.

    [???]     Normalized T4 generation to not use a custom suffix to let Rider generate those things via its built-in T4 generator.

              Also removed those silly *.meta files.
              
    [???]     More work on test cases

    [???]     Fixed capacity update which could fail if the initial capacity was small.

    [???]     Reworked system creation to use a more verbose, but cleaner to read/write, builder supporting input and output parameters.

    [???]     Improve iteration performance.

    [???]     Essential Build files were ignored by a Unity specific exclude rule.

    [???]     Delete Unity garbage.

    [???]     Normalize how nuget pulls in NUnit.

    [???]     Add support for explict read-only ref access to document compiler checked safe usages. Complete non-context apply methods for system builder.

    [???]     Code cleanup

    [???]     Cleanup

    [???]     WIP: Cleanups; fixed failing tests after refactoring and fixing item placement logic.

    [???]     Fixing tests

    [???]     Using a cleaner error message.

    [???]     Regenerated files

    [???]     Fixing target framework property.

    [???]     Code cleanup to make release mode compile happy.

    [???]     Reworked serialization API to handle multiple entity keys per serialization.

              This wraps the generic delegate for key-lookup into a non-generic
              interface. That in return makes it possible for components of one
              entity type to reference entities of a different entity registry.
              (Enables actors to reference items, for instance)
              
    [???]     Reduce warnings by cleaning up obvious stuff.

    [???]     Removed unused parameter.

    [???]     Entity systems no longer produce raw actions and preserve some context information instead.

              The EntitySystemReference type can be implicitly converted into action delegates, but also
              records the inner system action for documentation and tracing purposes. I simply hate
              digging into the debugger internals to know what real method is called.
              
    [???]     Code gen change

    [Api]     Added more monad methods on Optional [Bug] Serialisation failed on optional types

    [Api]     Updated project sdks to latest LTS release. Tests use net6.0; libraries use netstandard2.1.

    [Bug]     Removing entries from a SparseSet/SparseDictionary/Pool created an inconsistent state.

    [Bug]     Tag writing was serializing to Optional<> instead of writing raw value

    [Build]   Add standard Nuke build and release scripts

    [Build]   Net6 changed the way tools are referenced in projects. Project has updated to the new method. T4 updated to 2.3.0

    [Build]   Adds Build Scripts as projects to solution.

              Use the "Build" configuration to actually run and compile those from
              within the IDE.
              
    [Cleanup] Added proper nullability attributes throughout the code. Raised target framework to netstandard 2.1.

    [Cleanup] Line ending fixes

    [Cleanup] - Defines line ending text mode for files via git attribute

    [Cleanup] - Removes invalid referenced project from solution

    [Cleanup] Silences warning of unused events

    [CleanUp] Fixed some warnings

    [CleanUp] Replaced Console.WriteLine statements with proper logging.

    [CleanUp] Cleaned up editor config

    [CleanUp] Nullable annotations

    [Feature] Updated EntitySystemReference to properly pretty-print closure method names.

    [Feature] Moved update events to readonly pool interface. Added new created event that carries over the component created. [Feature] Created new event to track entity creation and entity destruction.

    [Feature] Add support for derived values.

              Derived values are computed on the fly based on a given function.
              
## [vNext]
- Initial release
