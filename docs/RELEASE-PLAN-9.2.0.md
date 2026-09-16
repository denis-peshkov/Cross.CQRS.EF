Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.2.0` · **ветка:** `release/9.2.0-EF-Core-SourceLink` · **база:** `origin/master` (`v9.0.0`) · **дата:** `2026-09-16`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.2.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-9.0.0.md](RELEASE-PLAN-9.0.0.md)
>
> Дельта: `origin/master...HEAD` — **15** коммита · **32** файлов · **+495 / −278**. Open C/H/M/L пустые.

**CodeRabbit:**
- `2026-09-16` · pasted finding (TransactionLockTests ~L149) · 1 findings (0 Critical, 0 Major, 1 Minor) → все закрыты в этом плане.
- `2026-09-16` · pasted finding (SampleWebApp.csproj ImplicitUsings) · 1 findings (0 Critical, 0 Major, 0 Minor) · 1 Trivial/Info → все закрыты в этом плане.

**PR:** [#10](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/10) (`chore:` SourceLink, EF bumps, lock tests, docs/rules hygiene).

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

- Bump EF Core — patch/latest-stable per TFM, не consumer API break; отдельной секции `BREAKING.md` `From 9.0.0 to 9.2.0` нет.
- SemVer только `GitVersion.yml` (`next-version: 9.2.0` на этой ветке).
- `SampleWebApp` — host smoke, не часть NuGet.
- Maintainer `.cursor/rules` в этой дельте — репозиторий, не пакет.
- SampleWebApp `LicenseKey = "YOUR_LICENSE_KEY"` — канонический placeholder, не секрет и не баг.
- `Microsoft.SourceLink.GitHub` `1.1.1` (`PrivateAssets=All`) — достаточно для этой дельты.
- `config.nuspec` `releaseNotes` — только ссылки на CHANGELOG и BREAKING, без summary в nuspec.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ EF Core bump | net8 `8.0.31` / net9 `9.0.20` / net10 `10.0.12`; nuspec groups совпадают |
| ✅ SourceLink added | `Microsoft.SourceLink.GitHub` 1.1.1 `PrivateAssets=All` |
| ✅ #H25 GitVersion next-version | `GitVersion.yml` `next-version: 9.2.0` (`7e37133`) |
| ✅ #M21 sample LicenseKey | принято: placeholder `YOUR_LICENSE_KEY`, код не менять |
| ✅ #L28 SourceLink 1.1.1 | принято: 1.1.1 ок, не bump до 8.x |
| ✅ #L29 nuspec releaseNotes | только ссылки CHANGELOG / BREAKING, без абзаца про UnifiedTransaction / pagination |
| ✅ SampleWebApp SQLite | in-memory + `EnsureCreated`, POST/GET `/somescope`, `[ExactTransaction]` |
| ✅ Tests naming / lock | без суффикса `Async`; lock tests WAL/TCS; test TFM pkgs `6.0.36` |
| ✅ README RPL 1.5 | badge + описание SampleWebApp / Tests |
| ✅ CONTRIBUTING Licensing | folder в scope репозитория |
| ✅ cursor rules dedupe | logging / secrets / readonly deps |
| ✅ CHANGELOG v9.2.0 | `update-changelog.mjs --write`; секция уточнена |
| ✅ pr-message skill | `.cursor/skills/pr-message` + Shell `required_permissions: ["all"]` |
| ✅ #M22 lock-read test | writer `BEGIN EXCLUSIVE` + plain reader SELECT (без второго `BeginTransactionAsync`) |
| ✅ #L30 SampleWebApp ImplicitUsings | `disable`; импорты только из `GlobalUsings.cs` |

---

## Что в библиотеке уже нормально

- Public API транзакций (`AddEntityFrameworkIntegration`, `UnifiedTransactionBehavior`, `ExactTransaction`) в дельте не менялся.
- Cross.CQRS **11.1.2**; TFMs net6–net10; net6/net7 EF 6.0.36 / 7.0.20 без bump.
- Breaking 8.4.1 → 9.0.0 по-прежнему в `docs/BREAKING.md`; для 9.0.0 → 9.2.0 API-секция не нужна.

---

## Приоритет фиксов

Открытый backlog вне этой дельты: [`TO-DO.md`](TO-DO.md).
