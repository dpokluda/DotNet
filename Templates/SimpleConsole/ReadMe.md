# Simple Console Template

Simple console application demonstrating how to:
- configure dependency injection
- configure console logging
- configure program argument parsing
- use color console output

The following command will print **Name** and the output will contain additional debug information.
```sh
dotnet run -- --debug
# display help information 
dotnet run -- --help
```

# Further customizations

There are two versions of the program, one is using simpler [DragonFruit](https://github.com/dotnet/command-line-api/blob/main/docs/DragonFruit-overview.md) 
library for program argument parsing and another one is using the regular [System.CommandLine](https://github.com/dotnet/command-line-api) library.

The default `Program.cs` is using the simplified library and `ProgramWithCommand.cs` is using the full System.CommandLine library 
(for the full library you need to comment/uncomment the corresponding dependency in `csproj` project file).

# Resources
- [Dependency Injection and Logging in Console application](https://www.code4it.dev/blog/dependency-injection-config-logging-in-console-application/) by Davide Bellone