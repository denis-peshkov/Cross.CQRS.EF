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
- `200-frontend-angular.mdc` - Язык и формат Angular
- `201-frontend-rxjs-only.mdc` - RxJS
- `202-frontend-state-stores-signals.mdc` - Stores и Signals
- `203-frontend-http.mdc` - HTTP (ApiService)
- `204-frontend-ui-tailwind.mdc` - UI и Tailwind
- `205-frontend-i18n.mdc` - Интернационализация
- `206-frontend-forms.mdc` - Формы и нормализация ввода
- `207-frontend-routing.mdc` - Роутинг и guards
- `208-frontend-errors.mdc` - Обработка ошибок

### 300-399: Testing
- `300-testing-dotnet.mdc` - Тестирование .NET
- `301-testing-angular.mdc` - Тестирование Angular

### 400-499: Output Format
- `400-output-format.mdc` - Формат ответа агента
- `401-markdown.mdc` - Форматирование Markdown-таблиц
