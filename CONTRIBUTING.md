# Contributing to Cross.CQRS

Thank you for your interest in the project.

## Quick links

- [Report an issue](https://github.com/denis-peshkov/Cross.CQRS/issues/new/choose)
- [Open PRs](https://github.com/denis-peshkov/Cross.CQRS/pulls)
- [CI (.NET)](https://github.com/denis-peshkov/Cross.CQRS/actions/workflows/dotnet.yml)
- [CI (back-merge master → dev)](https://github.com/denis-peshkov/Cross.CQRS/actions/workflows/backmerge-master-to-dev.yml)
- [Branch policy](https://github.com/denis-peshkov/Cross.CQRS/actions/workflows/branch-policy.yml)
- [Triage](https://github.com/denis-peshkov/Cross.CQRS/actions/workflows/triage.yml)
- [SonarCloud](https://sonarcloud.io/summary/new_code?id=Cross.CQRS)
- [NuGet](https://www.nuget.org/packages/Cross.CQRS/)
- [README](README.md)
- [Release notes](docs/CHANGELOG.md)
- Breaking changes: [`docs/BREAKING.md`](docs/BREAKING.md)
- Release readiness: [`docs/RELEASE-PLAN-to-master.md`](docs/RELEASE-PLAN-to-master.md)
- Open backlog: [`docs/TO-DO.md`](docs/TO-DO.md)

---

## What is Cross.CQRS?

**Cross.CQRS** is a NuGet library for .NET MediatR-based CQRS:

- Queries / Commands / CommandEvents and pipeline behaviors;
- FluentValidation, request/result filters;
- optional JWT licensing (`CqrsServiceConfiguration.LicenseKey`, `ILicenseProductInfo`);
- fluent registration via `services.AddCQRS(cfg => …)`.

Consumers call `AddCQRS` and send requests through MediatR (`IMediator` / `ISender`).

---

## How you can help

| Type | Examples |
|---|---|
| **Report** | Bug with repro steps, expected/actual behavior, package version and TFM |
| **Fix** | Pipeline behavior regression, registration bug, licensing validation |
| **Build** | New filter/behavior, tests, SampleWebApp improvements |
| **Review** | PR review, especially licensing and DI registration |
| **Document** | README, `docs/CHANGELOG.md`, `docs/BREAKING.md`, release plans |

---

## Development principles

### Licensing and security first

Any change to licensing, pipeline behaviors, or DI registration is **high-priority review**. Do not log or commit license JWTs, private keys, or production secrets. Prefer placeholders in samples and issues. For security-related issues/PRs, include package version and a minimal repro — never real keys or live tokens.

### Public surface is a contract

Registration (`AddCQRS`), public types, and documented pipeline order are contracts for NuGet consumers. Breaking changes require an entry in `docs/BREAKING.md` only (`config.nuspec` `releaseNotes` links there and must not duplicate the list).

### Minimal diff

Do not mix refactoring, formatting untouched files, and a feature in one PR. Drive-by changes belong in a separate PR.

### Repository conventions

- `.editorconfig` — style source (UTF-8 BOM, CRLF, 4 spaces for `.cs`).
- `GlobalUsings.cs` — prefer shared usings; `ImplicitUsings` = `disable`.
- New `.cs` / `.csproj` / `.sln` / `.slnx` files — **UTF-8 with BOM**.
- Tests — **NUnit** + FluentAssertions; prefer method names `Given[X]_When[Y]_Then[Z]` (async → `…Async`); add/update tests with behavior changes.

---

## In scope / out of scope

### In scope

- `Cross.CQRS/` — library (commands, queries, events, behaviors, licensing, DI);
- `Cross.CQRS.Tests/` — unit / pipeline / licensing tests;
- `SampleWebApp/` — smoke host example;
- `README.md`, `docs/CHANGELOG.md`, `docs/BREAKING.md`, `Cross.CQRS/config.nuspec`;
- CI: `.github/workflows/dotnet.yml`, `branch-policy.yml`, `triage.yml`, `backmerge-master-to-dev.yml`.

### Out of scope (without maintainer discussion)

- Large architecture refactors “for aesthetics”;
- New external dependencies without a strong reason;
- Consumer-breaking changes without a `docs/BREAKING.md` entry;
- Secrets, keys, `.env` in commits.

---

## Branches and releases

```
                        ┌──── CI merge ─────────┐     (master → dev, no PR)
feature/* ──┐           ▼                       │
fix/*     ──┼── PR ──► dev ── merge ──► master ─┴─► NuGet + git tag
chore/*   ──┘                              ▲
                                           │
                                 release/* / hotfix/* (owner only)
```

| Branch | Purpose | Who |
|---|---|---|
| `master` | Stable release; GitVersion, **stable** git tag (`vX.Y.Z`), NuGet push | **Owner only** — direct push and PRs |
| `release/*` | Release preparation; NuGet (may be `-preview.*`); **no** git tag for pre-releases | **Owner only** — branch creation and push |
| `hotfix/*` | Urgent production patches; same tag/NuGet rules as `release/*` | **Owner only** — branch creation and push |
| `dev` | Feature integration; NuGet pre-release (`-dev.*`); **never** creates git tags | **Default PR target** for all contributors |
| `feature/*` | New functionality (build/test only — no tag/NuGet) | Contributors |
| `fix/*` | Bug fixes (build/test only — no tag/NuGet) | Contributors |
| `chore/*` | CI, deps, docs-only, maintenance (no tag/NuGet) | Contributors |

Git tags are created only for **stable** `X.Y.Z` (no `-` in `semVer`) on `master` / `release/*` / `hotfix/*`. **`dev` never creates git tags**; it may still push NuGet pre-release packages.

**Access rules (enforced in CI via `.github/workflows/branch-policy.yml`):**

- Contributors open PRs **only into `dev`** from `feature/*`, `fix/*`, or `chore/*`.
- PRs targeting **`master`** — repository owner only (`denis-peshkov`).
- Pushing to **`master`**, **`release/*`**, or **`hotfix/*`** — owner only.
- Release merge `dev` → `master`, tags, and NuGet publish — maintainer step after the release checklist.
- After changes land on **`master`**, CI (`.github/workflows/backmerge-master-to-dev.yml`) **merges `master` into `dev` and pushes** (no PR, no build wait) using the owner PAT secret **`TAGTOKEN`** — required to bypass Protect-dev (PR + `build`). Plain `GITHUB_TOKEN` is rejected (`GH013`). If there are conflicts, the job fails — resolve locally and push to `dev`.

Optional GitHub Rulesets: import recipes from [`.github/rulesets/`](.github/rulesets/).

Versioning: **GitVersion** (`GitVersion.yml`). `dev` is pre-release (`-dev.N`), not a release branch. `commit-message-incrementing: Disabled`.

---

## Branch naming

Prefix + kebab-case description:

| Prefix | When |
|---|---|
| `feature/` | New functionality |
| `fix/` | Bug fix |
| `chore/` | CI, deps, docs-only, maintenance |
| `release/` | Release preparation (owner) |
| `hotfix/` | Urgent production patch (owner) |

Examples:

```
release/11.0.0-short-name
hotfix/critical-license-check
feature/license-product-info-docs
fix/validation-behavior-order
chore/editorconfig-and-templates
```

---

## Commit messages

Use **clear English** messages in imperative/descriptive style:

```
Add LicenseCheckBehavior coverage tests
Fix FluentValidation scan for multiple assemblies
Update README licensing section
```

For breaking changes, explicitly include `BREAKING:` in the commit body or PR title/description.

`docs/CHANGELOG.md` is maintained by maintainers before release (not a contributor checklist item).

---

## Pull request process

### 1. Preparation

```bash
git checkout dev
git pull origin dev
git checkout -b feature/short-description
```

### 2. Changes

- Follow existing folder layout (`Behaviors/`, `Licensing/`, `Extensions/`, …).
- Do not touch unrelated files.
- Breaking change → `docs/BREAKING.md` only (nuspec keeps a link, not a duplicate list).

### 3. Tests (required)

See [Testing](#testing).

### 4. Open PR

- **Base branch:** `dev` (required for contributors)
- **Do not** open PRs into `master`, `release/*`, or `hotfix/*` unless you are the repository owner
- Description: what, why, how to verify (**English** — for GitHub history and the triage bot)
- For licensing / security — explicitly note risks
- Breaking consumer change → prefix the **PR title** with `BREAKING:`

### 5. CI

Must pass:

- `.NET` workflow (`dotnet build` + `dotnet test`)
- Branch policy (`.github/workflows/branch-policy.yml`) — contributors cannot PR to `master` or push `release/*` / `hotfix/*`
- SonarCloud quality gate on PR (`sonar.qualitygate.wait=true` when enabled)
- Triage PR comment job when enabled (`CURSOR_API_KEY`) — must not fail on large diffs

CodeRabbit (`.coderabbit.yaml`): comment `@coderabbitai full review` when a full pass is needed.

### 6. Review and merge

After approval — merge into `dev`. Release to `master` and NuGet publish is a separate maintainer step.

### One PR rule

**One PR = one feature or one fix.** Split large changes (library + tests → docs → CI).

---

## Testing

### Local run

```bash
dotnet build Cross.CQRS.slnx
dotnet test Cross.CQRS.Tests/Cross.CQRS.Tests.csproj
```

With coverage (OpenCover), as in CI:

```bash
dotnet test Cross.CQRS.Tests/Cross.CQRS.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

### Pre-PR checklist

- [ ] Tests added/updated for changed behavior (`Given[X]_When[Y]_Then[Z]`; async → `Async`)
- [ ] `dotnet build` / `dotnet test` — green locally
- [ ] For licensing / pipeline — not only happy path
- [ ] No secrets in code, samples, or test data
- [ ] README / `docs/BREAKING.md` / `config.nuspec` updated when the public surface changes

---

## Documentation

| What changed | Update |
|---|---|
| Public API / registration | `README.md` |
| Breaking change for consumers | `docs/BREAKING.md` only (`config.nuspec` `releaseNotes` = link, no duplicate list) |
| Released behavior | **`docs/CHANGELOG.md` (maintainers, on release work)** and short `config.nuspec` `releaseNotes` (+ link to BREAKING) |
| Packaging / dependencies | `Cross.CQRS/config.nuspec` |
| Release readiness | `docs/RELEASE-PLAN-*.md` |
| Deferred findings | `docs/TO-DO.md` |

---

## License

Code is under [RPL 1.5](LICENSE.md) (Reciprocal Public License). By contributing, you agree that derivative works are distributed under the same terms, or under a [Peshkov commercial license](https://peshkov.biz/license).

There is no separate CLA — merging a PR means agreement with the repository license.

---

## Questions?

- Bugs and features: [GitHub Issues](https://github.com/denis-peshkov/Cross.CQRS/issues)

**Thank you for contributing to Cross.CQRS.**
