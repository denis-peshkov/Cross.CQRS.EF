# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `9.0.0` · [`release/9.0.0-Transaction-Configuration-and-Behavior-Refactor`](https://github.com/denis-peshkov/Cross.CQRS.EF/tree/release/9.0.0-Transaction-Configuration-and-Behavior-Refactor) · [`RELEASE-PLAN-9.0.0.md`](RELEASE-PLAN-9.0.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-16
> **HEAD:** `7bb6a15` · PR [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) → `master`

**Change summary:** **22** items — ✅ **16** (73%) · 🟨 **2** (9%) · ⬜ **4** (18%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | ✅ GitVersion `MajorMinorPatch` = `9.0.0`; git tag `v9.0.0` нет |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-9.0.0.md`](RELEASE-PLAN-9.0.0.md) (`closed`; Open C/H/M/L пустые) |
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
| Q1 | `dotnet build` Release | ✅ локально + CI `.NET` build на `7bb6a15` (net6–net10) |
| Q2 | `dotnet test` Release | ✅ локально 65 passed / 3 skipped × 5 TFM; CI `Run tests` SUCCESS |
| Q3 | CI `.NET` green on release branch | ✅ PR #9 + push: `.NET` build SUCCESS на `7bb6a15` |
| Q4 | SonarCloud / quality gate | ✅ SonarCloud Code Analysis SUCCESS (`7bb6a15`); CodeQL SUCCESS |
| Q5 | SampleWebApp smoke | 🟨 `dotnet build` Release 0 errors; в `Cross.CQRS.EF.slnx`. HTTP smoke нет (нет `AddDbContext`, sample не host) |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ id/TFMs net6–net10 = `.csproj`; Cross.CQRS 11.1.2; files `LICENSE.md` / README / `icon.png` |
| N2 | Secrets `NUGET_API_KEY`, `TAGTOKEN` | ✅ имена есть (`NUGET_API_KEY`, `TAGTOKEN`, `SONAR_TOKEN`); значения не прогонялись на stable push |
| N3 | Tag + NuGet push from CI | ⬜ нет git tag / GitHub Release `v9.0.0`; nuget.org: `9.0.0-preview.*`, latest `9.1.0`, **нет** `9.0.0` |
| N4 | GitHub Release notes | ⬜ `gh release view v9.0.0` — not found |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after land; workflow есть на этой ветке, на `origin/master` ещё нет (404) |
| A2 | Hosts on Cross.CQRS **11.1.2** + этот пакет | 🟨 пакет уже зависит от Cross.CQRS 11.1.2; апгрейд хостов — после NuGet `9.0.0` |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | ⬜ owner decision; PR #9 `mergeable=true`, `mergeable_state=blocked` (0 reviews, Protect master) |
| G2 | Publish blockers cleared | ✅ version plan C/H/M/L пустые; CI .NET / Sonar / CodeQL / CodeRabbit на `7bb6a15` green |

- **Date:** 2026-09-16
- **Notes:** Breaking `8.4.1` → `9.0.0` задокументирован. Код к merge готов по CI; merge в `master` ждёт ruleset/review (G1). Stable tag/NuGet `9.0.0` ещё нет (N3/N4). На nuget.org уже есть pre-release `9.0.0-preview.*` и пакет `9.1.0` — не путать со stable `9.0.0`.
