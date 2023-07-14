EnTT Sharp is a C# port of [EnTT for C++](https://github.com/skypjack/entt). EnTT is
a lightweight entity-component-system library with a focus on performance and cache
friendliness. 

This C# port roughly matches version 2.8 of the C++ EnTT library. It does not contain
many of the C++ specific helper functions (like type information systems or sane events, 
etc) which are part of the .NET runtime or can be easily added via external libraries. 
Referencing libraries is not as painful in C# as it apparently is in C++.

# Table of Contents

* [Introduction](#introduction)
  * [Code Example](#code-example)
  * [Motivation](#motivation)
  * [Performance](#performance)
* [Build Instructions](#build-instructions)
  * [Documentation](#documentation)
  * [Tests](#tests)
* [Packaging Tools](#packaging-tools)
* [EnTT in Action](#entt-in-action)
* [Contributors](#contributors)
* [License](#license)

# Introduction

The entity-component-system (also known as _ECS_) is an architectural pattern
used mostly in game development. For further details:

* [Entity Systems Wiki](http://entity-systems.wikidot.com/)
* [Evolve Your Hierarchy](http://cowboyprogramming.com/2007/01/05/evolve-your-heirachy/)
* [ECS on Wikipedia](https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system)

The EnTT wiki contains helpful 
* [EnTT C++ FAQ](https://github.com/skypjack/entt/wiki/Frequently-Asked-Questions)
* [EnTT C++ wiki](https://github.com/skypjack/entt/wiki)

This project is a faithful port of the C++ version of EnTT without any of the C++
specific helper systems needed in the original version. The EnTT system allows you
to define abstract entities, assign tags and components to those entities and finally
to iterate the entity and component pools in a fast and cache friendly way.

EnTTSharp has the following features:

* An incredibly fast **entity-component system** based on sparse sets, with its
  own _pay for what you use_ policy to adjust performance and memory usage
  according to the users' requirements.
* Works on POCO objects, no need to pollute your code with library interfaces.
* Optional, attribute-based automatic registration of components and serializers 
* 64-Bit Entity Keys, 2^31 entities per registry and 24 bits of additional data
  per key to allow partitioned key-spaces in distributed systems.
* Snapshop and state serialization support
* Compatible with .NET-Framework 4.7, .NET Core 2.0 (and later) and Unity's C# dialect.

## Code Example

        public readonly struct Position
        {
            public readonly double X;
            public readonly double Y;

            public Position(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public readonly struct Velocity
        {
            public readonly double DeltaX;
            public readonly double DeltaY;

            public Velocity(double deltaX, double deltaY)
            {
                DeltaX = deltaX;
                DeltaY = deltaY;
            }
        }

        public static void UpdatePosition(EntityRegistry registry, TimeSpan deltaTime)
        {
            // view contains all the entities that have both a position and a velocty component ...
            var view = registry.View<Velocity, Position>();
            foreach (var entity in view)
            {
                if (view.GetComponent(entity, out Position pos) &&
                    view.GetComponent(entity, out Velocity velocity))
                {
                    var posChanged = new Position(pos.X + velocity.DeltaX * deltaTime.TotalSeconds,
                                                  pos.Y + velocity.DeltaY * deltaTime.TotalSeconds);
                    registry.AssignComponent(entity, in posChanged);
                }
            }
        }

        public static void ClearVelocity(EntityRegistry registry)
        {
            var view = registry.View<Velocity>();
            foreach (var entity in view)
            {
                registry.AssignComponent(entity, new Velocity(0,0));
            }
        }

        public static void Main(string [] args)
        {
            var random = new Random();
            var registry = new EntityRegistry();
            registry.Register<Velocity>();
            registry.Register<Position>();

            for (int x = 0; x < 10; x += 1)
            {
                var entity = registry.Create();
                registry.AssignComponent<Position>(entity);
                if ((x % 2) == 0)
                {
                    registry.AssignComponent(entity, new Velocity(random.NextDouble(), random.NextDouble()));
                }
            }

            UpdatePosition(registry, TimeSpan.FromSeconds(0.24));
            ClearVelocity(registry);
        }

## Motivation

I ported EnTT-Sharp after looking though many of the existing C# based ECS libraries. 
The existing libraries (Entidas, SharpECS) are sadly not written with garbage-collector or cache friendliness 
in mind and proceed to define components and entities mandatorily as reference types. 

I do not want to be forced to pay the 20-bytes garbage collector enablement tax the .NET framework 
imposes on reference types. Unity's ECS system seems to have the right ideas, but after years of
working against Unity's bugs I have little trust left to bind myself exclusively to a Unity provided
implementation.

I ported EnTT to provide me with a fast way of defining entities and component systems to 
enable me to work in a more data driven way. The library is intentionally not bound to any game
library. The ECS architecture splits the update loop in distinct phases that are small enough to be
easily understood and can be individually debugged. 

Each system function works on a limited set of data and can be trivially unit tested. EnTT's 
design cleanly separates the game logic from the library. 

I have used this library successfully in both Unity and MonoGame projects. Testing its support for
Godot is on the roadmap, I suspect it will work out of the box.


# Build instructions

This library can be built via the standard build command

     dotnet build

The core EnTTSharp library has no external dependencies. 
A NuGet package can be created via 

     dotnet pack

Todo: Add release scripts.

## Documentation

Todo

## Tests

Todo

# Packaging Tools

ToDo. EnTT Sharp will be available via NuGet once it has some better tests
and documentation.

# Contributors

`EnTTSharp` was written initially as a faster alternative to other well known and
open source entity-component systems. Requests for features, PR, suggestions 
and feedback are highly appreciated.

If you find you can help me and want to contribute to the project with your
experience or you do want to get part of the project for some other reasons,
feel free to contact me directly (you can find the mail in the
[profile](https://github.com/rabbitstewdio)).<br/>
I can't promise that each and every contribution will be accepted, but I can
assure that I'll do my best to take them all seriously.

If you decide to participate, please see the guidelines for
[contributing](CONTRIBUTING.md) before to create issues or pull
requests.<br/>
Take also a look at the
[contributors list](https://github.com/rabbitstewdio/entt-sharp/blob/master/AUTHORS) to
know who has participated so far.

# License

C# code and documentation Copyright(c) 2020 Thomas Morgner<br/>
Original code and documentation Copyright (c) 2017-2020 Michele Caini.

Code released under
[the MIT license](https://github.com/skypjack/entt/blob/master/LICENSE).
Documentation released under
[CC BY 4.0](https://creativecommons.org/licenses/by/4.0/).<br/>
Logo released under
[CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).
