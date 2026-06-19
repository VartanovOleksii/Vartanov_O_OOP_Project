# EShop — Інтернет-магазин

Навчальний курсовий проект — повноцінний e-commerce застосунок із розділеними бекендом і фронтендом.

---

## Стек технологій

| Частина | Технології |
|---|---|
| **Бекенд** | ASP.NET Core (.NET 10), Entity Framework Core, SQLite, ASP.NET Core Identity, JWT Bearer |
| **Фронтенд** | SvelteKit 5, TypeScript, Tailwind CSS v4, shadcn-svelte, Zod, Axios |
| **Тести** | xUnit (бекенд), Vitest + Playwright (фронтенд) |

---

## Структура репозиторію

```
CourseWork/
├── Backend/
│   └── EShop/
│       ├── EShop.API/          # REST API (контролери, міграції, налаштування)
│       ├── EShop.Domain/       # Доменні сутності, інтерфейси, сервіси
│       └── EShop.Domain.Tests/ # Юніт-тести доменного шару
├── Frontend/                   # SvelteKit застосунок
├── Diagrams/                   # UML та UI-діаграми
└── Template/                   # Макети сторінок (PNG)
```

---

## Функціональність

### Ролі користувачів
- **Гість** — перегляд каталогу товарів
- **Покупець (Buyer)** — реєстрація, вхід, кошик, оформлення замовлень, профіль
- **Продавець (Seller)** — створення та редагування товарів, перегляд статистики продажів

### Основні можливості
- Реєстрація та автентифікація через JWT
- Каталог товарів із пошуком
- Кошик і оформлення замовлень
- Завантаження зображень товарів
- Статистика доходів продавця
- Експорт даних у JSON
- Темна/світла тема інтерфейсу

---

## Запуск проекту

### Вимоги
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) + [pnpm](https://pnpm.io/)

### Бекенд

```bash
cd Backend/EShop

# Задати JWT-ключ у appsettings.Development.json (мінімум 32 символи)
# "Jwt": { "Key": "your-secret-key-here-at-least-32-chars" }

dotnet run --project EShop.API
```

API запуститься на `https://localhost:7xxx` / `http://localhost:5xxx`.  
Swagger UI доступний за `/openapi` у режимі розробки.

### Фронтенд

```bash
cd Frontend

# Скопіювати приклад конфігурації
cp .env.example .env
# Вказати URL бекенду у .env:
# VITE_API_URL=http://localhost:5000

pnpm install
pnpm dev
```

Фронтенд запуститься на `http://localhost:5173`.

---

## Тести

```bash
# Бекенд
cd Backend/EShop
dotnet test

# Фронтенд
cd Frontend
pnpm test
```

---

## Діаграми

| Файл | Опис |
|---|---|
| [`Diagrams/Empty_UML_Diagram.drawio.png`](Diagrams/Empty_UML_Diagram.drawio.png) | UML-діаграма класів |
| [`Diagrams/UI_Diagram.drawio.png`](Diagrams/UI_Diagram.drawio.png) | Діаграма інтерфейсу |

Макети сторінок знаходяться в папці [`Template/`](Template/).

---

## API-ендпоінти (короткий огляд)

| Метод | Шлях | Опис |
|---|---|---|
| `POST` | `/api/auth/register` | Реєстрація |
| `POST` | `/api/auth/login` | Вхід (повертає JWT) |
| `GET` | `/api/products` | Список товарів |
| `POST` | `/api/products` | Створення товару (Seller) |
| `GET` | `/api/cart` | Кошик поточного користувача |
| `POST` | `/api/orders` | Оформлення замовлення |
| `GET` | `/api/seller/stats` | Статистика продавця |
| `GET` | `/api/account` | Профіль користувача |
