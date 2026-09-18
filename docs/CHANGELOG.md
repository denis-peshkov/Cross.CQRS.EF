# Changelog — Cross.CQRS.EF

Newest releases first. Published versions: [GitHub Releases](https://github.com/denis-peshkov/Cross.CQRS.EF/releases).

Breaking upgrade notes for NuGet consumers: [`BREAKING.md`](BREAKING.md).

---

## v10.0.0 — 18 Sep 2026

### Dependencies

- Bumped Cross.CQRS to **11.3.1** (`PackageReference` + all `config.nuspec` groups; closes #H26).

### Versioning

- `GitVersion.yml` `next-version: 10.0.0` (major after `9.2.2`; planned unpublished `9.3.0` / patch `9.2.3` folded into this release).

### Samples / Tests

- SampleWebApp and test modules: commands, queries, and events are `sealed record` types.

### Documentation

- Added version plans `9.2.1` / `9.2.2` / `10.0.0`; CHANGELOG for shipped `9.2.1` / `9.2.2`.
- Consumer notes: `docs/BREAKING.md` **From 9.2.2 to 10.0.0**.

### Repository tooling

- GitVersion resolve uses `/nofetch` + `MajorMinorPatch` (avoids remote-fetch hang).
- release-plan / pr-message: local `dotnet test` note (`SkipNetCoreApp31Tests` only when the test csproj defines it); repo-agnostic build/test discovery.
- Cursor README, CQRS request-filter wording, changelog/triage unit tests aligned to generic paths.

---

## v9.2.2 — 18 Sep 2026

### Dependencies

- Bumped `Cross.CQRS` PackageReference to `11.2.0` (nuspec still lagged at `11.1.2` until `10.0.0`).

### CI / release process

- NuGet push authenticates via `NuGet/login` OIDC instead of `NUGET_API_KEY`.
- Added GitHub labels snapshot `.github/LABELS.yml` / `.github/LABELS.md`.

### Repository tooling

- PR triage label apply / comment template and `pr-message` skill updates.

---

## v9.2.1 — 16 Sep 2026

### Tests

- Added `IsolationLevelIntegrationTests` and `ExactTransaction` probes: commit/rollback, Scope/TransactionalScope ambient isolation, overrides across ReadUncommitted–Serializable.

### Documentation

- README: struck through the wiki TODO for isolation-level integration tests.

---

## v9.2.0 — 16 Sep 2026

### Dependencies

- Bumped EF Core / Relational to latest stable per TFM: net8 `8.0.31`, net9 `9.0.20`, net10 `10.0.12` (net6/net7 unchanged). `config.nuspec` groups match.

### Packaging

- Added `Microsoft.SourceLink.GitHub` `1.1.1` (`PrivateAssets=All`) for symbol/source linking.
- `config.nuspec` `releaseNotes` contain only CHANGELOG and BREAKING links (no inline feature summary).

### Versioning

- `GitVersion.yml` `next-version: 9.2.0`.

### Tests

- Aligned test package versions (`6.0.36` on net6); dropped `Async` suffix on test method names; `TransactionLockTests` isolation/WAL/TCS; writer `BEGIN EXCLUSIVE` without nested EF transactions.

### Samples

- `SampleWebApp`: SQLite in-memory EF Core, `SaveChanges` + `[ExactTransaction]`, POST/GET `/somescope`; `ImplicitUsings` disabled (`GlobalUsings.cs`).

### Documentation

- README license badge RPL 1.5; SampleWebApp described as Minimal API host. CONTRIBUTING: `Licensing` folder in repo scope.

### Repository tooling

- `.cursor/rules` dedupe (structured logging, secrets, readonly deps).
- `.cursor/skills/pr-message`; agent Shell calls require `required_permissions: ["all"]`.

---

## v9.0.0 — 15 Sep 2026

### Transactions

- Replaced `TransactionalBehavior` / `ScopeBehavior` / `TransactionalScopeBehavior` with a single `UnifiedTransactionBehavior` (pipeline order 10).
- Renamed `ExplicitTransactionAttribute` (empty opt-out marker) to `ExactTransactionAttribute(behavior, isolation)`.
- `AddEntityFrameworkIntegration` moved to namespace `Cross.CQRS.EF.Extensions` and accepts `isolationLevel` (default `Serializable`).
- `TransactionBehaviorOptions` defaults match the extension: `TransactionalBehavior` and `Serializable`.

### Pagination

- Removed `PaginationQuery` / `PaginationQueryHandler`, pagination models (`PaginationRequest`, `PaginationResult`, `SortParameter`, `SortDirectionEnum`), `QueryableExtensions`, and `IQueryableFilter` assembly scan.

### Licensing

- Package depends on Cross.CQRS **11.1.2**.
- Registers `EfLicenseProductInfo` (JWT type `Cross_CQRS_EF`).

### Target frameworks

- Added `net9.0` and `net10.0`; EF Core versioned per TFM (`net6.0`–`net10.0`).

### Tests

- Added `Cross.CQRS.EF.Tests` (handler fixtures, transaction/lock cases, licensing registration).

### CI / release process

- `GitVersion.yml` `next-version: 9.0.0`; branch-policy, back-merge, and triage workflows; GitHub issue/PR templates and rulesets.

### Packaging

- Solution `Cross.CQRS.EF.sln` → `Cross.CQRS.EF.slnx`; pack via `Cross.CQRS.EF/config.nuspec` (`_nuget/` helpers removed).

### Documentation

- Consumer notes in `docs/CHANGELOG.md` and `docs/BREAKING.md` (`8.4.1` → `9.0.0`); `CONTRIBUTING.md`.

### Repository tooling

- Maintainer kit: `.cursor` rules / skills / triage, CodeRabbit config.

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
