# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `9.0.0` (shipped) · нет активного следующего · [`RELEASE-PLAN-9.0.0.md`](RELEASE-PLAN-9.0.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-16
> **HEAD:** `044589d` · PR [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) merged → `master`

**Change summary:** **22** items — ✅ **22** (100%) · 🟨 **0** (0%) · ⬜ **0** (0%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | ✅ GitVersion `next-version: 9.0.0`; git tag / GitHub Release [`v9.0.0`](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0) на `044589d` |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-9.0.0.md`](RELEASE-PLAN-9.0.0.md) (`published / closed`; Open C/H/M/L пустые) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags только `master` / `release/*` / `hotfix/*`; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ `From 8.4.1 to 9.0.0` |
| B2 | PR title `BREAKING:` where applicable | ✅ [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) `BREAKING: Unify EF transaction behavior…` |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ ссылки на CHANGELOG / BREAKING, без копипаста секций |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v9.0.0` |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | ✅ локально + CI `.NET` build на `044589d` (net6–net10) |
| Q2 | `dotnet test` Release | ✅ локально 65 passed / 3 skipped × 5 TFM; CI `Run tests` SUCCESS |
| Q3 | CI `.NET` green on release branch | ✅ PR #9 merged; `.NET` / tag / NuGet push SUCCESS на `044589d` |
| Q4 | SonarCloud / quality gate | ✅ SonarCloud Code Analysis SUCCESS; CodeQL SUCCESS |
| Q5 | SampleWebApp smoke | ✅ SQLite in-memory `AddDbContext` + `SaveChanges` + `[ExactTransaction]`; локальный HTTP POST/GET `/somescope` 200. CI HTTP нет |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ id/TFMs net6–net10 = `.csproj`; Cross.CQRS 11.1.2; files `LICENSE.md` / README / `icon.png` |
| N2 | Secrets `NUGET_API_KEY`, `TAGTOKEN` | ✅ CI использовал на stable push: tag `v9.0.0` + nuget.org `9.0.0` |
| N3 | Tag + NuGet push from CI | ✅ git tag / GitHub Release `v9.0.0`; nuget.org latest **`9.0.0`** (ошибочный `9.1.0` unlisted) |
| N4 | GitHub Release notes | ✅ [`v9.0.0`](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0) published 2026-09-16 |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ✅ workflow `Back-merge master to dev` SUCCESS (2026-09-16); `origin/dev` содержит `origin/master` |
| A2 | Hosts on Cross.CQRS **11.1.2** + этот пакет | ✅ NuGet `9.0.0` published (зависимость Cross.CQRS 11.1.2); апгрейд хостов — у потребителей |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | ✅ merge [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) → `master` 2026-09-16 (`044589d`) |
| G2 | Publish blockers cleared | ✅ version plan C/H/M/L пустые; tag + NuGet `9.0.0` + back-merge done |

- **Date:** 2026-09-16
- **Notes:** `9.0.0` shipped. Следующего version plan нет (`next-version` в GitVersion всё ещё `9.0.0` → следующий патч с `master` будет `9.0.1`). Q5: SampleWebApp — SQLite in-memory + `SaveChanges` + `[ExactTransaction]` (локальный HTTP smoke).
