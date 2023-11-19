[![Nuget](https://img.shields.io/nuget/v/Cross.CQRS.EF.svg)](https://nuget.org/packages/Cross.CQRS.EF/)

# Cross.CQRS

Simple .NET MediatR base EF Transactional Behavior, Paginated Queries, Paginated Models.

Written on C#.

Main Features:
* **TransactionalBehavior and ExplicitTransaction**.

  When added wrap every CommandHandler into EF Transaction, so only whole CommandHandler instructions will be commited or rejected. To switch off this wrapper on specific CommandHandler have to use ExplicitTransaction attribute on the command handler.

* **PaginatedQueries and PaginatedModels**.

  Implemented base patterns to work with Commands. The Commands used to modify entities.

* **.NET Standard 2.1 and Source Linking**.

  From version 1.0 repository contains .NET Standard 2.0, .NET 6, .NET 7 and .NET 8 projects.
  Source linking enabled and symbol package is published to nuget symbols server, making debugging easier.

## Install with nuget.org:

https://www.nuget.org/packages/Cross.CQRS.EF

## Installation

Clone repository or Install Nuget Package
```
Install-Package Cross.CQRS.EF
```

## Issues and Pull Request

Contribution is welcomed. If you would like to provide a PR please add some testing.

## How To's

Please use [Wiki](https://github.com/denis-peshkov/Cross.CQRS.EF/wiki) for documentation and usage examples.

### Complete usage examples can be found in the test project ###
Note - test project is not a part of nuget package. You have to clone repository.

## Roadmap:
-
