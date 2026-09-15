# Changelog — Cross.CQRS

Newest releases first. GitHub releases: <https://github.com/denis-peshkov/Cross.CQRS/releases>

Breaking upgrade notes for NuGet consumers: [`BREAKING.md`](BREAKING.md).

---

## v11.1.2 — 14 Sep 2026

### CI / release process

- SonarCloud: `sonar.qualitygate.wait` only on `pull_request` (`QUALITY_GATE_WAIT`).
- NuGet pack: `denis-peshkov/update-nuspec-action@v2` before `nuget pack` (sync nuspec dependencies from the `.csproj`).
- Triage workflow also runs on `ready_for_review`.
- PR template: comments for the Changes list and Test plan.

### Versioning

- `GitVersion.yml` whitespace / line-ending normalize only; SemVer strategy unchanged.

### Documentation

- `CONTRIBUTING.md` rewritten for Cross.CQRS (branching, licensing, testing); no Identity leftovers.
- Readiness checklist renamed to `docs/RELEASE-PLAN-to-master.md` (CONTRIBUTING link updated).
- `docs/BREAKING.md` markdown table formatting only (no new consumer break).

### Repository tooling

- `release-plan-summary.mjs` renamed to `release-plan-to-master.mjs` (+ tests, `FINALIZE-REPLY.md` template).
- Triage: `flatten-paginated.mjs` for `gh api --paginate --slurp` (+ tests), wired into `post-pr-triage.mjs`.
- Skills / templates / `.coderabbit.yaml`: markdown table separator normalization (`|---|`).

---

## v11.1.1 — 13 Sep 2026

### CI / release process

- CI `.NET` workflow: GitVersion setup/execute upgraded to `gittools/actions` `@v4.7.0` (supports GitVersion 6.8.x).
- Create/Push git Tag only for **stable** SemVer on `master` / `release/*` / `hotfix/*` (`dev` never creates git tags; NuGet push on `dev` unchanged).
- SonarCloud: `sonar.projectKey` / `sonar.projectName` = `Cross.CQRS` (display name applies on main-branch analysis).
- Automated PR triage: full `base...head` scope (all commits), `fetch-depth: 0`, cumulative classification.

### Versioning

- `GitVersion.yml` (GV 6.x): `commit-message-incrementing: Disabled`; `main.increment: Inherit` from `release`/`hotfix`; root `increment: Patch` for orphaned `master`.
- Pull-request branch regex captures numeric PR id (`(?<Number>\d+)`) for GitHub Actions refs.

### Documentation

- `CONTRIBUTING.md` / README aligned with stable-only git tag policy and static `RPL 1.5` license badge.
- Root `ReleaseNotes.md` removed; release notes live in `docs/CHANGELOG.md`.

### Repository tooling

- `update-changelog.mjs` (+ tests): auto CHANGELOG from release delta; `--dry-run` wins; repo-agnostic `categorizePath`.
- CodeRabbit skill: pasted findings / explain → open C/H/M/L in current RELEASE-PLAN same turn.
- Removed obsolete `gitversion-strategy` skill (matrix runner); GitVersion policy remains in `GitVersion.yml`.

---

## v11.1.0 — 13 Sep 2026

### CI / release process

- Git tags (`vX.Y.Z`) are created only for **stable** SemVer (no pre-release suffix such as `-preview` / `-dev`).
- NuGet may still publish pre-release packages from eligible branches (`master` / `release/*` / `hotfix/*` / `dev`); those builds no longer create matching git tags.
- `CONTRIBUTING.md` updated to match the tag policy.

### Licensing

- Optional JWT license key on `CqrsServiceConfiguration`; `LicenseAccessor` exposes a cached `License` built from claims (subscription, user, edition, product type, dates).
- JWT compact-format pre-check (`IsValidJwtFormat`) before token validation; invalid shape logs a clear JWS/JWE error.
- `LicenseValidator.Validate(license, ILicenseProductInfo)` with default `LicenseProductInfo` (`Cross_CQRS` / `Cross_CQRS_EF`); extensions can register additional `ILicenseProductInfo` (e.g. Cross.CQRS.EF for stricter EF SKU rules).
- Validator messages use product metadata (`Company` / `Product` / `Site`) from `ILicenseProductInfo` (company text baked into the message template, not a separate structured log property).
- `CheckLicense` resolves all `ILicenseProductInfo` from DI and validates when `Product` is `"Cross.CQRS"` or `"Cross.CQRS.EF"`.
- `LicenseCheckBehavior` runs on every MediatR request; pipeline order **-2** for core license check, **-1** reserved for Cross.CQRS.EF.
- `InternalsVisibleTo` Cross.CQRS.EF (and Cross.CQRS.Tests) for shared licensing / internals.
- Dependency: `Microsoft.IdentityModel.JsonWebTokens` for JWT validation.

### Registration and MediatR pipeline

- New registration API: `services.AddCQRS(cfg => { … })` with `CqrsServiceConfiguration` (`LicenseKey`, `RegisterFromAssemblies`, `RegisterFromAssemblyContaining<T>`).
- Returns `CqrsRegistrationSyntax` / `BehaviorCollection` for ordered custom pipeline behaviors.
- FluentValidation: `AddValidatorsFromAssemblies` uses the **full** assembly set from `CqrsServiceConfiguration` (handlers, validators, filters stay aligned).
- `AsyncRequestHandlerBase` removed; `CommandHandler<TCommand>` implements `IRequestHandler<TCommand>` (MediatR `Unit`) directly.
- Library awaits use `ConfigureAwait(false)` (CA2007 addressed in library code).

### Target frameworks and dependencies

- Libraries: `netstandard2.1`, `net6.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0`.
- Microsoft.Extensions.Configuration / Logging versions vary by TFM (8.x on netstandard2.1 and net6–8, 9.0.14 on net9.0, 10.0.5 on net10.0), matching the project file.

### Solution, packaging, and repo

- Solution format `Cross.CQRS.slnx` replaces `Cross.CQRS.sln`.
- NuGet metadata in `Cross.CQRS/config.nuspec` with dependency groups per TFM; legacy `_nuget` scripts folder removed.
- `LICENSE.md` replaces plain `LICENSE`; README updated for `AddCQRS` configuration, licensing, and TFMs.
- Docs: `docs/BREAKING.md`, `docs/CHANGELOG.md` (canonical release notes; root `ReleaseNotes.md` points here), release plans, `docs/TO-DO.md`, `CONTRIBUTING.md`.
- GitHub: issue/PR templates, branch-policy / back-merge workflows, rulesets under `.github/rulesets/`, CodeRabbit config, issue/PR triage workflow and Cursor triage skills (`.cursor/triage`, `.cursor/skills/triage*`).
- GitVersion.yml and `.NET` CI workflow updated (incl. `netcoreapp3.1` / multi-TFM test matrix where configured).

### Tests

- Cross.CQRS.Tests migrated to **NUnit**; coverage expanded and organized by zone (Licensing / Registration / Behaviors / Queue / Core / Coverage).
- Test TFMs: `netcoreapp3.1` (exercises the library’s `netstandard2.1` asset), plus `net6.0`–`net10.0`; `SkipNetCoreApp31Tests` skips 3.1 when the host has no x64 3.1 runtime (e.g. Apple Silicon).
- `IsExternalInit` shim for `netcoreapp3.1` record/`init` support in tests.

---

## v10.1.4 — 10 Sep 2025

1. Logging improvements:
   - Enhanced log message formatting (changed “for a {Elapsed} ms” to “in {Elapsed} ms”)
   - Optimized work with Stopwatch
   - Improved internal logging
2. Dependency updates:
   - FluentValidation upgraded to version 11.11.0
3. Architectural changes:
   - Removed `AsyncRequestHandlerBase` class
   - `CommandHandler<TCommand>` now directly implements `IRequestHandler<TCommand>`
4. Code optimizations:
   - Simplified some methods
   - Improved code formatting
   - Refactoring for cleaner architecture

---

## v10.1.3 — 14 Apr 2025

- Fix issue with logging of Command, EventCommand and Query objects.
- Optimize work with Stopwatch.
- Improve internal logging.

---

## v10.1.2 — 11 Apr 2025

- Fix log value for Command, Event, Query.

---

## v10.1.1 — 17 Mar 2025

- Fix log field name.

---

## v10.1.0 — 13 Mar 2025

- Make `ObjectExtensions.GetObjectSize()` safe for some cases.

---

## v10.0.1 — 13 Mar 2025

- Added `ILogger` into `CommandEventHandler`, `CommandHandler`, and `QueryHandler`.

---

## v9.2.0 — 09 Mar 2025

- Add `ILogger` on CommandEvent processing.

---

## v9.1.2 — 09 Mar 2025

- Hotfix / add some examples.

---

## v9.1.1 — 09 Mar 2025

- Add SonarCloud.

---

## v9.1.0 — 04 Mar 2025

- Added `CommandEventTypeEnum` to define a type of Event, which should process event inside or outside of a transaction.
- Renamed EventQueueProcessBehavior → CommandEventQueueProcessBehavior, EventHandler → CommandEventHandler, EventQueue → CommandEventQueue, IEvent → ICommandEvent, IEventQueue → ICommandEventQueue, IEventQueueReader → ICommandEventQueueReader, IEventQueueWriter → ICommandEventQueueWriter.

---

## v8.3.1 — 23 Feb 2025

- Small internal refactoring.
- Fix build pipeline.

---

## v8.3.0 — 03 Jul 2024

- Set the `RequestFilterBehavior` before the `ValidationBehavior`.

---

## v8.2.2 — 16 May 2024

- Fix pipeline.

---

## v8.2.1 — 16 May 2024

- Add autobuild/push NuGet packages.

---

## v8.2.0 — 16 Apr 2024

- Added `IRequestFilter` with Behaviour to filter request of query or command.
- Added `IResultFilter` with Behaviour to filter result of query or command.

---

## v8.1.2 — 3 Apr 2024

- Added `NoRegisterAutomaticallyAttribute` to avoid register instance of class automatically in DI.
- Small fixes.
- Added sample project.

---

## v8.0.1 — 24 Nov 2023

- Fixed unhandled exception on .NET 8 (`System.TypeLoadException` when resolving MediatR `IRequestHandler<TRequest,TResponse>` for command handlers).
- Upgrade packages.

---

## v8.0.0 — 18 Nov 2023

- Added support for .NET 8.

---

## v7.0.0 — 18 Nov 2023

- Removed `PaginatedQuery` as useless.
- Updated version to correlate with .NET version.

---

## v1.0.1 — 13 Nov 2023

- Readme updated.
- Icon updated.

---

## v1.0.0 — 31 Oct 2023

- Release.

---

## v0.6.0 — 4 July 2023

- Added Net 7.0 targeted libraries.

---

## v0.5.0 — 2 June 2023

- Added Net 6.0 targeted libraries.

---

## v0.4.0 — 12 May 2023

- Package updates.

---

## v0.3.0 — 23 Aug 2022

- Bugfixes.

---

## v0.2.0 — 01 Jan 2022

- Added NetStandard 2.0 targeted libraries.

---

## v0.1.0 — 01 Sep 2021

- Initial version.
