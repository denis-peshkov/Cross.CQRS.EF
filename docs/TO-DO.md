# Cross.CQRS.EF — open backlog (`TO-DO`)

Нерешённые пункты вне дельты version plan + кросс-версионные принятые trade-off’ы.

**Id high-water (не переиспользовать ≤):** `C0` `H26` `M22` `L30`

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
- Licensing: только `EfLicenseProductInfo` (JWT type `Cross_CQRS_EF`); не дублировать проверку лицензии в этом пакете.
- SonarCloud display name меняется только анализом main (`master`); PR analysis не переименовывает проект.
- Sonar `qualitygate.wait` только на `pull_request`; push/publish QG не ждёт.
- Локальные хвосты `release/*` могут завышать SemVer — для publish ориентир = CI **после** очистки мусорных tags.
- Корневой `ReleaseNotes.md` не нужен — канон `docs/CHANGELOG.md` (+ ссылка в README).
- CHANGELOG целевой версии — dated `## vX.Y.Z` до git tag / GitHub Release (не `Unreleased`).
- Pagination / `IQueryableFilter` / `QueryableExtensions` убраны из этого пакета — не возвращать без отдельного product decision.
- TFMs библиотеки и тестов: `net6.0`–`net10.0` (netstandard в этом пакете нет).
- Maintainer kit (`.cursor` rules/skills/triage, GitHub templates) — часть репозитория, не NuGet.
- EF Core: patch/latest-stable per TFM, не consumer API break; секция BREAKING только при смене public API.
- SampleWebApp — host smoke, не часть NuGet.
- SampleWebApp `LicenseKey = "YOUR_LICENSE_KEY"` — канонический placeholder, не секрет и не баг.
- `Microsoft.SourceLink.GitHub` `1.1.1` (`PrivateAssets=All`) — достаточно, пока нет отдельного product bump.
- `config.nuspec` `releaseNotes` — только ссылки на CHANGELOG и BREAKING, без summary в nuspec.
- `SkipNetCoreApp31Tests` в agent skills — только если свойство объявлено в test csproj.
- Планируемый unpublished `9.3.0` (nuspec align) свёрнут в major `10.0.0` (Cross.CQRS **11.3.1**); отдельного `RELEASE-PLAN-9.3.0` нет.
- SampleWebApp / Tests: `sealed record` для commands/queries/events — host style, не контракт NuGet.
- Consumer major при bump Cross.CQRS floor с секцией BREAKING (как `9.2.2` → `10.0.0`, Cross.CQRS **11.3.1**).
