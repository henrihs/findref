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
  assemblyname                the name of the assembly to look for references to. Case insensitive, matches if the FullName is equal to the argument.

Options:
  -?|-h|--help                Show help information
  -d|--directory <DIRECTORY>  the root directory to search through (default: working directory)
  -r|--recursive              search directory recursively
  -v|--verbose                write verbose output to stdout
  -e|--regex                  use assemblyname argument as regex pattern

$ findref -v -d $REPOS/FindRef/src/bin/debug/netcoreapp2.1/ dnlib
Loading DLLs from 'src/bin/Debug/netcoreapp2.1/'
FindRef.dll (1.0.0.0) has a reference to dnlib (3.1.0.0)
```

### Run from source
```sh
$ git clone https://github.com/henrihs/findref.git
$ cd findref/src
$ dotnet run -- -r McMaster.Extensions.CommandLineUtils

FindRef.dll has a reference to McMaster.Extensions.CommandLineUtils
```

### Run using Docker

```shell
docker run --rm -it --volume="$PWD:/assemblies:ro" henrihs/findref [arguments] [options]
```

Example, to search for `NewtonSoft.Json` in the current directoy, use:

```shell
docker run --rm -it --volume="$PWD:/assemblies:ro" henrihs/findref -d . Newtonsoft.Json
```

#### Building the image locally

```shell
docker build -t henrihs/findref .
```
