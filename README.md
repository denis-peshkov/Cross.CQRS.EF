[![License](https://img.shields.io/badge/license-RPL%201.5-blue)](LICENSE.md)
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

Main Features:
* **Configurable Transaction Behavior**

  When added wrap every CommandHandler into EF Transaction, so only whole CommandHandler instructions will be commited or rejected.
  Flexible configuration of transaction behavior and isolation levels through DI system.
  Unified transaction handling with support for different transaction behavior.

* **Enhanced Transaction Control**

  Per-handler `[ExactTransaction(behavior, isolation)]` overrides global options. Opt-out: `[ExactTransaction(TransactionBehaviorEnum.NoBehavior)]` (replaces 8.4 `ExplicitTransaction`).
  Registration: `AddEntityFrameworkIntegration<TDbContext>` in namespace `Cross.CQRS.EF.Extensions` (after `AddCQRS`). Defaults: `TransactionalBehavior` and `Serializable`.

* **Pagination**

  Paging and query-filter APIs (`PaginationQuery`, `QueryableExtensions`, `IQueryableFilter`) were removed from this package in 9.0.0. Keep them in the host or another library.

* **Cross.CQRS dependency**

  Requires Cross.CQRS **11.1.2**. This package registers `EfLicenseProductInfo` (JWT type `Cross_CQRS_EF`).

* **.NET frameworks and Source Linking**.

  9.0.0 targets **net6.0–net10.0**. Source linking enabled and symbol package is published to nuget symbols server, making debugging easier.

**Supported frameworks:** .NET 6, .NET 7, .NET 8, .NET 9, .NET 10

Upgrade from 8.4.x: [`docs/BREAKING.md`](docs/BREAKING.md) · release notes: [`docs/CHANGELOG.md`](docs/CHANGELOG.md)

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

`SampleWebApp` is a Minimal API host: SQLite in-memory EF Core, one command with `SaveChanges`, and `[ExactTransaction]`. Canonical coverage of transaction behaviors is in `Cross.CQRS.EF.Tests` (clone the repository; the test project is not in the NuGet package).

## Roadmap:
- Add support for distributed transactions and SAGAs
- Implement transaction timeout configuration
- Add monitoring and metrics for transactions
- Provide more examples and documentation for transaction configurations
- ~~Add integration tests for different isolation levels~~
