Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.0.0` · **ветка:** `release/9.0.0-Transaction-Configuration-and-Behavior-Refactor` · **база:** `origin/master` (`v8.4.1`) · **дата:** `2026-09-15`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** —
>
> Дельта: `origin/master...HEAD` — **69** коммита · **168** файлов · **+11994 / −708**. Open: C0 H0 M5 L4

**CodeRabbit:** `2026-09-15` · logs `.cursor/skills/coderabbit/.cache/cr-*-20260915-174*.jsonl` (dirs: `Cross.CQRS.EF`, `Cross.CQRS.EF.Tests`, `SampleWebApp`, `docs`) · 16 findings (0 Critical, 9 Major, 7 Minor) → 14 открыты в плане (#H10–#H16, #M13–#M17, #L24–#L25); skipped 2 (dup #M12, #L21).

**PR:** [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) (`BREAKING:` Unify EF transaction behavior and ship Cross.CQRS.EF 9.0.0).

---

## Критично (безопасность)

---

## Высокий (логика / licensing / auth model)

---

## Средний (противоречия / баги контрактов)

### ⬜ M12. Switch `_ => default` без `next()`

В `UnifiedTransactionBehavior` неизвестное значение enum возвращает `default(TResponse)` и не вызывает `next()` (в 8.4.x был `ArgumentOutOfRangeException`). Неожиданный enum / порча значения тихо глотает команду.

### ⬜ M13. XML summary `AddEntityFrameworkIntegration` про assemblies

Summary говорит «from the specified assemblies», хотя метод assemblies не принимает. (CR 2026-09-15)

### ⬜ M15. `CreateTestEntityCommand.CommandId` всегда `Guid.Empty`

Getter без хранения; handler пишет пустой id в events. (CR 2026-09-15)

### ⬜ M16. `DeleteTestEntityCommand.CommandId` всегда `Guid.Empty`

То же для delete command. (CR 2026-09-15)

### ⬜ M17. Sample: wrong `ILogger<T>` на internal handler

`SomeScopeInternalCommandHandler` инжектит logger внешнего handler-типа. (CR 2026-09-15)

---

## Низкий (техдолг / несогласованности)

### ⬜ L20. README не отражает breaking 9.0.0

README всё ещё про «.NET 8 from version 8.0» и не описывает `ExactTransaction`, смену namespace, удаление pagination и зависимость Cross.CQRS 11.1.2.

### ⬜ L21. JWT в комментарии SampleWebApp

В `SampleWebApp/Program.cs` закомментирован полный JWT license key. Даже в комментарии это секрет/PII в git. (triage 2026-09-15)

### ⬜ L23. Тело PR #9 устарело относительно дерева

Синхронизировать body с `Configure<TransactionBehaviorOptions>`, актуальным BREAKING и закрытыми H8/M10. (triage 2026-09-15)

### ⬜ L24. CHANGELOG `v9.0.0` выглядит published

Секция датирована как релиз, но tag/GitHub Release ещё нет — пометить Unreleased / pending. (CR 2026-09-15)

---

## Принято (осознанный trade-off)

- Licensing: пакет регистрирует `EfLicenseProductInfo` (JWT type `Cross_CQRS_EF`).
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
| ✅ EF license SKU | `EfLicenseProductInfo` only (JWT type `Cross_CQRS_EF`) |
| ✅ TFMs net9/net10 | библиотека и nuspec: net6–net10; EF Core per-TFM |
| ✅ Tests project | `Cross.CQRS.EF.Tests` + `UnifiedTransactionBehaviorTests` / `SqlitePipelineHost` (MediatR Send) |
| ✅ slnx + nuspec | `.sln` → `.slnx`; pack `Cross.CQRS.EF/config.nuspec`; `_nuget/` убран |
| ✅ GitVersion 9.0.0 | `next-version: 9.0.0`; CI GitVersion / branch-policy / back-merge / triage |
| ✅ CHANGELOG | published 7.0.0–8.4.1 из GitHub Releases; формат с секциями |
| ✅ BREAKING 8.4.1→9.0.0 | `docs/BREAKING.md` переписан под Cross.CQRS.EF |
| ✅ to-master checklist | `docs/RELEASE-PLAN-to-master.md` на цель `9.0.0` / этот репозиторий |
| ✅ #H8 IOptions registration | `Configure<TransactionBehaviorOptions>`; pipeline резолвит options (triage 2026-09-15) |
| ✅ #M10 pipeline transaction tests | `UnifiedTransactionBehaviorTests` + `SqlitePipelineHost.SendAsync` (triage 2026-09-15) |
| ✅ #L19 чужие plan 11.1.x | файлы `docs/RELEASE-PLAN-11.1.*` в дереве отсутствуют (triage 2026-09-15) |
| ✅ #M11 ResetLicenseCheckForTests | убран из `AddEntityFrameworkIntegration`; на EF-регистрации reset не нужен |
| ✅ #L25 to-master без H8 gates | Q1/Q2/Q5/N1/G2 без ссылок на закрытый H8; B2 = PR #9 |
| ✅ #H9 isolation defaults | `TransactionBehaviorOptions.IsolationLevel` = `Serializable` (как у extension / `ExactTransaction`) |
| ✅ #L22 behavior defaults | `TransactionBehaviorOptions.Behavior` = `TransactionalBehavior` (как у extension) |
| ✅ #H10 ConfigureAwait(false) | library awaits + `CA2007` убран из `NoWarn` |
| ✅ #H11 Detach in finally | ChangeTracker cleanup в `finally` после `Get()` |
| ✅ #H12 EF BeginTransactionAsync | `Database.BeginTransactionAsync(isolation, ct)` вместо raw connection / `UseTransactionAsync` |
| ✅ #H13 lock tests connections | shared-memory URI; отдельный connection на каждый DbContext |
| ✅ #M14 lock tests AsNoTracking | observation reads на `_dbContext2` через `AsNoTracking` |
| ✅ #H14 DifferentBehaviors pipeline | `DifferentBehaviors_*` через `SqlitePipelineHost` / MediatR |
| ✅ #H15 event tests mock SetUp | `_commandEventsMock` создаётся в `[SetUp]`, не в `OneTimeSetUp` |
| ✅ #H16 licensing docs | BREAKING / CHANGELOG / plan: только `EfLicenseProductInfo`, без чужого license pipeline |

---

## Что в библиотеке уже нормально

- Точка входа для хоста по-прежнему `AddEntityFrameworkIntegration<TDbContext>` (после `AddCQRS`).
- Транзакции по-прежнему только вокруг `ICommand`; queries идут в `next()` без обёртки.
- `TransactionBehaviorEnum` (No / Transactional / Scope / TransactionalScope) не переименовывался.
- `InternalsVisibleTo("Cross.CQRS.EF.Tests")`; SampleWebApp вызывает новый overload с `ScopeBehavior`.
- Consumer breaking зафиксирован в `docs/BREAKING.md` + `config.nuspec` `releaseNotes` (ссылки, не копипаст секций).

---

## Приоритет фиксов

1. **M12** — unknown enum must not swallow the command.
2. **L24** — CHANGELOG `v9.0.0` не должен выглядеть published до tag.
3. **L21** — убрать commented JWT; **M15**–**M17** — tests/sample hygiene.
4. **M13** / **L20** / **L23**.

Открытый backlog вне этой дельты: [`TO-DO.md`](TO-DO.md).
