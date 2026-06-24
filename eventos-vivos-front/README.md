**¿Por qué feature/core/shared?**
Mantiene las responsabilidades separadas a escala. Core contiene los servicios singleton y modelos. Las features son autocontenidas y se cargan de forma lazy. Shared contiene únicamente componentes de UI reutilizables sin lógica de negocio.

---

## Reglas de Negocio Implementadas

| Regla | Descripción |
|-------|-------------|
| RN-01 | Un evento no puede superar la capacidad del venue |
| RN-02 | No se permiten eventos con horarios superpuestos en el mismo venue |
| RN-03 | Los eventos en fin de semana no pueden iniciar después de las 22:00 |
| RN-04 | No se permiten reservas con menos de 1 hora para el inicio del evento |
| RN-05 | Eventos con precio mayor a $100 tienen un límite de 10 entradas por transacción |
| RN-06 | Los eventos se marcan automáticamente como completados al superar su hora de fin |
| RN-07 | Las cancelaciones con menos de 48 horas para el evento marcan las entradas como "perdidas" (no se liberan para venta) |

> **Nota de prioridad (RF-03):** La regla de 24 horas (máx. 5 entradas) tiene prioridad sobre la regla de precio (máx. 10 entradas) cuando ambas aplican simultáneamente.
---

## Ejecución Local

### Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

### 1 — Clonar el repositorio

```bash
git clone https://github.com/TU_USUARIO/eventos-vivos.git
cd eventos-vivos
```

### 2 — Ejecutar el Backend

```bash
cd eventos-vivos-back
dotnet run --project EventosVivos.API/EventosVivos.API.csproj
```

API disponible en: `http://localhost:5217`
Swagger UI: `http://localhost:5217/swagger`

> La base de datos se inicializa automáticamente con los 3 venues al arrancar.

### 3 — Ejecutar el Frontend

```bash
cd eventos-vivos-front
npm install
ng serve
```

Aplicación disponible en: `http://localhost:4200`

---

## Endpoints de la API

| Método | Endpoint                            | Descripción                   |
|--------|-------------------------------------|-------------------------------|
| POST   | `/api/Events`                       | Crear evento                  |
| GET    | `/api/Events`                       | Listar eventos (con filtros)  |
| GET    | `/api/Events/{id}`                  | Obtener evento por ID         |
| GET    | `/api/Events/{id}/report`           | Reporte de ocupación          |
| POST   | `/api/Reservations`                 | Crear reserva                 |
| GET    | `/api/Reservations/{id}`            | Obtener reserva por ID        |
| PUT    | `/api/Reservations/{id}/confirm`    | Confirmar pago                |
| PUT    | `/api/Reservations/{id}/cancel`     | Cancelar reserva              |
| GET    | `/api/Reservations/event/{eventId}` | Obtener reservas de un evento |

### Parámetros de filtro para `GET /api/Events`

| Parámetro  | Tipo | Descripción |
|------------|------|-------------|
| `title`    | string | Búsqueda parcial, sin distinción de mayúsculas |
| `type`     | string | `conferencia`, `taller`, `concierto` |
| `status`   | string | `activo`, `cancelado`, `completado` |
| `venueId`  | int | Filtrar por venue |
| `dateFrom` | datetime | Rango de fecha de inicio (desde) |
| `dateTo`   | datetime | Rango de fecha de inicio (hasta) |

---

## Ejecución de Tests

```bash
cd eventos-vivos-back
dotnet test
```

### Cobertura de pruebas

| Prueba | Regla |
|--------|-------|
| Crear evento con datos válidos | RF-01 |
| Tipo de evento inválido | RF-01 validación |
| Fecha de inicio en el pasado | RF-01 validación |
| Fecha de fin anterior al inicio | RF-01 validación |
| Evento en fin de semana después de las 22:00 | RN-03 |
| Superposición de venue | RN-02 |
| Evento marcado automáticamente como completado | RN-06 |
| Reserva válida | RF-03 |
| Evento no encontrado | RF-03 validación |
| Evento no activo | RF-03 validación |
| Menos de 1 hora para el inicio | RN-04 |
| Precio > $100 con más de 10 entradas | RN-05 |
| Regla de 24h tiene prioridad sobre la de precio | RF-03 prioridad |
| Sin disponibilidad | RN-01 |
| Confirmar pago válido | RF-04 |
| Confirmar reserva ya confirmada | RF-04 validación |
| Confirmar reserva cancelada | RF-04 validación |
| Cancelar reserva confirmada | RF-05 |
| Cancelar con penalización (menos de 48h) | RN-07 |
| Cancelar reserva en pendiente de pago | RF-05 validación |
| Cancelar reserva ya cancelada | RF-05 validación |

---

## Datos de Referencia — Venues

| ID | Nombre | Capacidad | Ciudad |
|----|--------|-----------|--------|
| 1 | Auditorio Central | 200 | Bogotá |
| 2 | Sala Norte | 50 | Bogotá |
| 3 | Arena Sur | 500 | Medellín |

---

## Estructura del Proyecto
eventos-vivos/

├── eventos-vivos-back/

│   ├── EventosVivos.API/

│   └── EventosVivos.Tests/

└── eventos-vivos-front/
