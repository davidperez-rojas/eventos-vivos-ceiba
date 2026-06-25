import { Routes } from '@angular/router';
import {adminGuard, authGuard} from './core/guards/role-guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/role-selector/role-selector')
        .then(m => m.RoleSelector)
  },
  {
    path: 'eventos',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/events/list/list')
        .then(m => m.List)
  },
  {
    path: 'eventos/crear',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/events/create/create')
        .then(m => m.Create)
  },
  {
    path: 'eventos/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/events/detail/detail')
        .then(m => m.Detail)
  },
  {
    path: 'eventos/:id/reservar',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/reservations/create/create')
        .then(m => m.Create)
  },
  {
    path: 'reservas/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/reservations/manage/manage')
        .then(m => m.Manage)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
