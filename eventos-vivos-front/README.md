# EventosVivos — Frontend Angular 21

Aplicación web para la gestión de eventos y reservas. Consume la API REST de EventosVivos.

---

## Tecnologías

- **Angular 21** — Standalone components + Zoneless
- **TypeScript**
- **SCSS**
- **Vitest** — Tests unitarios
- **FontAwesome** — Iconografía

---

## Arquitectura — Feature/Core/Shared

```
src/app/
├── core/
│   ├── models/
│   │   ├── event.model.ts         # Interfaces de Event, CreateEventRequest, EventFilter
│   │   ├── reservation.model.ts   # Interfaces de Reservation, CreateReservationRequest
│   │   ├── venue.model.ts         # Interface de Venue
│   │   └── report.model.ts        # Interface de OccupancyReport
│   ├── services/
│   │   ├── event.service.ts       # HTTP calls a /api/events
│   │   ├── reservation.service.ts # HTTP calls a /api/reservations
│   │   └── venue.service.ts       # Datos de referencia de venues
│   └── interceptors/
│       └── error.interceptor.ts   # Interceptor global de errores HTTP
├── features/
│   ├── events/
│   │   ├── event-list/            # Lista de eventos con filtros
│   │   ├── create-event/          # Formulario crear evento
│   │   └── event-detail/          # Detalle + reporte + lista de reservas
│   └── reservations/
│       ├── create-reservation/    # Formulario reservar entradas
│       └── manage-reservation/    # Confirmar pago / Cancelar
└── shared/
    └── components/
        └── badge-estado/          # Badge reutilizable de estado
```

---

## Decisiones técnicas

### Standalone Components
Cada componente declara sus propias dependencias en el array `imports` sin necesitar NgModule. Permite lazy loading nativo por ruta.

### Zoneless
Usa `provideZonelessChangeDetection()` en lugar de Zone.js. La detección de cambios se basa en signals, lo que mejora el rendimiento.

### Signals
Estado reactivo con `signal<T>()`. Cuando un signal cambia, Angular re-renderiza solo los templates que lo leen.

```typescript
events = signal<Event[]>([]);      // crear
events.set(data);                  // actualizar
events()                           // leer
```

### Formularios reactivos
`FormBuilder` con `Validators`. Los errores se muestran solo cuando el campo es `invalid && touched`.

### Error Interceptor
Intercepta todas las respuestas HTTP y extrae el mensaje de error del backend para mostrarlo al usuario.

---

## Pantallas

| Ruta | Componente | Descripción |
|------|-----------|-------------|
| `/events` | EventListComponent | Lista con filtros en tiempo real |
| `/events/create` | CreateEventComponent | Formulario con validaciones frontend |
| `/events/:id` | EventDetailComponent | Detalle + reporte de ocupación + reservas |
| `/events/:id/reserve` | CreateReservationComponent | Formulario de reserva |
| `/reservations/:id` | ManageReservationComponent | Confirmar pago o cancelar |

---

## Validaciones en Frontend

Además de las validaciones del backend, el frontend valida:

- Fecha de inicio debe ser futura
- Fecha de fin debe ser posterior al inicio
- Eventos en fin de semana no pueden iniciar después de las 22:00 (RN-03)
- Capacidad máxima no puede exceder la del venue seleccionado (RN-01)

---

## Ejecutar

```bash
npm install
ng serve
```

App disponible en `http://localhost:4200`

---

## Tests

```bash
# Ejecutar una vez
npm test

# Modo watch
npm run test:watch

# Con cobertura
npm run test:coverage
```

### Cobertura de tests

| Archivo | Pruebas |
|---------|---------|
| `event.service.spec.ts` | GET todos, GET con filtros, GET por id, POST crear, GET reporte |
| `reservation.service.spec.ts` | POST crear, GET por id, PUT confirmar, PUT cancelar, cancelar con penalización |
| `venue.service.spec.ts` | 3 venues con datos correctos |
| `create-event.component.spec.ts` | Validaciones frontend, flujo feliz, errores de API |

---

## Variables de entorno

Las URLs del backend están configuradas directamente en los servicios:

```
core/services/event.service.ts       → baseUrl del backend
core/services/reservation.service.ts → baseUrl del backend
```

---

## Build para producción

```bash
ng build
```

Output en `dist/eventos-vivos-front/browser/`
