import { Injectable, signal } from '@angular/core';

export type Role = 'user' | 'admin' | null;

@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly _role = signal<Role>(null);

  readonly role = this._role.asReadonly();

  setRole(role: Role): void {
    this._role.set(role);
    if (role) sessionStorage.setItem('role', role);
    else sessionStorage.removeItem('role');
  }

  loadFromSession(): void {
    const saved = sessionStorage.getItem('role') as Role;
    if (saved) this._role.set(saved);
  }

  isAdmin(): boolean {
    return this._role() === 'admin';
  }

  isUser(): boolean {
    return this._role() === 'user';
  }

  hasRole(): boolean {
    return this._role() !== null;
  }

  clearRole(): void {
    this._role.set(null);
    sessionStorage.removeItem('role');
  }
}
