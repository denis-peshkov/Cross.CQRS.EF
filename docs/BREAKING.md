# Breaking changes (NuGet consumers)

Breaking changes for **Cross.CQRS**, grouped by **from → to** package version.
Sections are **newest first** (top) → **oldest last** (bottom). When skipping releases, apply every intervening section **from oldest to newest** (bottom-up through the relevant range).

| Upgrade path | Section |
|---|---|
| `10.1.x` → `11.0.0+` | [From 10.1.x to 11.0.0](#from-101x-to-1100) |

Breaking-change details live **only** in this file. [`Cross.CQRS/config.nuspec`](../Cross.CQRS/config.nuspec) `releaseNotes` should link here and must not duplicate the versioned sections.

When shipping a new breaking change: insert a **From X.Y.Z to A.B.C** section **at the top** of the versioned sections (and a matching TOC row), and prefix the **PR title** with `BREAKING:`.

Historical renames before 11.0.0 (e.g. Event* → CommandEvent* in 9.1.0) are documented in [`docs/CHANGELOG.md`](CHANGELOG.md). Promote them into versioned sections here only when consumers still need a structured upgrade guide.

---

## From 10.1.x to 11.0.0

Release: [v11.0.0](https://github.com/denis-peshkov/Cross.CQRS/releases/tag/v11.0.0).

### JWT licensing (new pipeline gate)

| Area | Was (10.1.x) | Now (11.0.0+) |
|---|---|---|
| License | none | Optional `CqrsServiceConfiguration.LicenseKey` (JWT) |
| Validation | n/a | `LicenseValidator.Validate(license, ILicenseProductInfo)` |
| Pipeline | no license behavior | `LicenseCheckBehavior` on every MediatR request (order **-2**; **-1** reserved for Cross.CQRS.EF) |
| Product metadata | n/a | Default `LicenseProductInfo` (`Cross.CQRS` / types `Cross_CQRS` + `Cross_CQRS_EF`); extra `ILicenseProductInfo` via DI |

**Action:** hosts that enable licensing must supply a valid key (config / env). Extensions such as **Cross.CQRS.EF** register their own `ILicenseProductInfo`. Without a key, behavior follows the library’s “optional license” rules documented in README.

### Registration / FluentValidation

| Area | Was | Now |
|---|---|---|
| Validators | scanned from a narrower assembly set | `AddValidatorsFromAssemblies` uses the **full** assembly set from `CqrsServiceConfiguration` |

**Action:** ensure validator types live in assemblies passed to `AddCQRS` / configuration; mis-placed validators will not register.

### Handlers

| Area | Was | Now |
|---|---|---|
| `AsyncRequestHandlerBase` | present in earlier lines | **removed**; `CommandHandler<TCommand>` implements `IRequestHandler<TCommand>` (MediatR `Unit`) directly |

**Action:** custom handlers inheriting removed bases must implement MediatR interfaces directly (already required since 10.1.4 notes).

### Target frameworks

| Area | Was (typical 10.x) | Now (11.0.0) |
|---|---|---|
| Library TFMs | netstandard2.1 + net6–net10 (see prior notes) | `netstandard2.1;net6.0;net7.0;net8.0;net9.0;net10.0` |
| Extension packages | Microsoft.Extensions.* | Version per TFM (8.x / 9.0.14 / 10.0.5) as in `.csproj` |

**Action:** multi-target consumers should restore against the published TFM groups in `config.nuspec`.

### Packaging / solution

| Area | Was | Now |
|---|---|---|
| Solution | `Cross.CQRS.sln` | **`Cross.CQRS.slnx`** |
| NuGet scripts | `_nuget/` helpers | removed; pack via CI / `config.nuspec` |

**Action:** open `Cross.CQRS.slnx` in supported Visual Studio / CLI tooling.
