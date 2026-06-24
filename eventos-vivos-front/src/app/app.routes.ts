import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'eventos',
    pathMatch: 'full'
  },
  {
    path: 'eventos',
    loadComponent: () =>
      import('./features/events/list/list')
        .then(m => m.List)
  },
  {
    path: 'eventos/crear',
    loadComponent: () =>
      import('./features/events/create/create')
        .then(m => m.Create)
  },
  {
    path: 'eventos/:id',
    loadComponent: () =>
      import('./features/events/detail/detail')
        .then(m => m.Detail)
  },
  {
    path: 'eventos/:id/reservar',
    loadComponent: () =>
      import('./features/reservations/create/create')
        .then(m => m.Create)
  },
  {
    path: 'reservas/:id',
    loadComponent: () =>
      import('./features/reservations/manage/manage')
        .then(m => m.Manage)
  },
  {
    path: '**',
    redirectTo: 'eventos'
  }
];
