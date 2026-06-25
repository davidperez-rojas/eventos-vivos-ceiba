import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { RoleService } from '../services/role.service';

export const authGuard: CanActivateFn = () => {
  const roleService = inject(RoleService);
  const router = inject(Router);

  roleService.loadFromSession();

  if (!roleService.hasRole()) {
    router.navigate(['/']);
    return false;
  }
  return true;
};

export const adminGuard: CanActivateFn = () => {
  const roleService = inject(RoleService);
  const router = inject(Router);

  if (!roleService.isAdmin()) {
    router.navigate(['/eventos']);
    return false;
  }
  return true;
};
