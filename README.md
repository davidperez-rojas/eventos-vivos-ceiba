# EventosVivos

Sistema de reservas de eventos culturales, conferencias y talleres. Desarrollado como prueba técnica fullstack con **.NET 10** y **Angular 21**.

---

## Descripción del Problema

EventosVivos es una startup que organiza eventos culturales, conferencias y talleres. Actualmente gestionan todo mediante hojas de cálculo y formularios en papel, lo que genera:

- Venta de más entradas que la capacidad del venue por falta de control en tiempo real
- Conflictos de horarios cuando un venue tiene múltiples eventos
- Horas perdidas validando manualmente reservas y pagos

Este sistema resuelve estos problemas mediante un núcleo de reservas con validaciones automáticas y reglas de negocio estrictas.

---

## Stack Tecnológico

| Capa | Tecnología | Versión |
|------|-----------|---------|
| Backend | .NET Web API | 10 |
| ORM | Entity Framework Core InMemory | 9+ |
| Frontend | Angular Standalone + Zoneless | 21 |
| Tests Backend | xUnit + Moq + FluentAssertions | Última |
| Tests Frontend | Vitest + Angular TestBed | Última |
| Documentación API | Swagger / Swashbuckle | Última |

---

## Arquitectura

### Backend — Arquitectura por Capas

```
Request HTTP
     ↓
Controllers (Presentation)   → Recibe HTTP, valida formato, retorna status codes
     ↓
BL — Business Logic Layer    → Aplica reglas de negocio
     ↓
DAO — Data Access Layer      → Queries a la base de datos con EF Core
     ↓
EF Core InMemory Database
```

**Estructura de carpetas:**

```
EventosVivos.API/
├── Controllers/       # Endpoints REST
├── BL/                # Lógica de negocio
│   └── Interfaces/
├── DAO/               # Acceso a datos
│   └── Interfaces/
├── DTOs/
│   ├── Requests/      # Contratos de entrada
│   └── Responses/     # Contratos de salida
├── Models/            # Entidades EF Core
├── Data/              # AppDbContext + seed
└── Middlewares/       # Manejo global de errores
```

**¿Por qué arquitectura por capas?**

Cada capa tiene una única responsabilidad y depende solo de la capa inmediatamente inferior a través de interfaces. Esto permite:
- **Testabilidad**: los tests mockean las interfaces con Moq sin necesitar base de datos real
- **Desacoplamiento**: cambiar de InMemory a PostgreSQL solo requiere modificar la capa DAO, sin tocar BL ni Controllers
- **Mantenibilidad**: cada cambio de negocio tiene un lugar claro donde hacerse

### Frontend — Feature/Core/Shared

```
src/app/
├── core/
│   ├── models/        # Interfaces TypeScript
│   ├── services/      # Clientes HTTP
│   └── interceptors/  # Manejo global de errores HTTP
├── features/
│   ├── events/        # Lista, crear, detalle
│   └── reservations/  # Crear, gestionar
└── shared/
    └── components/    # BadgeStatus (reutilizable)
```

**¿Por qué feature/core/shared?**

- **Core**: servicios singleton y modelos compartidos por toda la app
- **Features**: módulos autocontenidos y lazy-loaded por ruta
- **Shared**: componentes UI reutilizables sin lógica de negocio

---

## Requerimientos Funcionales Implementados

| RF | Descripción | Endpoint |
|----|-------------|---------|
| RF-01 | Crear evento con validaciones | `POST /api/events` |
| RF-02 | Listar eventos con filtros | `GET /api/events` |
| RF-03 | Reservar entrada | `POST /api/reservations` |
| RF-04 | Confirmar pago de reserva | `PUT /api/reservations/{id}/confirm` |
| RF-05 | Cancelar reserva | `PUT /api/reservations/{id}/cancel` |
| RF-06 | Reporte de ocupación | `GET /api/events/{id}/report` |

---

## Reglas de Negocio Implementadas

| Regla | Descripción |
|-------|-------------|
| RN-01 | Un evento no puede exceder la capacidad del venue |
| RN-02 | No se permiten eventos con horarios superpuestos en el mismo venue |
| RN-03 | Los eventos en fin de semana no pueden iniciar después de las 22:00 |
| RN-04 | No se permiten reservas con menos de 1 hora para el inicio del evento |
| RN-05 | Eventos con precio mayor a $100 tienen un límite de 10 entradas por transacción |
| RN-06 | Los eventos se marcan automáticamente como completados al superar su hora de fin |
| RN-07 | Las cancelaciones con menos de 48 horas para el evento marcan las entradas como perdidas |

> **Prioridad RF-03:** La regla de 24 horas (máx. 5 entradas) tiene prioridad sobre la regla de precio (máx. 10 entradas) cuando ambas aplican simultáneamente.

---

## Datos de Referencia — Venues

| ID | Nombre | Capacidad | Ciudad |
|----|--------|-----------|--------|
| 1 | Auditorio Central | 200 | Bogotá |
| 2 | Sala Norte | 50 | Bogotá |
| 3 | Arena Sur | 500 | Medellín |

> Los venues se inicializan automáticamente al arrancar la aplicación.

---

## Ejecutar Localmente

### Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

### 1 — Clonar el repositorio

```bash
git clone https://github.com/davidperez-rojas/eventos-vivos-ceiba.git
cd eventos-vivos
```

### 2 — Backend

```bash
cd eventos-vivos-back
dotnet run --project EventosVivos.API/EventosVivos.API.csproj
```

- API: `http://localhost:5217`
- Swagger: `http://localhost:5217/swagger`

### 3 — Frontend

```bash
cd eventos-vivos-front
npm install
ng serve
```

- App: `http://localhost:4200`

### 4 — Tests Backend

```bash
cd eventos-vivos-back
dotnet test
```

### 5 — Tests Frontend

```bash
cd eventos-vivos-front
npm test
```

---

## Endpoints de la API

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST   | `/api/events` | Crear evento |
| GET    | `/api/events` | Listar eventos con filtros |
| GET    | `/api/events/{id}` | Detalle de evento |
| GET    | `/api/events/{id}/report` | Reporte de ocupación |
| POST   | `/api/reservations` | Crear reserva |
| GET    | `/api/reservations/{id}` | Detalle de reserva |
| GET    | `/api/reservations/event/{eventId}` | Reservas por evento |
| PUT    | `/api/reservations/{id}/confirm` | Confirmar pago |
| PUT    | `/api/reservations/{id}/cancel` | Cancelar reserva |
| GET    | `/api/Reservations/event/{eventId}	` | Obtener reservas de un evento |

### Filtros disponibles para `GET /api/events`

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `title` | string | Búsqueda parcial, sin distinción de mayúsculas |
| `type` | string | `conferencia`, `taller`, `concierto` |
| `status` | string | `activo`, `cancelado`, `completado` |
| `venueId` | int | Filtrar por venue |
| `dateFrom` | datetime | Rango de fecha de inicio (desde) |
| `dateTo` | datetime | Rango de fecha de inicio (hasta) |

---

## Despliegue

| Servicio | URL |
|---------|-----|
| Backend (Railway) | `https://eventos-vivos-ceiba-production.up.railway.app/` |
| Frontend (Vercel) | `https://eventos-vivos-ceiba-om9p.vercel.app/eventos` |

---

## Estructura del Proyecto

```
eventos-vivos/
├── eventos-vivos-back/
│   ├── EventosVivos.API/
│   └── EventosVivos.Tests/
├── eventos-vivos-front/
└── .gitignore
```
