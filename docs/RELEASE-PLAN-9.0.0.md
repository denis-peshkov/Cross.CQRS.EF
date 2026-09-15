Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.0.0` · **ветка:** `release/9.0.0-Transaction-Configuration-and-Behavior-Refactor` · **база:** `origin/master` (`v8.4.1`) · **дата:** `2026-09-15`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** —
>
> Дельта: `origin/master...HEAD` — **69** коммита · **168** файлов · **+11994 / −708**. Open: C0 H1 M2 L4

**CodeRabbit:** не запускался.

**PR:** [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) (`BREAKING:` Unify EF transaction behavior and ship Cross.CQRS.EF 9.0.0).

---

## Критично (безопасность)

---

## Высокий (логика / licensing / auth model)

### ⬜ H9. Default isolation `Serializable` vs 8.4.x / `TransactionBehaviorOptions`

В 8.4.x `BeginTransactionAsync` без isolation (дефолт провайдера, обычно ReadCommitted). В 9.0.0 публичный default `AddEntityFrameworkIntegration(..., isolationLevel: Serializable)`, при этом `TransactionBehaviorOptions.IsolationLevel` по умолчанию `ReadCommitted`. После апгрейда хосты получат более жёсткие блокировки, плюс два разных «дефолта» в одном API.

---

## Средний (противоречия / баги контрактов)

### ⬜ M11. `ResetLicenseCheckForTests()` в production-регистрации

`AddEntityFrameworkIntegration` вызывает `LicenseCheckExtensions.ResetLicenseCheckForTests()` при каждом хост-setup. Это тестовый reset static license cache в потребительском DI.

### ⬜ M12. Switch `_ => default` без `next()`

В `UnifiedTransactionBehavior` неизвестное значение enum возвращает `default(TResponse)` и не вызывает `next()` (в 8.4.x был `ArgumentOutOfRangeException`). Неожиданный enum / порча значения тихо глотает команду.

---

## Низкий (техдолг / несогласованности)

### ⬜ L20. README не отражает breaking 9.0.0

README всё ещё про «.NET 8 from version 8.0» и не описывает `ExactTransaction`, смену namespace, удаление pagination и зависимость Cross.CQRS 11.1.2.

### ⬜ L21. JWT в комментарии SampleWebApp

В `SampleWebApp/Program.cs` закомментирован полный JWT license key. Даже в комментарии это секрет/PII в git. (triage 2026-09-15)

### ⬜ L22. Default `Behavior` в options vs extension

`TransactionBehaviorOptions.Behavior` по умолчанию `TransactionalScopeBehavior`, параметр `AddEntityFrameworkIntegration` — `TransactionalBehavior`. Config-bind без явного enum получит другой режим, чем вызов extension «как есть».

### ⬜ L23. Тело PR #9 устарело относительно дерева

В PR всё ещё могут фигурировать старые формулировки (`AddSingleton` options). Нужно синхронизировать body с `Configure<TransactionBehaviorOptions>`, актуальным `docs/BREAKING.md` (From 8.4.1 → 9.0.0) и закрытыми H8/M10. (triage 2026-09-15)

---

## Принято (осознанный trade-off)

- Licensing только через core (`ILicenseProductInfo` / `CheckLicense`): EF регистрирует `EfLicenseProductInfo`, без второго pipeline-слота и без hosted validator.
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
| ✅ EF license SKU | `EfLicenseProductInfo` only; CheckLicense via core (−2). No `EfLicenseCheckBehavior` / `EfLicenseHostedValidator` |
| ✅ TFMs net9/net10 | библиотека и nuspec: net6–net10; EF/Hosting per-TFM |
| ✅ Tests project | `Cross.CQRS.EF.Tests` + `UnifiedTransactionBehaviorTests` / `SqlitePipelineHost` (MediatR Send) |
| ✅ slnx + nuspec | `.sln` → `.slnx`; pack `Cross.CQRS.EF/config.nuspec`; `_nuget/` убран |
| ✅ GitVersion 9.0.0 | `next-version: 9.0.0`; CI GitVersion / branch-policy / back-merge / triage |
| ✅ CHANGELOG | published 7.0.0–8.4.1 из GitHub Releases; формат с секциями |
| ✅ BREAKING 8.4.1→9.0.0 | `docs/BREAKING.md` переписан под Cross.CQRS.EF |
| ✅ to-master checklist | `docs/RELEASE-PLAN-to-master.md` на цель `9.0.0` / этот репозиторий |
| ✅ #H8 IOptions registration | `Configure<TransactionBehaviorOptions>`; pipeline резолвит options (triage 2026-09-15) |
| ✅ #M10 pipeline transaction tests | `UnifiedTransactionBehaviorTests` + `SqlitePipelineHost.SendAsync` (triage 2026-09-15) |
| ✅ #L19 чужие plan 11.1.x | файлы `docs/RELEASE-PLAN-11.1.*` в дереве отсутствуют (triage 2026-09-15) |

---

## Что в библиотеке уже нормально

- Точка входа для хоста по-прежнему `AddEntityFrameworkIntegration<TDbContext>` (после `AddCQRS`).
- Транзакции по-прежнему только вокруг `ICommand`; queries идут в `next()` без обёртки.
- `TransactionBehaviorEnum` (No / Transactional / Scope / TransactionalScope) не переименовывался.
- `InternalsVisibleTo("Cross.CQRS.EF.Tests")`; SampleWebApp вызывает новый overload с `ScopeBehavior`.
- Consumer breaking зафиксирован в `docs/BREAKING.md` + `config.nuspec` `releaseNotes` (ссылки, не копипаст секций).

---

## Приоритет фиксов

1. **H9** — явно выбрать default isolation и выровнять `TransactionBehaviorOptions`.
2. **M11** / **M12** — убрать test-only reset из production; не глотать неизвестный enum.
3. **L21** — убрать commented JWT из SampleWebApp перед merge.
4. **L20** / **L22**, затем **L23** (sync PR #9 body).

Открытый backlog вне этой дельты: [`TO-DO.md`](TO-DO.md).
