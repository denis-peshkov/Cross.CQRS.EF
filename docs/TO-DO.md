# Cross.CQRS.EF — open backlog (`TO-DO`)

Нерешённые пункты вне дельты version plan + кросс-версионные принятые trade-off’ы.

**Id high-water (не переиспользовать ≤):** `C0` `H7` `M9` `L18`

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

- Git tags: только stable SemVer с `master` / `release/*` / `hotfix/*` (никогда с `dev`; без pre-release suffix). NuGet Push также с `dev` (pre-release).
- SemVer только через `GitVersion.yml` (без CI override / matrix fallback / `+semver`); `commit-message-incrementing: Disabled`. CI GitVersion 6.8.2.
- `main`: Inherit от `release`/`hotfix`; корневой `increment: Patch` для orphaned master; цифры в `release/*` игнорируются.
- SampleWebApp / Tests: `CA2007` в `NoWarn` (host/test style), library — `ConfigureAwait(false)`.
- Licensing: только `EfLicenseProductInfo`; не дублировать проверку лицензии в этом пакете.
- SonarCloud display name меняется только анализом main (`master`); PR analysis не переименовывает проект.
- Sonar `qualitygate.wait` только на `pull_request`; push/publish QG не ждёт.
- Локальные хвосты `release/*` могут завышать SemVer — для publish ориентир = CI **после** очистки мусорных tags.
- Корневой `ReleaseNotes.md` не нужен — канон `docs/CHANGELOG.md` (+ ссылка в README).
