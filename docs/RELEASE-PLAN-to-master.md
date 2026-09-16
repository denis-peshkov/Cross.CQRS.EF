# Release readiness plan `dev` → `master`

> **Purpose:** checklist before merging into `master` for a NuGet release.
> **Product:** Cross.CQRS.EF
> **Current target:** `9.2.0` · [`release/9.2.0-EF-Core-SourceLink`](https://github.com/denis-peshkov/Cross.CQRS.EF/tree/release/9.2.0-EF-Core-SourceLink) · [`RELEASE-PLAN-9.2.0.md`](RELEASE-PLAN-9.2.0.md)
> **Legend:** ⬜ open · ✅ done · 🟨 partial · ❌ blocker
> **Related:** [`BREAKING.md`](BREAKING.md) · [`CHANGELOG.md`](CHANGELOG.md) · [`TO-DO.md`](TO-DO.md)
> **Updated:** 2026-09-16
> **HEAD:** `664a8e7` (local, ahead 5 of origin) · PR [#10](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/10) head `7e37133` → `master` · база `origin/master` = [`v9.0.0`](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0)

**Change summary:** **22** items — ✅ **11** (50%) · 🟨 **5** (23%) · ⬜ **6** (27%) · ❌ **0** (0%)

---

## 1. Preconditions

| # | Item | Status |
|---|---|---|
| P1 | Target version agreed (GitVersion / tag `vX.Y.Z`) | ✅ `GitVersion.yml` `next-version: 9.2.0`; tag `v9.2.0` нет |
| P2 | Version plan `docs/RELEASE-PLAN-X.Y.Z.md` filled | ✅ [`RELEASE-PLAN-9.2.0.md`](RELEASE-PLAN-9.2.0.md) (Open C/H/M/L пустые) |
| P3 | `docs/TO-DO.md` — no unexpected C/H blockers | ✅ open C/H/M/L пустые |
| P4 | Branch policy understood (`CONTRIBUTING.md`) | ✅ stable tags; `dev` не тегает |

---

## 2. Breaking changes

| # | Item | Status |
|---|---|---|
| B1 | Consumer breaks in `docs/BREAKING.md` | ✅ нет API-break 9.0.0 → 9.2.0 (только EF patch deps) |
| B2 | PR title `BREAKING:` where applicable | ✅ N/A ([#10](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/10) `chore:`) |
| B3 | `config.nuspec` `releaseNotes` → BREAKING | ✅ только ссылки CHANGELOG / BREAKING (#L29) |
| B4 | `docs/CHANGELOG.md` updated | ✅ `## v9.2.0` |

---

## 3. Build, tests, quality

| # | Item | Status |
|---|---|---|
| Q1 | `dotnet build` Release | 🟨 PR #10 `.NET` SUCCESS на `7e37133`; local tip `664a8e7` ещё не на origin |
| Q2 | `dotnet test` Release | 🟨 тот же job на `7e37133` |
| Q3 | CI `.NET` green on release branch | 🟨 PR SHA green; tip CI нет (ahead 5) |
| Q4 | SonarCloud / quality gate | 🟨 SonarCloud SUCCESS на PR #10 (`7e37133`) |
| Q5 | SampleWebApp smoke | 🟨 SQLite in-memory + endpoints; placeholder LicenseKey принято (#M21) |

---

## 4. Packaging and publish

| # | Item | Status |
|---|---|---|
| N1 | `config.nuspec` metadata | ✅ TFMs/deps EF bump; releaseNotes = ссылки (#L29) |
| N2 | Secrets `NUGET_API_KEY`, `TAGTOKEN` | ⬜ проверить перед publish |
| N3 | Tag + NuGet push from CI | ⬜ нет `v9.2.0` |
| N4 | GitHub Release notes | ⬜ для `v9.2.0` |

---

## 5. After `master`

| # | Item | Status |
|---|---|---|
| A1 | Back-merge `master` → `dev` | ⬜ after land |
| A2 | Hosts on Cross.CQRS **11.1.2** + этот пакет | ⬜ после NuGet `9.2.0` |
| A3 | Leftover TO-DO C/H/M/L | ✅ open пуст (работа дельты в version plan) |

---

## 6. Go / No-Go

| # | Item | Status |
|---|---|---|
| G1 | Go / No-Go recorded | ⬜ PR #10 `mergeable_state=blocked` (reviews) |
| G2 | Publish blockers cleared | ✅ version plan C/H/M/L пустые; H25 = `next-version: 9.2.0` |

- **Date:** 2026-09-16
- **Notes:** Дельта 18 коммитов vs `origin/master` (`v9.0.0`). Local ahead 5 of `origin/release/9.2.0-EF-Core-SourceLink`. Open C/H/M/L пустые. PR #10 CI green на `7e37133` (CodeQL / Sonar / CodeRabbit / `.NET`).
