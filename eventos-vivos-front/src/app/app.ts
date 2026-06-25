import {Component, inject} from '@angular/core';
import {Router, RouterLink, RouterOutlet} from '@angular/router';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {faCalendarCheck} from '@fortawesome/free-solid-svg-icons';
import {RoleService} from './core/services/role.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, FontAwesomeModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  faCalendar = faCalendarCheck;

  readonly roleService = inject(RoleService);
  private readonly router = inject(Router);

  logout(): void {
    this.roleService.clearRole();
    this.router.navigate(['/']);
  }
}
