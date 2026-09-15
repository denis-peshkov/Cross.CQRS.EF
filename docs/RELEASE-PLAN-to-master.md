# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `9.0.0` · [`release/9.0.0-Transaction-Configuration-and-Behavior-Refactor`](https://github.com/denis-peshkov/Cross.CQRS.EF/tree/release/9.0.0-Transaction-Configuration-and-Behavior-Refactor) · [`RELEASE-PLAN-9.0.0.md`](RELEASE-PLAN-9.0.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-15

**Change summary:** **22** items — ✅ **8** (36%) · 🟨 **1** (5%) · ⬜ **13** (59%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | ✅ `9.0.0` (GitVersion MajorMinorPatch; tag ещё нет) |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-9.0.0.md`](RELEASE-PLAN-9.0.0.md) (open C/H/M/L) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые (H8/H9 в version plan) |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ `From 8.4.1 to 9.0.0` |
| B2 | PR title `BREAKING:` where applicable | ⬜ PR ещё нет |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ ссылки на CHANGELOG / BREAKING |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v9.0.0` |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | ⬜ после H8 |
| Q2 | `dotnet test` Release | ⬜ после H8 / M10 |
| Q3 | CI `.NET` green on release branch | ⬜ tip CI |
| Q4 | SonarCloud / quality gate | ⬜ after tip CI |
| Q5 | SampleWebApp smoke | ⬜ после H8 |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | 🟨 TFMs/deps есть; publish после H8/H9 |
| N2 | Secrets `NUGET_API_KEY`, `TAGTOKEN` | ⬜ проверить перед publish |
| N3 | Tag + NuGet push from CI | ⬜ нет `v9.0.0` |
| N4 | GitHub Release notes | ⬜ для `v9.0.0` |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after land |
| A2 | Sibling packages consume core | ⬜ хосты: Cross.CQRS **11.1.2** + этот пакет |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст (работа дельты в version plan) |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | ⬜ |
| G2 | Publish blockers cleared | ⬜ H8 (DI options) до publish |

- **Date:** 2026-09-15
- **Notes:** Version plan `9.0.0` открыт. Breaking `8.4.1` → `9.0.0` задокументирован. Не публиковать, пока не закрыт **H8**.
