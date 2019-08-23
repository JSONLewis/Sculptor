# Sculptor
 A [.NET Core](https://docs.microsoft.com/en-gb/dotnet/core/) based console application that works cross platform (Windows, Linux, OSX) as a static site generator.

**Warning:** this project is currently still being fleshed out. Everything is subject to change and / or exists in an un-ideal state. When I'm happy with the structure I'll add a version number and start documenting this in detail.

## Command Examples

* `sculptor create -n "MyFirstProject"` Valid command using the default name
* `sculptor create -n "MyFirstProject" -o "site"` Valid command using the non-default name
* `sculptor create -n` Invalid command with no value provided for the name
* `sculptor create -n "MyFirstProject" -o "site" --invalidoption` Invalid command option / argument
* `sculptor invalidverb -t` Invalid command / verb

For the purposes of debugging you can simply copy these into the Visual Studio arguments section in the Sculptor project settings. **Note:** that you will only need to copy the *arguments* from the snippets above - the `sculptor` part is uncessary.

## TODO:
* Look into acceptance / regression testing by running a full instance againts a known set of commands and output. Maybe comparing screenshots of the result or piping the output to verify?