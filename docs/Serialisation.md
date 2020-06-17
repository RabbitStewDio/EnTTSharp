# Serialisation

EnTT allows to write the entire entity registry into an archive and
to later read back those archive entries to restore the state of the
registry.

In the original C++ sources, archives are treated as an abstract data
stream with minimal operations to get data in and out of the stream.
That archive system required that archive streams are read in exactly
the same order as they had been originally written. That makes archives
suitable for short term storage. This system does not deal well with
change and thus can cause troubles when used for save games (which 
may be transfered from older versions of a game to newer versions).

In C# we have a rich set of serialisation libraries that use the built
in reflection system to read and write arbitrary objects. 

## Stream format

In its most basic format, the stream supported by EnTT sharp must be
able to read 