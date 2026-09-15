Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `11.1.0` (published / closed) · **ветка:** `release/11.1.0-new-license-improve-functionality` · **база:** `origin/master` (`v10.1.3`) · **дата:** `2026-09-13`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS/releases/tag/v11.1.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** —
>
> Дельта: `origin/master...HEAD` — **98** коммита · **140** файлов · **+8061 / −416**. Open C/H/M/L пустые (план закрыт).

**CodeRabbit:** `2026-09-13` · log `.cursor/skills/coderabbit/.cache/cr-release-11.1.0-new-license-improve-functionality-vs-origin-master-all-20260913-001057.jsonl` · 13 findings (0 Critical, 6 Major, 7 Minor) → все закрыты в этом плане.

**PR:** [#20](https://github.com/denis-peshkov/Cross.CQRS/pull/20) (`BREAKING:` Cross.CQRS 11.1.0 — JWT licensing, pipeline/docs hygiene, CI & triage).

---

## Критично (безопасность)

---

## Высокий (логика / auth model)

---

## Средний (противоречия / баги контрактов)

---

## Низкий (техдолг / несогласованности)

---

## Принято (осознанный trade-off)

- SampleWebApp / Tests: `CA2007` в `NoWarn` (host/test style); в библиотеке — `ConfigureAwait(false)`.
- Sibling EF-пакет (отдельный репозиторий): в core только интеграционные хуки (`InternalsVisibleTo`, product claim / filter, pipeline −1) — не часть NuGet description этого пакета.
- Матрица тестов держит **netcoreapp3.1**, чтобы гонять сборку библиотеки **netstandard2.1** (`SkipNetCoreApp31Tests` без x64 3.1 host).
- Лицензия опциональна: без ключа — правила «optional license» из README.
- Tag + NuGet Push только с `master` / `release/*` / `hotfix/*` / `dev`.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ #H1 LicenseCheck every request | accepted: by design — validate on every MediatR request (CR major skipped as dup) |
| ✅ #H2 SampleWebApp LicenseKey | fixed: placeholder `"<license key here>"` as in README; real JWT removed from sample |
| ✅ #H3 resolve-target-version bump rules | fixed: любая ветка → GitVersion `MajorMinorPatch`; ручной override `--version` |
| ✅ #H4 collect-data.sh JSONL files | fixed: `gh --jq` → `{number, files: [paths]}` (JSON-safe array) |
| ✅ #H5 post-pr-triage comment upsert | fixed: lookup by `TRIAGE_MARKER` + `github-actions[bot]` (CI token author) |
| ✅ #H6 post-pr-triage labels gate | fixed: `TRIAGE_APPLY_LABELS` default on (`1`), opt-out `false`; confidence floor (default 70) + allowlist; comment always suggests |
| ✅ #M1 README license check frequency | fixed: removed redundant «first/every» pipeline blurb; frequency only in LicenseKey section |
| ✅ #M2 scaffold-breaking-section skip resolve | fixed: both `--from` + `--to`/`--version` → scaffold without GitVersion |
| ✅ #M3 release-plan-summary missing line | fixed: missing `**Checklist summary:**` → insert; «up to date» only when present+equal |
| ✅ #M4 queue process continues after publish fail | fixed: two StandardFlow events, first Publish throws — both attempted/Published, handler result ok |
| ✅ #M5 BehaviorCollection duplicate AddBehavior | fixed: re-Add same type updates order (`[type]=order`); test asserts single descriptor |
| ✅ #M6 queue keeps exception-safe for targetId | fixed: after Standard read, assert ExceptionSafe for same targetId still readable |
| ✅ #M7 post-pr-triage oversized patch | fixed: skip too-large chunk (`continue`), keep packing smaller later patches |
| ✅ #L5 nuspec tags + releaseNotes | fixed: tags clarify JWT/AddCQRS; short v11 blurb + CHANGELOG/BREAKING links |
| ✅ #L4 nuspec releaseNotes trim | fixed: short v11 blurb + links to CHANGELOG/BREAKING; no «3.1 dropped» / EF marketing dump |
| ✅ #L3 CA2007 library | `ConfigureAwait(false)` на await в библиотеке |
| ✅ #L2 NuGet publish secret | `NUGET_API_KEY` обновлён (ops); CI push больше не блокируется этим 403 |
| ✅ #L1 ReleaseNotes vs test TFMs | Notes: netcoreapp3.1 kept to exercise netstandard2.1 (not dropped) |
| ✅ JWT licensing pipeline | `LicenseKey`, `LicenseAccessor`, `LicenseValidator.Validate(license, ILicenseProductInfo)`, `LicenseCheckBehavior` (−2), product metadata DI |
| ✅ AddCQRS registration | `CqrsServiceConfiguration`; FluentValidation `AddValidatorsFromAssemblies` по полному набору сборок |
| ✅ TFMs / deps | `netstandard2.1;net6–net10`; Extensions.* по TFM |
| ✅ Solution / packaging | `Cross.CQRS.slnx`; `config.nuspec`; `_nuget` убран; `LICENSE.md` |
| ✅ Docs consumers | `docs/BREAKING.md` From 10.1.x→11.1.0; `docs/CHANGELOG.md` `## v11.1.0` |
| ✅ Tests NUnit | зоны Licensing / Registration / Behaviors / Queue / Core |
| ✅ CI tag/NuGet gates | `startsWith` для release/hotfix; publish = master/release/hotfix/dev |
| ✅ Sonar key aligned | `projectKey=Cross.CQRS` в workflow + README/CONTRIBUTING |
| ✅ PR / issues templates | breaking → `docs/BREAKING.md`; placeholders `x.y.z`; legacy template удалён |
| ✅ Package description | nuspec + CI Description без маркетинга sibling EF |
| ✅ triage.yml secrets-in-if / fork path | fixed: no `secrets` in `if`; same-repo `pull_request` + fork `pull_request_target` (checkout `base.sha` only) |

---

## Что в библиотеке уже нормально

- MediatR pipeline: license (−2) / reserved −1 / filters / validation согласованы с кодом.
- Breaking 10.1.x→11.1.0 — только в `docs/BREAKING.md`.
- Local Release build/tests (net6–net10) ранее зелёные на ветке.
- Version plan `11.1.0` закрыт; publish gate остаётся в [`RELEASE-PLAN-to-master.md`](RELEASE-PLAN-to-master.md).

---

## Приоритет фиксов

_(пусто — релиз `11.1.0` опубликован; открытый backlog → [`TO-DO.md`](TO-DO.md).)_
