Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.2.2` (published / closed) · **ветка:** `master` · **база:** `v9.2.1` · **дата:** `2026-09-18`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.2.2
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-9.2.1.md](RELEASE-PLAN-9.2.1.md)
>
> Дельта: `v9.2.1...v9.2.2` — **1** коммита · **11** файлов · **+181 / −30**. Open C/H/M/L пустые (план закрыт).

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

- NuGet push с `master` / `release/*` / `hotfix/*` / `dev` — OIDC `NuGet/login`, не secret `NUGET_API_KEY`.
- Снимок GitHub labels в `.github/LABELS.yml` + `.github/LABELS.md` — репозиторий, не пакет.
- Maintainer kit (`.cursor` triage / `pr-message`) — не NuGet.
- `Cross.CQRS` PackageReference `11.2.0` без секции `BREAKING.md` (нет смены public API этого пакета); расхождение nuspec → закрыто в `9.3.0` #H26.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ Cross.CQRS csproj 11.2.0 | `PackageReference` `11.2.0`; nuspec groups остались `11.1.2` (закрыто #H26 в `9.3.0`) |
| ✅ NuGet OIDC login | `.github/workflows/dotnet.yml`: `id-token: write` + `NuGet/login@v1` → push |
| ✅ GitHub labels snapshot | `.github/LABELS.yml` / `.github/LABELS.md` |
| ✅ triage label apply | `apply-pr-labels` + comment template |
| ✅ pr-message skill | правки skill в этой дельте |

---

## Что в библиотеке уже нормально

- Public API транзакций не менялся.
- Isolation tests из `9.2.1` на месте.
- Breaking по-прежнему только `8.4.1` → `9.0.0`.

---

## Приоритет фиксов

_(пусто — релиз `9.2.2` опубликован; открытый backlog → [`TO-DO.md`](TO-DO.md).)_
