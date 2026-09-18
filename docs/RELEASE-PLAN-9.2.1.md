Ниже — **проблемы внутри библиотеки**, по уровню критичности. Аудит по дельте ветки относительно базовой ветки (обычно `master`).

> **Версия:** `9.2.1` (published / closed) · **ветка:** `master` · **база:** `v9.2.0` · **дата:** `2026-09-16`
>
> **Релиз (если есть):** https://github.com/denis-peshkov/Cross.CQRS.EF/releases/tag/v9.2.1
>
> **Легенда:** ⬜ open · ✅ done · 🟨 partial / принято · ❌ blocker
>
> **Предыдущий план:** [RELEASE-PLAN-9.2.0.md](RELEASE-PLAN-9.2.0.md)
>
> Дельта: `v9.2.0...v9.2.1` — **1** коммита · **10** файлов · **+202 / −46**. Open C/H/M/L пустые (план закрыт).

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

- Isolation-level coverage только в `Cross.CQRS.EF.Tests` — consumer API / `BREAKING.md` не менялись.
- Прямой коммит в `master` (без PR) для этого патча.

---

## Закрыто (проверено в коде)

| # | Суть |
|---|---|
| ✅ IsolationLevelIntegrationTests | commit/rollback, Scope / TransactionalScope ambient, `ExactTransaction` overrides ReadUncommitted–Serializable |
| ✅ ExactTransactionIsolationProbes | общий probe-хелпер для матрицы isolation |
| ✅ README wiki TODO | struck through «Add integration tests for different isolation levels» |
| ✅ CHANGELOG isolation tests | секция `v9.2.1`; bullet убран из `v9.2.0` (docs pass `9.3.0`) |

---

## Что в библиотеке уже нормально

- Public API транзакций (`AddEntityFrameworkIntegration`, `UnifiedTransactionBehavior`, `ExactTransaction`) не менялся.
- Cross.CQRS **11.1.2**; EF patch-набор как в `9.2.0`.
- Breaking по-прежнему только `8.4.1` → `9.0.0`.

---

## Приоритет фиксов

_(пусто — релиз `9.2.1` опубликован; открытый backlog → [`TO-DO.md`](TO-DO.md).)_
