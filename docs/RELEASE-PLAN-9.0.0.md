Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.0.0` (published / closed) · **ветка:** `release/9.0.0-Transaction-Configuration-and-Behavior-Refactor` · **база:** `origin/master` (`v8.4.1`) · **дата:** `2026-09-16`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** —
>
> Дельта: `origin/master...HEAD` — **99** коммита · **176** файлов · **+12594 / −713**. Open C/H/M/L пустые (план закрыт).

**CodeRabbit:**
- `2026-09-15` · logs `.cursor/skills/coderabbit/.cache/cr-*-20260915-174*.jsonl` (dirs: `Cross.CQRS.EF`, `Cross.CQRS.EF.Tests`, `SampleWebApp`, `docs`) · 16 findings (0 Critical, 9 Major, 7 Minor) → все закрыты в этом плане.
- `2026-09-16` · logs `.cursor/skills/coderabbit/.cache/cr-*-20260916-1237*.jsonl` (dirs: `Cross.CQRS.EF`, `Cross.CQRS.EF.Tests`, `SampleWebApp`, `docs`) · 1 finding (0 Critical, 0 Major, 1 Minor) → все закрыты в этом плане.

**PR:** [#9](https://github.com/denis-peshkov/Cross.CQRS.EF/pull/9) (`BREAKING:` Unify EF transaction behavior and ship Cross.CQRS.EF 9.0.0).

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

- Licensing: пакет регистрирует `EfLicenseProductInfo` (JWT type `Cross_CQRS_EF`).
- CHANGELOG целевой версии — dated `## vX.Y.Z` до git tag / GitHub Release (не `Unreleased`).
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
| ✅ #M12 unknown enum | `_ => throw ArgumentOutOfRangeException`; неизвестный behavior больше не глотает команду |
| ✅ #M13 XML assemblies | summary `AddEntityFrameworkIntegration` без «specified assemblies»; returns = `CqrsRegistrationSyntax` |
| ✅ #M15 Create CommandId | `CreateTestEntityCommand` : `Command`; event test проверяет не-empty id |
| ✅ #M16 Delete CommandId | `DeleteTestEntityCommand` : `Command`; unit test проверяет не-empty id |
| ✅ #M17 sample ILogger | `SomeScopeInternalCommandHandler` → `ILogger<SomeScopeInternalCommandHandler>` |
| ✅ #L24 CHANGELOG dated | принято: `## vX.Y.Z` до tag — канон, не Unreleased |
| ✅ #L20 README 9.0.0 | ExactTransaction, Extensions, pagination removed, Cross.CQRS 11.1.2, net6–net10 |
| ✅ #L21 sample JWT | убран commented license JWT из `SampleWebApp/Program.cs` |
| ✅ #L23 PR body | не актуально для version plan: GitHub-описание, не дефект пакета |
| ✅ #M18 remaining CommandId | Update/Failing*/Probe : `Command`/`Command<T>`; query : `Query<T>`; других пустых id нет |
| ✅ #L26 TearDown finally | `HandlerTestsBase`: `EnsureDeleted` null-safe; Dispose в `finally` |
| ✅ #L27 lock TCS | `TransactionLockTests`: TCS handshake; WAL tempfile; observer сравнивает original name, не tracked instance |
| ✅ #H17 tracker cleanup | `Clear()` только в `catch`; `NoBehavior` не трогает ChangeTracker |
| ✅ #H18 ExactTransaction isolation | probe оставляет `ReadCommitted`; assert — isolation, с которым EF начал tx, не SQLite-reported |
| ✅ #H20 isolation TestCase | `IsolationCapture.LastStartedIsolationLevel` в parameterized isolation tests |
| ✅ #H21 event pipeline | `TransactionEventTests` через MediatR; кейс commit-failure не публикует events |
| ✅ #H22 lock timeout | `WaitAsync(5s)` на lock handshake / observer read (и ignored concurrent tests) |
| ✅ #H19 tracker restore | `catch` восстанавливает pre-command snapshot; graph упавшей команды Detach, чужой Unchanged/pending остаётся |
| ✅ #M19 ExecuteAsync CT | оба `executionStrategy.ExecuteAsync` — overload `Func<CancellationToken, Task>` + `cancellationToken` |
| ✅ #H24 tracker values snapshot | rollback: CurrentValues + OriginalValues + State; тот же state больше не `continue` |
| ✅ #H23 retry restore | перед каждой попыткой `ExecuteAsync` — `RestoreTrackedEntities` (тот же snapshot, что #H24) |
| ✅ #M20 retry test uniqueness | `ToList` + `ContainSingle`: дубликат с тем же Name падает, а не `FirstOrDefault` |

---

## Что в библиотеке уже нормально

- Точка входа для хоста по-прежнему `AddEntityFrameworkIntegration<TDbContext>` (после `AddCQRS`).
- Транзакции по-прежнему только вокруг `ICommand`; queries идут в `next()` без обёртки.
- `TransactionBehaviorEnum` (No / Transactional / Scope / TransactionalScope) не переименовывался.
- `InternalsVisibleTo("Cross.CQRS.EF.Tests")`; SampleWebApp вызывает новый overload с `ScopeBehavior`.
- Consumer breaking зафиксирован в `docs/BREAKING.md` + `config.nuspec` `releaseNotes` (ссылки, не копипаст секций).

---

## Приоритет фиксов

_(пусто — релиз `9.0.0` опубликован; открытый backlog → [`TO-DO.md`](TO-DO.md).)_
