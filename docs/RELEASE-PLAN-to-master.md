# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `9.3.0` · [`release/9.3.0-nuspec-CrossCQRS-align`](https://github.com/denis-peshkov/Cross.CQRS.EF/tree/release/9.3.0-nuspec-CrossCQRS-align) · [`RELEASE-PLAN-9.3.0.md`](RELEASE-PLAN-9.3.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-18
> **HEAD:** `da37c85` = [`v9.2.2`](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.2.2) · база `origin/master` · предыдущие планы [`9.2.1`](RELEASE-PLAN-9.2.1.md) / [`9.2.2`](RELEASE-PLAN-9.2.2.md) published

**Change summary:** **22** items — ✅ **10** (45%) · 🟨 **5** (23%) · ⬜ **7** (32%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | 🟨 `next-version: 9.3.0`; tags `v9.2.0`–`v9.2.2`; `v9.3.0` нет |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-9.3.0.md`](RELEASE-PLAN-9.3.0.md) (Open C/H/M/L пустые) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ нет API-break 9.2.2 → 9.3.0 |
| B2 | PR title `BREAKING:` where applicable | ✅ N/A (нет PR на `9.3.0`) |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ только ссылки CHANGELOG / BREAKING |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v9.3.0` (+ `v9.2.1` / `v9.2.2`) |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | ⬜ не гонялся на working tree `9.3.0` |
| Q2 | `dotnet test` Release | ⬜ не гонялся на working tree `9.3.0` |
| Q3 | CI `.NET` green on release branch | 🟨 `master` @ `v9.2.2`; `9.3.0` ещё не запушен |
| Q4 | SonarCloud / quality gate | 🟨 как на последнем `master` push |
| Q5 | SampleWebApp smoke | 🟨 SQLite in-memory + endpoints; placeholder LicenseKey принято |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ Cross.CQRS groups `11.3.1` = csproj (#H26) |
| N2 | Secret `TAGTOKEN` | ⬜ проверить перед publish (`NUGET_API_KEY` заменён OIDC) |
| N3 | Tag + NuGet push from CI | ⬜ нет `v9.3.0` |
| N4 | GitHub Release notes | ⬜ для `v9.3.0` |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after `v9.3.0` |
| A2 | Hosts on Cross.CQRS **11.3.1** + этот пакет | ⬜ после NuGet `9.3.0` |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст; #H26 закрыт в `9.3.0` |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | 🟨 pack metadata ok; Q1/Q2/N2–N4 ещё open |
| G2 | Publish blockers cleared | ✅ #H26 закрыт; остальное — pre-publish checklist |

- **Date:** 2026-09-18
- **Notes:** `master` = `v9.2.2`. Планы `9.2.1`/`9.2.2` published/closed. Текущий `9.3.0` (retarget с `9.2.3`): Open C/H/M/L пустые; nuspec Cross.CQRS = `11.3.1`. Working tree — docs + `.cursor` + nuspec.
