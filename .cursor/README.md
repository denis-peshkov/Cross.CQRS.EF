# `.cursor` — обзор

Каталог настроек Cursor для репозитория: правила, skills, triage/CI.

## Содержимое

| Путь | Назначение |
|---|---|
| [`rules/`](rules/) | Cursor Rules (`.mdc`) — стиль, архитектура, security, тесты |
| [`skills/`](skills/) | Agent skills (triage, release-plan, coderabbit, …) |
| [`triage/`](triage/) | GitHub triage: `gh` wrapper, CI runners, отчёты |

Подробнее по triage: [`triage/docs/README.md`](triage/docs/README.md).  
Каталог rules (только `.mdc`): [`rules/README.md`](rules/README.md).

## Триаж GitHub (issues / PRs)

- `.cursor/skills/triage/` — оркестратор issue + PR
- `.cursor/skills/triage-issue/` — GitHub issues
- `.cursor/skills/triage-pr/` — GitHub PRs (Phase 2 / review templates)
- `.cursor/triage/` — `gh-wrapper`, `collect-data.sh`, `post-pr-triage.mjs`

Правило `003-triage.mdc` больше нет — канон в skills + скриптах.

### Guidance для automated PR triage

`post-pr-triage.mjs` → `loadReviewChecklists`:

1. Матчит `.cursor/rules/*.mdc` по frontmatter (`alwaysApply: true` или `globs` ↔ пути файлов PR).
2. Если ни одно rule не совпало — fallback на `.cursor/skills/triage-pr/references/*-checklist.md` (EF → core → angular).

Логика: [`triage/load-review-rules.mjs`](triage/load-review-rules.mjs).

## Skills (проект)

- `skills/release-plan/` — планы версий, `TO-DO.md`, `BREAKING.md`
- `skills/coderabbit/` — CodeRabbit CLI → текущий RELEASE-PLAN
- `skills/db-scripts/` — DbUp / SQL scripts (соглашения также в `rules/102-backend-efcore.mdc`)
- `skills/stripe-accounts/`, `skills/stripe-products/`, `skills/translate-resources/` — по необходимости продукта
