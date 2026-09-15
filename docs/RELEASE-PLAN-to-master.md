# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS
> **Current target:** `11.1.2` · [`hotfix/release-plan-triage`](https://github.com/denis-peshkov/Cross.CQRS/tree/hotfix/release-plan-triage) · [`RELEASE-PLAN-11.1.2.md`](RELEASE-PLAN-11.1.2.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-14

**Change summary:** **22** items — ✅ **15** (68%) · 🟨 **0** (0%) · ⬜ **7** (32%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | ✅ `11.1.2` |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-11.1.2.md`](RELEASE-PLAN-11.1.2.md) (closed) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ нет (только CI/GitVersion/docs) |
| B2 | PR title `BREAKING:` where applicable | ✅ N/A |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ без изменений |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v11.1.2` |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | ✅ N/A — API не менялся |
| Q2 | `dotnet test` Release | ✅ N/A — API не менялся |
| Q3 | CI `.NET` green on release branch | ⬜ после tip CI на `hotfix/release-plan-triage` |
| Q4 | SonarCloud / quality gate | ⬜ after tip CI |
| Q5 | SampleWebApp smoke | ✅ N/A — product unchanged |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ без изменений |
| N2 | Secrets `NUGET_API_KEY`, `TAGTOKEN` | ✅ |
| N3 | Tag + NuGet push from CI | ⬜ ожидать `v11.1.2` |
| N4 | GitHub Release notes | ⬜ для `v11.1.2` |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after land |
| A2 | Sibling packages consume core | ✅ N/A этот релиз |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | ⬜ |
| G2 | Publish blockers cleared | ⬜ tip CI green → N3/N4 |

- **Date:** 2026-09-14
- **Notes:** Version plan `11.1.2` **closed** (leftovers → TO-DO: none). Publish: merge [#24](https://github.com/denis-peshkov/Cross.CQRS/pull/24) → tip CI → tag `v11.1.2` / NuGet.
