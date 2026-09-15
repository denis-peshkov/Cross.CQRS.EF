Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `11.1.2` (closed) · **ветка:** `hotfix/release-plan-triage` · **база:** `origin/master` (`v11.1.1`) · **дата:** `2026-09-14`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS/releases/tag/v11.1.2
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-11.1.1.md](RELEASE-PLAN-11.1.1.md)
>
> Дельта: `origin/master...HEAD` — **5** коммита · **30** файлов · **+435 / −269**. Open C/H/M/L пустые (план закрыт).

**CodeRabbit:** не запускался.

**PR:** [#24](https://github.com/denis-peshkov/Cross.CQRS/pull/24).

---

## Критично (безопасность)

---

## Высокий (логика / licensing / auth model)

---

## Средний (противоречия / баги контрактов)

---

## Низкий (техдолг / несогласованности)

---

## Принято (осознанный trade-off)

- Sonar `qualitygate.wait` только на `pull_request` (`QUALITY_GATE_WAIT`); push/publish QG не ждёт.
- `GitVersion.yml` в этой дельте — только whitespace / line-ending; стратегия SemVer не менялась (канон по-прежнему только этот файл).

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ #M9 Checklist path to-master | rename `docs/RELEASE-PLAN-dev-to-master.md` → `docs/RELEASE-PLAN-to-master.md`; CONTRIBUTING + 11.1.0 link; CLI `--write` находит файл |
| ✅ rename release-plan-to-master | `release-plan-summary.mjs` → `release-plan-to-master.mjs` (+ tests, `FINALIZE-REPLY.md`) |
| ✅ flatten-paginated | `gh api --paginate --slurp` → плоский список; тесты; wired в `post-pr-triage.mjs` |
| ✅ triage ready_for_review | `triage.yml` `pull_request` / `pull_request_target` + `ready_for_review` |
| ✅ CONTRIBUTING CQRS rewrite | branching / licensing / testing; без Identity leftovers |
| ✅ PR template comments | подсказки для Changes и Test plan |
| ✅ table formatting | `|---|` / spacing в skills, templates, BREAKING, планах |
| ✅ GitVersion.yml whitespace | ignore-all-space diff пустой; стратегия не менялась |
| ✅ Sonar QG wait on PR | `QUALITY_GATE_WAIT` + `-Dsonar.qualitygate.wait` |
| ✅ update-nuspec-action@v2 | шаг перед `nuget pack` (deps из `.csproj` → nuspec) |
| ✅ #L18 CHANGELOG v11.1.2 | `update-changelog.mjs --write`; секция уточнена |

---

## Что в библиотеке уже нормально

- API / licensing / MediatR / тесты библиотеки в дельте не менялись (`Cross.CQRS/`, `Cross.CQRS.Tests/` пустые).
- Breaking для NuGet не нужен (`11.1.1` → `11.1.2` patch: docs / CI / maintainer tooling).
- `docs/BREAKING.md` — только формат таблиц, новой секции `From 11.1.1 to 11.1.2` нет.

---

## Приоритет фиксов

_(пусто — релиз `11.1.2` опубликован; открытый backlog → [`TO-DO.md`](TO-DO.md).)_
