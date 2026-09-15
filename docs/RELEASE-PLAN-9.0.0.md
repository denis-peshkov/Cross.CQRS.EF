Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.0.0` · **ветка:** `release/9.0.0-Transaction-Configuration-and-Behavior-Refactor` · **база:** `origin/master` (`v8.4.1`) · **дата:** `2026-09-15`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** —
>
> Дельта: `origin/master...HEAD` — **69** коммита · **168** файлов · **+11994 / −708**. Open: C0 H8 M6 L5

**CodeRabbit:** `2026-09-15` · logs `.cursor/skills/coderabbit/.cache/cr-*-20260915-174*.jsonl` (dirs: `Cross.CQRS.EF`, `Cross.CQRS.EF.Tests`, `SampleWebApp`, `docs`) · 16 findings (0 Critical, 9 Major, 7 Minor) → 14 открыты в плане (#H10–#H16, #M13–#M17, #L24–#L25); skipped 2 (dup #M12, #L21).

**PR:** [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) (`BREAKING:` Unify EF transaction behavior and ship Cross.CQRS.EF 9.0.0).

---

## Критично (безопасность)

---

## Высокий (логика / licensing / auth model)

### ⬜ H9. Default isolation `Serializable` vs 8.4.x / `TransactionBehaviorOptions`

В 8.4.x `BeginTransactionAsync` без isolation (дефолт провайдера, обычно ReadCommitted). В 9.0.0 публичный default `AddEntityFrameworkIntegration(..., isolationLevel: Serializable)`, при этом `TransactionBehaviorOptions.IsolationLevel` по умолчанию `ReadCommitted`. После апгрейда хосты получат более жёсткие блокировки, плюс два разных «дефолта» в одном API.

### ⬜ H10. `CA2007` в `NoWarn` библиотеки

В `Cross.CQRS.EF.csproj` `CA2007` подавлен; library awaits без `ConfigureAwait(false)`. CR: убрать NoWarn и добавить `ConfigureAwait(false)` в библиотечном коде. (CR 2026-09-15)

### ⬜ H11. Detach ChangeTracker не в `finally`

В `UnifiedTransactionBehavior.Handle` cleanup tracked entries выполняется только после успешного switch; при исключении из `next`/commit entries остаются. (CR 2026-09-15)

### ⬜ H12. Raw `DbConnection.BeginTransactionAsync` + `UseTransactionAsync`

Transactional path открывает connection вручную и передаёт raw `DbTransaction` в `UseTransactionAsync`. CR: `Database.BeginTransactionAsync(isolation, ct)` для владения EF. (CR 2026-09-15)

### ⬜ H13. `TransactionLockTests` — shared SQLite connection

Два DbContext делят один `_connection`; CR: named shared-memory connection string, каждый контекст со своим connection. (CR 2026-09-15)

### ⬜ H14. `TransactionBehaviorTests.DifferentBehaviors_*` всё ещё в обход пайплайна

Несмотря на `UnifiedTransactionBehaviorTests`, старый `TestCase(TransactionBehaviorEnum.*)` по-прежнему вручную открывает транзакцию и вызывает handler. (CR 2026-09-15)

### ⬜ H15. `TransactionEventTests` — mock в `OneTimeSetUp`

`_commandEventsMock` создаётся один раз; assertions `Times.Never` могут течь между тестами. Нужен per-test `[SetUp]`. (CR 2026-09-15)

### ⬜ H16. BREAKING / plan / CHANGELOG расходятся по licensing 9.0.0

CR: сверить `docs/BREAKING.md`, `RELEASE-PLAN-9.0.0.md` и `CHANGELOG.md` с фактической регистрацией (`EfLicenseProductInfo` only vs упоминания behavior/hosted validator). (CR 2026-09-15)

---

## Средний (противоречия / баги контрактов)

### ⬜ M12. Switch `_ => default` без `next()`

В `UnifiedTransactionBehavior` неизвестное значение enum возвращает `default(TResponse)` и не вызывает `next()` (в 8.4.x был `ArgumentOutOfRangeException`). Неожиданный enum / порча значения тихо глотает команду.

### ⬜ M13. XML summary `AddEntityFrameworkIntegration` про assemblies

Summary говорит «from the specified assemblies», хотя метод assemblies не принимает. (CR 2026-09-15)

### ⬜ M14. `TransactionLockTests` — observation без `AsNoTracking`

Pre-commit read на `_dbContext2` может кэшировать tracked entity. (CR 2026-09-15)

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

### ⬜ L22. Default `Behavior` в options vs extension

`TransactionBehaviorOptions.Behavior` по умолчанию `TransactionalScopeBehavior`, параметр `AddEntityFrameworkIntegration` — `TransactionalBehavior`. Config-bind без явного enum получит другой режим, чем вызов extension «как есть».

### ⬜ L23. Тело PR #9 устарело относительно дерева

Синхронизировать body с `Configure<TransactionBehaviorOptions>`, актуальным BREAKING и закрытыми H8/M10. (triage 2026-09-15)

### ⬜ L24. CHANGELOG `v9.0.0` выглядит published

Секция датирована как релиз, но tag/GitHub Release ещё нет — пометить Unreleased / pending. (CR 2026-09-15)

---

## Принято (осознанный trade-off)

- Licensing: пакет регистрирует `EfLicenseProductInfo`; своего license pipeline / hosted validator нет.
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
| ✅ EF license SKU | `EfLicenseProductInfo` only. Removed `EfLicenseCheckBehavior` / `EfLicenseHostedValidator` |
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

---

## Что в библиотеке уже нормально

- Точка входа для хоста по-прежнему `AddEntityFrameworkIntegration<TDbContext>` (после `AddCQRS`).
- Транзакции по-прежнему только вокруг `ICommand`; queries идут в `next()` без обёртки.
- `TransactionBehaviorEnum` (No / Transactional / Scope / TransactionalScope) не переименовывался.
- `InternalsVisibleTo("Cross.CQRS.EF.Tests")`; SampleWebApp вызывает новый overload с `ScopeBehavior`.
- Consumer breaking зафиксирован в `docs/BREAKING.md` + `config.nuspec` `releaseNotes` (ссылки, не копипаст секций).

---

## Приоритет фиксов

1. **H11** / **H12** / **M12** — transaction behavior correctness (finally, BeginTransactionAsync, unknown enum).
2. **H9** / **L22** — выровнять defaults isolation/behavior.
3. **H10** — CA2007 / ConfigureAwait в library.
4. **H16** / **L24** — docs consistency (BREAKING/CHANGELOG).
5. **L21** — убрать commented JWT; **H13**–**H15** / **M14**–**M17** — tests/sample hygiene.
6. **M13** / **L20** / **L23**.

Открытый backlog вне этой дельты: [`TO-DO.md`](TO-DO.md).
