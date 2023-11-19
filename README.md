[![Nuget](https://img.shields.io/nuget/v/Cross.CQRS.EF.svg)](https://nuget.org/packages/Cross.CQRS.EF/)

# Cross.CQRS.EF

Simple .NET MediatR base EF Transactional Behavior, Paginated Queries, Paginated Models.

Written on C#.

Main Features:
* **TransactionalBehavior and ExplicitTransaction**.

  When added wrap every CommandHandler into EF Transaction, so only whole CommandHandler instructions will be commited or rejected. To switch off this wrapper on specific CommandHandler have to use ExplicitTransaction attribute on the command handler.

* **PaginatedQueries and PaginatedModels**.

  These classes represent basic requirements to Paginated queries, models and so on.

* **.NET 6, .NET 7 and Source Linking**.

  From version 7.0 repository contains .NET 6 and .NET 7 projects.
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
- Refactor the Paginated Queries and Paginated Models.
