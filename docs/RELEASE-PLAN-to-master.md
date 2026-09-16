# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `9.2.0` · ветка `master` (ahead 10 of `origin/master`) · [`RELEASE-PLAN-9.2.0.md`](RELEASE-PLAN-9.2.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-16
> **HEAD:** local `master` · база `origin/master` = [`v9.0.0`](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0)

**Change summary:** **22** items — ✅ **10** (45%) · 🟨 **2** (9%) · ⬜ **10** (45%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | 🟨 план `9.2.0`; GitVersion `next-version: 9.0.0` принято (#H25); tag `v9.2.0` нет |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-9.2.0.md`](RELEASE-PLAN-9.2.0.md) (Open C/H/M/L пустые) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ нет API-break 9.0.0 → 9.2.0 (только EF patch deps) |
| B2 | PR title `BREAKING:` where applicable | ✅ N/A |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ ссылки на CHANGELOG / BREAKING (#L29 принято) |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v9.2.0` |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | ⬜ после push |
| Q2 | `dotnet test` Release | ⬜ после push |
| Q3 | CI `.NET` green | ⬜ ветка `master` ahead 10, не на origin |
| Q4 | SonarCloud / quality gate | ⬜ after CI |
| Q5 | SampleWebApp smoke | 🟨 SQLite in-memory + endpoints; placeholder LicenseKey принято (#M21) |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ TFMs/deps EF bump; releaseNotes-ссылки (#L29 принято) |
| N2 | Secrets `NUGET_API_KEY`, `TAGTOKEN` | ⬜ проверить перед publish |
| N3 | Tag + NuGet push from CI | ⬜ нет `v9.2.0` (SemVer = GitVersion, #H25) |
| N4 | GitHub Release notes | ⬜ для `v9.2.0` |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after land / push origin |
| A2 | Hosts on Cross.CQRS **11.1.2** + этот пакет | ⬜ после NuGet `9.2.0` |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст (работа дельты в version plan) |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | ⬜ |
| G2 | Publish blockers cleared | ✅ version plan C/H/M/L пустые (H25/M21/L28/L29 принято) |

- **Date:** 2026-09-16
- **Notes:** Дельта 10 коммитов на local `master` vs `origin/master` (`v9.0.0`). Open C/H/M/L пустые. Publish tag/NuGet = GitVersion (не имя файла плана).
