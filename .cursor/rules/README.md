# Правила проекта (Cursor Rules)

## Структура файлов

### 000-099: Глобальные правила
- `000-global.mdc` - Глобальные правила проекта
- `001-team-workflow.mdc` - Правила командной работы
- `002-multi-repo.mdc` - Правила работы с мульти-репозиторием
- `003-cursor-rules.mdc` - Как писать правила в `.cursor/rules`

### 100-199: Backend (.NET)
- `100-backend-dotnet.mdc` - Язык и формат C# (usings, naming, async, EditorConfig, логи)
- `101-backend-cqrs.mdc` - CQRS, модули, DTO, нормализация ввода
- `102-backend-efcore.mdc` - EF Core + SQL-миграции DbUp (не Code First Migrations): слои, именование, append-only, идемпотентные скрипты
- `103-backend-http.mdc` - HTTP-контракт API
- `104-backend-auth.mdc` - Аутентификация и авторизация
- `105-backend-security.mdc` - Безопасность бэкенда (валидация, санитизация, CORS, HTTPS)
- `106-backend-nuget.mdc` - NuGet multi-targeting, nuspec, BREAKING/CHANGELOG

### 200-299: Frontend (Angular)
- `200-frontend-angular-general.mdc` - Общие правила Angular
- `201-frontend-rxjs-only.mdc` - Правила работы с RxJS
- `202-frontend-state-stores-signals.mdc` - Управление состоянием: Stores и Signals
- `203-frontend-http.mdc` - Правила работы с HTTP
- `204-frontend-ui-tailwind.mdc` - Правила работы с UI и Tailwind CSS
- `205-frontend-i18n.mdc` - Правила интернационализации
- `206-frontend-forms-ugc.mdc` - Правила работы с формами
- `207-frontend-formatting-and-style.mdc` - Форматирование и стиль кода фронтенда
- `208-frontend-angular-routing.mdc` - Роутинг в Angular
- `209-frontend-guards.mdc` - Guards (Защита роутов)
- `210-frontend-error-handling.mdc` - Обработка ошибок
- `211-frontend-input-normalization.mdc` - Нормализация пользовательского ввода на клиенте (формы, validators)

### 300-399: Testing
- `300-testing-dotnet.mdc` - Тестирование .NET
- `301-testing-angular.mdc` - Тестирование Angular

### 400-499: Output Format
- `400-output-format.mdc` - Правила форматирования вывода
- `401-markdown.mdc` - Форматирование Markdown-таблиц
