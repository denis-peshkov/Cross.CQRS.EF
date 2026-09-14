# PR Review Checklist — EF extension

Use for deep review of PRs in `triage-pr` and `bugbot` for an **EF / DbContext integration** package (sibling to a core CQRS library). Paths below are examples — prefer the PR delta / solution layout. Core CQRS checklist: `dotnet-checklist.md`.

## Security & Licensing (critical)

- No logging of real license JWTs, private keys, or production secrets (incl. sample hosts)
- EF license-check behavior keeps its reserved pipeline order relative to the core license behavior; see `docs/BREAKING.md`
- Startup / hosted license gate (if present) still runs and is registered with the EF integration entry point
- Product metadata for the EF package stays consistent with core licensing docs when touched
- Do not commit live sample license keys

## Public API & Registration

- EF registration stays on the CQRS registration syntax (not a raw `IServiceCollection` shortcut that bypasses the designed API)
- Registration still wires options, EF license behavior, transaction behavior, DbContext provider, and hosted license validator as designed
- Extension-method defaults vs options-class defaults stay intentional and documented when changed
- XML docs / remarks match actual behavior (no stale references to removed APIs or wrong pipeline orders)
- Public surface changes → `docs/BREAKING.md`, `docs/CHANGELOG.md`, and short `config.nuspec` `releaseNotes` (summary + link, not a full duplicate)

## Transactions & DbContext

- Unified transaction behavior wraps **commands** only; queries must not pick up unintended transaction wrappers
- Transaction strategies and isolation mapping stay correct
- Per-handler transaction override attribute (on the **handler class**) still honored
- Changes to transaction enums / options / attributes = consumer-facing — semver + BREAKING
- Side effects (e.g. detach of tracked entities, timeouts) remain deliberate and covered by tests when changed

## Packaging & Multi-targeting

- Library `TargetFrameworks` match `config.nuspec` `lib\` groups and `<dependencies>` groups
- Per-TFM package versions in `.csproj` match nuspec (core CQRS, EF Core, EF Relational, Hosting abstractions, and any other declared deps)
- Pack files still include Release binaries and documented pack assets (README / icon / LICENSE as required by the pack flow)
- Do not reintroduce removed public APIs without an explicit product decision and docs/nuspec sync

## .NET & Code Style

- Prefer project conventions: `GlobalUsings.cs`, `.editorconfig`, UTF-8 with BOM for `.cs` / `.csproj` / `.sln` / `.slnx`
- Library awaits: `ConfigureAwait(false)` where CA2007 applies; sample/test projects may NoWarn
- `Async` suffix on async methods; prefer Given/When/Then test names; async tests end with `Async`

## Tests

- New/changed EF transaction, isolation, concurrency, event, DI, or license behavior covered in the test project (zones from the solution layout)
- Prefer pipeline/integration coverage when changing EF registration, behaviors, or per-handler transaction overrides — not only direct handler calls
- Test TFMs stay aligned with the library; keep TFM-specific polyfills only where still required
- Run: `dotnet test <TestProject>/<TestProject>.csproj` (path from the solution)
- **Do not** require XML `/// <summary>` on test methods

## Breaking Changes

- Public NuGet API / registration / transaction surface / licensing — semver impact → `docs/BREAKING.md`
- Prefix PR title with `BREAKING:` when shipping a documented breaking change
