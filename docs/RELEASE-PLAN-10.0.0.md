Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `10.0.0` · **ветка:** `release/nuspec-CrossCQRS-align` · **база:** `origin/master` (`v9.2.2`) · **дата:** `2026-09-18`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v10.0.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-9.2.2.md](RELEASE-PLAN-9.2.2.md)
>
> Дельта: `origin/master...HEAD` — **2** коммита · **35** файлов · **+384 / −88**. Open C/H/M/L пустые.

**CodeRabbit:**
- не запускался.

**PR:** —

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

- Major `10.0.0` (вместо планируемого unpublished `9.3.0` / patch `9.2.3`): Cross.CQRS floor **11.3.1** + SemVer major (см. `docs/BREAKING.md` From 9.2.2 → 10.0.0).
- SampleWebApp / Tests: `sealed record` для commands / queries / events — host/sample style, не public API этого NuGet.
- Maintainer kit (`.cursor` rules/skills/triage) — репозиторий, не NuGet.
- `SkipNetCoreApp31Tests` — только если свойство есть в test csproj (в этом репо нет).
- Имя ветки `release/nuspec-CrossCQRS-align` — цифры в имени игнорируются GitVersion; SemVer = `next-version: 10.0.0`.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ #H26 nuspec Cross.CQRS | csproj + все nuspec groups = **11.3.1** |
| ✅ Cross.CQRS bump | PackageReference `11.2.0` → `11.3.1` |
| ✅ GitVersion 10.0.0 | `next-version: 10.0.0` |
| ✅ sealed record samples/tests | SampleWebApp + Tests commands/queries/events → `sealed record` |
| ✅ CHANGELOG / plans 9.2.1–9.2.2 | retroactive docs; `v10.0.0` секция в CHANGELOG |
| ✅ BREAKING 9.2.2 → 10.0.0 | секция + TOC в `docs/BREAKING.md` |
| ✅ GitVersion /nofetch | `resolve-target-version.sh`: `/nofetch` + `MajorMinorPatch` |
| ✅ release-plan / pr-message | local test note (`SkipNetCoreApp31Tests`), repo-agnostic discovery |
| ✅ cursor README / 101-cqrs / unit tests | generic wording и paths |

---

## Что в библиотеке уже нормально

- Public API транзакций (`AddEntityFrameworkIntegration`, `UnifiedTransactionBehavior`, `ExactTransaction`) в дельте не менялся.
- TFMs net6–net10; EF patch-набор как в `9.2.0`.
- `config.nuspec` `releaseNotes` — только ссылки CHANGELOG / BREAKING.

---

## Приоритет фиксов

1. Локальный `dotnet build` / `dotnet test` перед publish.
2. Tag `v10.0.0` + NuGet push (OIDC) + GitHub Release.
3. Кросс-версионный backlog: [`TO-DO.md`](TO-DO.md).
