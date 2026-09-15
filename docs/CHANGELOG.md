# Changelog — Cross.CQRS.EF

Newest releases first. GitHub releases: <https://github.com/denis-peshkov/Cross.CQRS.EF/releases>

Breaking upgrade notes for NuGet consumers: [`BREAKING.md`](BREAKING.md).

---

## v9.0.0 — 10 Sep 2025

Transaction Configuration and Behavior Refactor.

1. **Centralized configuration** — `TransactionBehaviorOptions` for unified transaction settings; configurable isolation levels via DI (singleton).
2. **Unified transaction handling** — replaced multiple behaviors (`TransactionalBehavior`, `ScopeBehavior`, etc.) with a single `UnifiedTransactionBehavior`; removed switch-based logic in favor of internal strategy selection.
3. **Improved documentation** — enhanced public API docs and parameter descriptions; clarified registration order and execution logic.
4. **Structural improvements** — reorganized code into feature folders (e.g. `Extensions`); added project icon and minor formatting updates; renamed “Solution Items” folder to `assets`; added `icon.svg`.
5. **Sample handler** — updated `SomeScopeExternalCommandHandler` (constructor parameters; `Events.Write` → `CommandEvents.Write`; logging).
6. Added IntegrationTests.
7. Moved `PaginationQuery` / `PaginationQueryHandler` into a separate library.
8. Removed `PaginationResult.cs`.

Goal: simplify transaction management; improve configurability, maintainability, and clarity.

---

## v8.4.1 — 9 Mar 2025

- Connect to Sonar Cloud.

---

## v8.4.0 — 13 Aug 2024

- Added NoBehavior.
- Added TransactionalScopeBehavior.

---

## v8.3.0 — 10 Aug 2024

- Added ScopeBehavior.
- Added SampleProject.

---

## v8.2.1 — 16 May 2024

- Add build pipeline.

---

## v8.2.0 — 16 Apr 2024

- Upgraded packages.

---

## v8.1.0 — 3 Apr 2024

- Small fixes.
- Upgraded packages.

---

## v8.0.1 — 24 Nov 2023

- Upgraded packages.

---

## v8.0.0 — 23 Nov 2023

- Added support for .NET 8.

---

## v7.0.0 — 18 Nov 2023

- Latest source from original repository.
