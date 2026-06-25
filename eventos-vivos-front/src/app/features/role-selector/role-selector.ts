import {Component, inject, OnInit} from '@angular/core';
import {RoleService} from '../../core/services/role.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-role-selector',
  imports: [],
  templateUrl: './role-selector.html',
  styleUrl: './role-selector.scss',
})
export class RoleSelector implements OnInit {
  private readonly roleService = inject(RoleService);
  private readonly router = inject(Router);

  select(role: 'user' | 'admin'){
    this.roleService.setRole(role);
    this.router.navigate(['/eventos']);
  }

  ngOnInit() {
    this.roleService.loadFromSession();

    if(this.roleService.hasRole()){
      this.router.navigate(['/eventos']);
    }
  }
}
