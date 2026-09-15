# Changelog — Cross.CQRS.EF

Newest releases first. Published versions: [GitHub Releases](https://github.com/denis-peshkov/Cross.CQRS.EF/releases).

Breaking upgrade notes for NuGet consumers: [`BREAKING.md`](BREAKING.md).

---

## v8.4.1 — 9 Mar 2025

### CI / release process

- SonarCloud scan step added to `.github/workflows/dotnet.yml` (`sonarsource/sonarcloud-github-action@v5`, project key `Cross.CQRS.EF`).
- Runner pinned to `ubuntu-22.04`; checkout `fetch-depth: 0` for better analysis relevance.

---

## v8.4.0 — 13 Aug 2024

### Transactions

- Added `TransactionalScopeBehavior` and `NoBehavior` to `TransactionBehaviorEnum`.
- Registration via `AddEntityFrameworkIntegration` updated for the new modes.

---

## v8.3.0 — 10 Aug 2024

### Transactions

- Added `ScopeBehavior` pipeline behavior and enum value.
- `AddEntityFrameworkIntegration` accepts transaction behavior selection.

### Samples

- SampleWebApp extended with scope command/event handlers (`SomeScopeExternal*` / `SomeScopeInternal*`).

---

## v8.2.1 — 16 May 2024

### CI / release process

- Added `.github/workflows/dotnet.yml` (build, test, NuGet pack/push).
- Added `GitVersion.yml` for SemVer in CI.

### Solution

- Solution items updated for GitVersion / CI assets.

---

## v8.2.0 — 16 Apr 2024

### Dependencies

- Upgraded package versions (library `.csproj` / nuspec).

---

## v8.1.1 — 3 Apr 2024

### Dependencies

- Upgraded package versions.

### Library

- XML docs on `AddEntityFrameworkIntegration`; small registration/docs fixes.

### Samples

- Added `SampleWebApp` (Minimal API host + EF `Context`, query/validator examples).

### Solution

- `SampleWebApp` included in the solution.

---

## v8.0.1 — 24 Nov 2023

### Dependencies

- Upgraded package versions (library `.csproj` / nuspec).

---

## v8.0.0 — 23 Nov 2023

### Target frameworks

- Added support for .NET 8 (`net8.0` TFM in library and packaging).

### Documentation

- README / release notes updated for .NET 8 support.

---

## v7.0.0 — 19 Nov 2023

### Packaging

- Initial NuGet line aligned with .NET 7: library, nuspec (`_nuget`), icon, LICENSE, README.
