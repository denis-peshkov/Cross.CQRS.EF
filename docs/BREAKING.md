# Breaking changes (NuGet consumers)

Breaking changes for **Cross.CQRS.EF**, grouped by **from → to** package version.
Sections are **newest first** (top) → **oldest last** (bottom). When skipping releases, apply every intervening section **from oldest to newest** (bottom-up through the relevant range).

| Upgrade path | Section |
|---|---|
| `8.4.1` → `9.0.0+` | [From 8.4.1 to 9.0.0](#from-841-to-900) |

Breaking-change details live **only** in this file. [`Cross.CQRS.EF/config.nuspec`](../Cross.CQRS.EF/config.nuspec) `releaseNotes` should link here and must not duplicate the versioned sections.

When shipping a new breaking change: insert a **From X.Y.Z to A.B.C** section **at the top** of the versioned sections (and a matching TOC row), and prefix the **PR title** with `BREAKING:`.

---

## From 8.4.1 to 9.0.0

Release: [v9.0.0](https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.0.0).

### Transactions

| Area | Was (8.4.x) | Now (9.0.0+) |
|---|---|---|
| Pipeline | one of `TransactionalBehavior` / `ScopeBehavior` / `TransactionalScopeBehavior` | single `UnifiedTransactionBehavior` (order 10) |
| Opt-out attribute | `ExplicitTransactionAttribute` (empty; skip wrapper) | `ExactTransactionAttribute(behavior, isolation)` |
| Isolation | EF `BeginTransactionAsync` without explicit isolation | `AddEntityFrameworkIntegration(..., isolationLevel)` and `TransactionBehaviorOptions.IsolationLevel` default `Serializable` |
| Extension namespace | `Cross.CQRS.EF` | `Cross.CQRS.EF.Extensions` |

**Action:** replace `[ExplicitTransaction]` with `[ExactTransaction(TransactionBehaviorEnum.NoBehavior)]` to keep opt-out. Add `using Cross.CQRS.EF.Extensions`. Re-evaluate isolation: 9.0.0 default is `Serializable` (stricter than typical 8.4.x provider default).

### Pagination / query helpers

Removed from this package: `PaginationQuery`, `PaginationQueryHandler`, `PaginationRequest`, `PaginationResult`, `SortDirectionEnum`, `SortParameter`, `QueryableExtensions`, `IQueryableFilter` (and the assembly scan that registered filters).

**Action:** move paging/filter helpers into the host or another package; remove usings that pointed at the deleted types.

### Licensing / core dependency

| Area | Was | Now |
|---|---|---|
| Cross.CQRS | 7.x / 8.2.0 per TFM | **11.1.2** all TFMs |
| License | none in EF | `EfLicenseProductInfo` |
| Product claim | n/a | when a license key is supplied, JWT `type` must include `Cross_CQRS_EF` |

**Action:** upgrade the host to Cross.CQRS 11.1.2.

### Target frameworks / packaging

| Area | Was (8.4.x) | Now (9.0.0) |
|---|---|---|
| Library TFMs | `net6.0;net7.0;net8.0` | `net6.0;net7.0;net8.0;net9.0;net10.0` |
| Solution | `Cross.CQRS.EF.sln` | **`Cross.CQRS.EF.slnx`** |
| NuGet scripts | `_nuget/` helpers | removed; pack via CI / `Cross.CQRS.EF/config.nuspec` |
| License file | `LICENSE` | `LICENSE.md` (`requireLicenseAcceptance`) |

**Action:** restore against the published TFM groups in `config.nuspec`. Open `Cross.CQRS.EF.slnx` in supported Visual Studio / CLI tooling.
