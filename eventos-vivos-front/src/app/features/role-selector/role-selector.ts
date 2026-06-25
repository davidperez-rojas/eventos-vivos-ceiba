import {Component, inject} from '@angular/core';
import {RoleService} from '../../core/services/role.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-role-selector',
  imports: [],
  templateUrl: './role-selector.html',
  styleUrl: './role-selector.scss',
})
export class RoleSelector {
  private readonly roleService = inject(RoleService);
  private readonly router = inject(Router);

  select(role: 'user' | 'admin'){
    this.roleService.setRole(role);
    this.router.navigate(['/eventos']);
  }
}
