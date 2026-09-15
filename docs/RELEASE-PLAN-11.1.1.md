Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `11.1.1` (published / closed) · **ветка:** `hotfix/no-preview-git-tags` · **база:** `origin/master` (`v11.1.0`) · **дата:** `2026-09-13`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS/releases/tag/v11.1.1
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-11.1.0.md](RELEASE-PLAN-11.1.0.md)
>
> Дельта: `origin/master...HEAD` — **18** коммита · **26** файлов · **+1075 / −691**. Open C/H/M/L пустые (план закрыт).

**CodeRabbit:** findings → **#L7–#L12** закрыты. Triage: **#M8**. README badge: **#L13**. Sonar: **#L14**. Drop ReleaseNotes: **#L15**. Changelog categorizePath: **#L16**. Drop gitversion-strategy skill: **#L17**.

**PR:** [#22](https://github.com/denis-peshkov/Cross.CQRS/pull/22) (GitVersion actions v4.7 and stop tagging from dev).

---

## Критично (безопасность)

---

## Высокий (логика / auth model)

---

## Средний (противоречия / баги контрактов)

---

## Низкий (техдолг / несогласованности)

---

## Принято (осознанный trade-off)

- SemVer целится **только** через `GitVersion.yml` (без CI `/overrideconfig`, без matrix fallback, без `+semver`).
- `commit-message-incrementing: Disabled` — всегда.
- `main.increment: Inherit` + `source-branches: [release, hotfix]`; корневой `increment: Patch` для orphaned `master`.
- Цифры в имени `release/*` игнорируются.
- CI: GitVersion **6.8.2**; git tag только stable на `master`/`release`/`hotfix`; **`dev` никогда не тегает** (NuGet `-dev.*` можно).
- Локальные хвосты `release/*` тоже могут завышать SemVer — для publish ориентир = CI **после** очистки мусорных tags.
- SonarCloud display name (`sonar.projectName`) обновляется **только** анализом main (`master`); PR-анализ пишет в тот же `projectKey`, но имя не меняет.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ #H7 delete v11.2.0-dev.3 | удалён с origin; `dev` убран из Create/Push git Tag (NuGet на `dev` остаётся) |
| ✅ #L7 changelog --dry-run wins | любой `--dry-run` запрещает запись (даже с `--write`); только печать секции; тесты |
| ✅ #L8 changelog pathBullet neutral | только path-based bullets; без hardcoded v4.7 / tag-policy / strategy claims; тест |
| ✅ #L9 no quiet CR fixes | `coderabbit` Phase 4: fix+`✅ #Id` same turn (release-plan не дублировали) |
| ✅ #L10 changelog collectDelta fail-hard | `rev-parse v${from}`; diff/log без fail→`[]`; тесты |
| ✅ #L11 CR explain→plan | pasted finding / «объясни» → Phase 3 open row **same turn** (forbid explain-only) |
| ✅ #L12 changelog JSDoc coverage | JSDoc на всех функциях `update-changelog.mjs` (docstring threshold) |
| ✅ #M8 triage full PR scope | PR comment/labels на весь `base...head` (все коммиты); `fetch-depth: 0`; `pr-scope.mjs` |
| ✅ #L13 README license badge | static `RPL 1.5` badge (как Cross.Identity); не `github/license` → NOASSERTION |
| ✅ #L14 Sonar projectName | `-Dsonar.projectName=Cross.CQRS` в CI; display name применится после анализа на `master` |
| ✅ #L15 drop root ReleaseNotes.md | дубль README/`docs/CHANGELOG.md`; убран из slnx + CONTRIBUTING |
| ✅ #L16 changelog categorizePath | repo-agnostic path heuristics + tests; grouping без hardcoded layout |
| ✅ #L17 drop gitversion-strategy skill | skill/scripts/matrix templates удалены; стратегия остаётся в `GitVersion.yml` + план |
| ✅ GitVersion PR Number capture | `pull-request.regex` + `(?<Number>\d+)` — CI PR не отдаёт `pr{Number}` (NU5010) |
| ✅ GitVersion.yml GV6 strategy | `Disabled`; root `Patch`; main `Inherit` + release/hotfix |
| ✅ Matrix golden green | release Minor / hotfix Patch / direct push Patch (исторически) |
| ✅ CI versionSpec 6.8.2 | + stable-only Create/Push tag |
| ✅ CHANGELOG v11.1.0 process | tags только stable |
| ✅ RELEASE-PLAN-11.1.0 closed | предыдущий план |
| ✅ .gitignore `.tmp-*` | |
| ✅ GitVersion actions v4.7.0 | setup/execute `@v4.7.0` для GV 6.8.x |
| ✅ #L6 CHANGELOG v11.1.1 | `update-changelog.mjs --write`; секция в `docs/CHANGELOG.md` |
| ✅ update-changelog.mjs | release-plan Phase 3 всегда пишет CHANGELOG |
| ✅ triage-pr checklists | `dotnet-checklist` + `angular-checklist` обновлены |

---

## Что в библиотеке уже нормально

- API / licensing / MediatR в дельте не менялись.
- Breaking для NuGet не нужен (`11.1.0` → `11.1.1` patch process/CI).
- Без тега `v11.2.0-dev.3` clean clone master → SemVer **`11.1.1`**.
- Sonar `projectKey=Cross.CQRS` принимает PR-анализы (#22).

---

## Приоритет фиксов

_(пусто — релиз `11.1.1` опубликован; открытый backlog → [`TO-DO.md`](TO-DO.md).)_
