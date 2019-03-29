[![NuGet](https://img.shields.io/nuget/vpre/FindRef.svg?maxAge=2592000)](https://www.nuget.org/packages/FindRef)
[![Build Status](https://dev.azure.com/hhe0094/FindRef/_apis/build/status/henrihs.findref.ci?branchName=master)](https://dev.azure.com/hhe0094/FindRef/_build/latest?definitionId=1&branchName=master)

# FindRef - find those references!


`findref` is a simple dotnet global tool designed for searching through a directory of DLLs after a given assembly reference. It supports searching through assemblies targeting `netstandard`, `netcoreapp` or `.NET Framework`

### Installing

Prerequisite: [.NET Core SDK >= 2.1](https://dotnet.microsoft.com/download)

```
$ dotnet tool install --global findref
```

### Usage
```
$ findref -h
Usage:  [arguments] [options]

Arguments:
  assemblyname                the name of the assembly to look for references to

Options:
  -?|-h|--help                Show help information
  -d|--directory <DIRECTORY>  The root directory to search through (default: working directory)
  -r|--recursive              search directory recursively
  -v|--verbose                write verbose output to stdout
  -i|--include-unmatched      include unmatched search results in the output

$ findref -v -d $REPOS/FindRef/src/bin/debug/netcoreapp2.1/ dnlib
Loading DLLs from 'src/bin/Debug/netcoreapp2.1/'
+ FindRef.dll has a reference to dnlib, Version=3.1.0.0, Culture=neutral, PublicKeyToken=50e96378b6e77999
```

### Run from source
```sh
$ git clone https://github.com/henrihs/findref.git
$ cd findref/src
$ dotnet run -- -r McMaster.Extensions.CommandLineUtils

+ FindRef.dll has a reference to McMaster.Extensions.CommandLineUtils
```
