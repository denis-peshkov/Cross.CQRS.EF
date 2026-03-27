[![License](https://img.shields.io/github/license/denis-peshkov/Cross.CQRS.EF)](LICENSE)
[![GitHub Release Date](https://img.shields.io/github/release-date/denis-peshkov/Cross.CQRS.EF?label=released)](https://github.com/denis-peshkov/Cross.CQRS.EF/releases)
[![NuGetVersion](https://img.shields.io/nuget/v/Cross.CQRS.EF.svg)](https://nuget.org/packages/Cross.CQRS.EF/)
[![NugetDownloads](https://img.shields.io/nuget/dt/Cross.CQRS.EF.svg)](https://nuget.org/packages/Cross.CQRS.EF/)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Cross.CQRS.EF&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Cross.CQRS.EF)
[![issues](https://img.shields.io/github/issues/denis-peshkov/Cross.CQRS.EF)](https://github.com/denis-peshkov/Cross.CQRS.EF/issues)
[![.NET PR](https://github.com/denis-peshkov/Cross.CQRS.EF/actions/workflows/dotnet.yml/badge.svg?event=pull_request)](https://github.com/denis-peshkov/Cross.CQRS.EF/actions/workflows/dotnet.yml)

![Size](https://img.shields.io/github/repo-size/denis-peshkov/Cross.CQRS.EF)
[![GitHub contributors](https://img.shields.io/github/contributors/denis-peshkov/Cross.CQRS.EF)](https://github.com/denis-peshkov/Cross.CQRS.EF/contributors)
[![GitHub commits since latest release (by date)](https://img.shields.io/github/commits-since/denis-peshkov/Cross.CQRS.EF/latest?label=new+commits)](https://github.com/denis-peshkov/Cross.CQRS.EF/commits/master)
![Activity](https://img.shields.io/github/commit-activity/w/denis-peshkov/Cross.CQRS.EF)
![Activity](https://img.shields.io/github/commit-activity/m/denis-peshkov/Cross.CQRS.EF)
![Activity](https://img.shields.io/github/commit-activity/y/denis-peshkov/Cross.CQRS.EF)

# Cross.CQRS.EF

Simple .NET MediatR base EF Transactional Behavior.

Written on C#.

Main Features:
* **Configurable Transaction Behavior**

  When added wrap every CommandHandler into EF Transaction, so only whole CommandHandler instructions will be commited or rejected.
  Flexible configuration of transaction behavior and isolation levels through DI system.
  Unified transaction handling with support for different transaction behavior.

* **Enhanced Transaction Control**

  The ExactTransaction attribute supports for custom isolation levels and transaction behavior configuration on both global and per-handler basis through the Attribute.
  To switch off transaction behavior wrapper on specific CommandHandler or change isolation level have to use ExactTransaction attribute on the command handler.

* **.NET frameworks and Source Linking**.

  From version 8.0 repository contains additional .NET 8 projects.
  From version 7.0 repository contains .NET 6 and .NET 7 projects.
  Source linking enabled and symbol package is published to nuget symbols server, making debugging easier.

**Supported frameworks:** .NET 6, .NET 7, .NET 8, .NET 9, .NET 10

## Install NuGet package

Install the _Cross.CQRS.EF_ [NuGet package](https://www.nuget.org/packages/Cross.CQRS.EF/) into your .NET project:

```powershell
Install-Package Cross.CQRS.EF
```
or
```bash
dotnet add package Cross.CQRS.EF
```

## Issues and Pull Request

Contribution is welcomed. If you would like to provide a PR please add some testing.

## How To's

Please use [Wiki](https://github.com/denis-peshkov/Cross.CQRS.EF/wiki) for documentation and usage examples.

### Complete usage examples can be found in the test project ###
Note - test project is not a part of nuget package. You have to clone repository.

## Roadmap:
- Add support for distributed transactions and SAGAs
- Implement transaction timeout configuration
- Add monitoring and metrics for transactions
- Provide more examples and documentation for transaction configurations
- Add integration tests for different isolation levels
