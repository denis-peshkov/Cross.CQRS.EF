Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.0.0` · **ветка:** `release/9.0.0-Transaction-Configuration-and-Behavior-Refactor` · **база:** `origin/master` (`v8.4.1`) · **дата:** `2026-09-15`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** —
>
> Дельта: `origin/master...HEAD` — **67** коммита · **157** файлов · **+11684 / −706**. Open: C0 H2 M3 L4

**CodeRabbit:** не запускался.

**PR:** —

---

## Критично (безопасность)

---

## Высокий (логика / licensing / auth model)

### ⬜ H8. `IOptions<TransactionBehaviorOptions>` vs `AddSingleton`

`UnifiedTransactionBehavior` зависит от `IOptions<TransactionBehaviorOptions>`, а `AddEntityFrameworkIntegration` регистрирует голый `AddSingleton(options)`. OptionsManager этот singleton не подхватывает — резолв пайплайна на первой команде, скорее всего, упадёт. Тесты (см. M10) DI этого поведения не проверяют.

### ⬜ H9. Default isolation `Serializable` vs 8.4.x / `TransactionBehaviorOptions`

В 8.4.x `BeginTransactionAsync` без isolation (дефолт провайдера, обычно ReadCommitted). В 9.0.0 публичный default `AddEntityFrameworkIntegration(..., isolationLevel: Serializable)`, при этом `TransactionBehaviorOptions.IsolationLevel` по умолчанию `ReadCommitted`. После апгрейда хосты получат более жёсткие блокировки, плюс два разных «дефолта» в одном API.

---

## Средний (противоречия / баги контрактов)

### ⬜ M10. Transaction-тесты обходят `UnifiedTransactionBehavior`

`TransactionBehaviorTests` / `TransactionLockTests` / `TransactionEventTests` создают handler вручную и сами открывают транзакцию. `TestCase(TransactionBehaviorEnum.*)` не меняет путь выполнения. Регресс H8 и режимы Scope / TransactionalScope пайплайном не ловятся.

### ⬜ M11. `ResetLicenseCheckForTests()` в production-регистрации

`AddEntityFrameworkIntegration` вызывает `LicenseCheckExtensions.ResetLicenseCheckForTests()` при каждом хост-setup. Это тестовый reset static license cache в потребительском DI.

### ⬜ M12. Switch `_ => default` без `next()`

В `UnifiedTransactionBehavior` неизвестное значение enum возвращает `default(TResponse)` и не вызывает `next()` (в 8.4.x был `ArgumentOutOfRangeException`). Неожиданный enum / порча значения тихо глотает команду.

---

## Низкий (техдолг / несогласованности)

### ⬜ L19. Чужие version plan’ы Cross.CQRS `11.1.x` в `docs/`

В дельте лежат `docs/RELEASE-PLAN-11.1.0.md` / `11.1.1` / `11.1.2` (продукт Cross.CQRS, не EF). Для SemVer этого репо предыдущего плана нет (`—`). Файлы путают потребителей и чеклист; не удалять без явной команды.

### ⬜ L20. README не отражает breaking 9.0.0

README всё ещё про «.NET 8 from version 8.0» и не описывает `ExactTransaction`, смену namespace, удаление pagination и зависимость Cross.CQRS 11.1.2.

### ⬜ L21. JWT в комментарии SampleWebApp

В `SampleWebApp/Program.cs` закомментирован полный JWT license key. Даже в комментарии это секрет/PII в git.

### ⬜ L22. Default `Behavior` в options vs extension

`TransactionBehaviorOptions.Behavior` по умолчанию `TransactionalScopeBehavior`, параметр `AddEntityFrameworkIntegration` — `TransactionalBehavior`. Config-bind без явного enum получит другой режим, чем вызов extension «как есть».

---

## Принято (осознанный trade-off)

- Licensing только через core (`ILicenseProductInfo` / `CheckLicense`): EF не дублирует JWT-стек; слот пайплайна **−1**, hosted validator.
- Pagination / `IQueryableFilter` / `QueryableExtensions` убраны из этого пакета — не возвращать без отдельного product decision.
- SemVer только `GitVersion.yml` (`next-version: 9.0.0`; digits в `release/*` игнорируются).
- TFMs библиотеки и тестов: `net6.0`–`net10.0` (netstandard в этом пакете нет).
- Maintainer kit (`.cursor` rules/skills/triage, GitHub templates) — часть репозитория, не NuGet.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ UnifiedTransactionBehavior | три pipeline-класса → один `UnifiedTransactionBehavior` (order 10); enum значений сохранён |
| ✅ ExactTransactionAttribute | `ExplicitTransactionAttribute` (opt-out) → `ExactTransactionAttribute(behavior, isolation)` |
| ✅ Pagination APIs removed | `PaginationQuery*` / models / `QueryableExtensions` / `IQueryableFilter` scan удалены |
| ✅ EF license slot | `EfLicenseProductInfo` + `EfLicenseCheckBehavior` (−1) + `EfLicenseHostedValidator`; Cross.CQRS 11.1.2 |
| ✅ TFMs net9/net10 | библиотека и nuspec: net6–net10; EF/Hosting per-TFM |
| ✅ Tests project | `Cross.CQRS.EF.Tests` добавлен (licensing registration покрыт; pipeline — см. M10) |
| ✅ slnx + nuspec | `.sln` → `.slnx`; pack `Cross.CQRS.EF/config.nuspec`; `_nuget/` убран |
| ✅ GitVersion 9.0.0 | `next-version: 9.0.0`; CI GitVersion / branch-policy / back-merge / triage |
| ✅ CHANGELOG v9.0.0 | `update-changelog.mjs --write`; секция уточнена |
| ✅ BREAKING 8.4.1→9.0.0 | `docs/BREAKING.md` переписан под Cross.CQRS.EF (черновик Cross.CQRS 11.0.0 убран) |
| ✅ to-master checklist | `docs/RELEASE-PLAN-to-master.md` на цель `9.0.0` / этот репозиторий |

---

## Что в библиотеке уже нормально

- Точка входа для хоста по-прежнему `AddEntityFrameworkIntegration<TDbContext>` (после `AddCQRS`).
- Транзакции по-прежнему только вокруг `ICommand`; queries идут в `next()` без обёртки.
- `TransactionBehaviorEnum` (No / Transactional / Scope / TransactionalScope) не переименовывался.
- `InternalsVisibleTo("Cross.CQRS.EF.Tests")`; SampleWebApp вызывает новый overload с `ScopeBehavior`.
- Consumer breaking зафиксирован в `docs/BREAKING.md` + `config.nuspec` `releaseNotes` (ссылки, не копипаст секций).

---

## Приоритет фиксов

1. **H8** — согласовать регистрацию options с `IOptions<T>` (иначе пакет не резолвится в пайплайне).
2. **H9** — явно выбрать default isolation и выровнять `TransactionBehaviorOptions`.
3. **M10** — тесты через MediatR / `UnifiedTransactionBehavior` (ловят H8).
4. **M11** / **M12** — убрать test-only reset из production; не глотать неизвестный enum.
5. **L20**–**L22**, затем **L19** (чужие 11.1.x планы — только по явной команде на удаление).

Открытый backlog вне этой дельты: [`TO-DO.md`](TO-DO.md).
