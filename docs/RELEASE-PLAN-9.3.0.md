Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.3.0` · **ветка:** `release/9.3.0-nuspec-CrossCQRS-align` · **база:** `origin/master` (`v9.2.2`) · **дата:** `2026-09-18`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.3.0
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-9.2.2.md](RELEASE-PLAN-9.2.2.md)
>
> Дельта: `origin/master...HEAD` — **0** коммита · **0** файлов committed · working tree (plans/CHANGELOG + `.cursor` + nuspec). Open C/H/M/L пустые.

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

- SemVer: `GitVersion.yml` `next-version: 9.3.0`; теги `v9.2.1` / `v9.2.2` published. Цель — `v9.3.0` (не patch `9.2.3`).
- `SkipNetCoreApp31Tests` в release-plan / pr-message — только если свойство есть в test csproj (в этом репо нет).
- Maintainer kit (`.cursor` rules/skills/triage) — репозиторий, не NuGet.
- Нет секции `BREAKING.md` `From 9.2.2 to 9.3.0` (нет смены public API; #H26 был pack metadata).

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ #H26 nuspec Cross.CQRS | все `config.nuspec` groups → `11.3.1` (как `PackageReference` в csproj) |
| ✅ CHANGELOG 9.2.1 / 9.2.2 / 9.3.0 | секции newest-first; isolation tests перенесены из `v9.2.0` в `v9.2.1` |
| ✅ Version plans 9.2.1 / 9.2.2 | retroactive `(published / closed)` по tag + GitHub Release |
| ✅ to-master checklist | current target `9.3.0`; N1/G1/G2 cleared for #H26 |
| ✅ GitVersion /nofetch | `resolve-target-version.sh`: `/nofetch` + `MajorMinorPatch` |
| ✅ release-plan local test note | `SkipNetCoreApp31Tests` только при наличии свойства |
| ✅ pr-message repo-agnostic | build/test targets из template / discovery, без хардкода имени продукта |
| ✅ cursor README / 101-cqrs | generic wording (нет обязательного skill `db-scripts`; «CQRS pipeline» вместо бренда) |
| ✅ changelog / triage unit tests | generic sample/src paths |

---

## Что в библиотеке уже нормально

- Public API транзакций не менялся в working tree.
- Tag `v9.2.2` = текущий `origin/master`.
- Breaking по-прежнему только `8.4.1` → `9.0.0`.
- `Cross.CQRS` csproj + nuspec = **11.3.1**.

---

## Приоритет фиксов

Кросс-версионный backlog: [`TO-DO.md`](TO-DO.md).
