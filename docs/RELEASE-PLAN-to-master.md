# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `10.0.0` · `release/nuspec-CrossCQRS-align` · [`RELEASE-PLAN-10.0.0.md`](RELEASE-PLAN-10.0.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-18
> **HEAD:** tip of `release/nuspec-CrossCQRS-align` · база `origin/master` = [`v9.2.2`](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.2.2) · [`9.2.1`](RELEASE-PLAN-9.2.1.md) / [`9.2.2`](RELEASE-PLAN-9.2.2.md) published

**Change summary:** **22** items — ✅ **8** (36%) · 🟨 **4** (18%) · ⬜ **10** (45%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | 🟨 `next-version: 10.0.0`; tags through `v9.2.2`; `v10.0.0` нет |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-10.0.0.md`](RELEASE-PLAN-10.0.0.md) (Open C/H/M/L пустые) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ From 9.2.2 → 10.0.0 (Cross.CQRS **11.3.1**) |
| B2 | PR title `BREAKING:` where applicable | ⬜ PR ещё нет — title должен быть `BREAKING:` |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ только ссылки CHANGELOG / BREAKING |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v10.0.0` |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | ⬜ не гонялся на tip `10.0.0` |
| Q2 | `dotnet test` Release | ⬜ не гонялся на tip `10.0.0` |
| Q3 | CI `.NET` green on release branch | ⬜ ветка / tip ещё не на CI publish path |
| Q4 | SonarCloud / quality gate | ⬜ после PR / push |
| Q5 | SampleWebApp smoke | 🟨 SQLite + `sealed record` handlers; placeholder LicenseKey принято |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ Cross.CQRS groups `11.3.1` = csproj (#H26) |
| N2 | Secret `TAGTOKEN` | ⬜ проверить перед publish (NuGet = OIDC) |
| N3 | Tag + NuGet push from CI | ⬜ нет `v10.0.0` |
| N4 | GitHub Release notes | ⬜ для `v10.0.0` |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after `v10.0.0` |
| A2 | Hosts on Cross.CQRS **11.3.1** + этот пакет | ⬜ после NuGet `10.0.0` |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст; #H26 закрыт в `10.0.0` |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | 🟨 pack/BREAKING/CHANGELOG ok; Q1–Q4 / N2–N4 ещё open |
| G2 | Publish blockers cleared | 🟨 #H26 закрыт; ждать build/test + tag |

- **Date:** 2026-09-18
- **Notes:** Дельта 2 коммита vs `origin/master` (`v9.2.2`): nuspec/csproj Cross.CQRS **11.3.1**, `sealed record` samples/tests, docs. Open C/H/M/L пустые. PR нет. Планируемый `9.3.0` не публиковался — работа в [`RELEASE-PLAN-10.0.0.md`](RELEASE-PLAN-10.0.0.md).
