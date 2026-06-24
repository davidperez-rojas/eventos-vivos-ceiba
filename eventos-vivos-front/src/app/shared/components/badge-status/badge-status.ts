import {Component, Input} from '@angular/core';
import {TitleCasePipe} from '@angular/common';

@Component({
  selector: 'app-badge-status',
  templateUrl: './badge-status.html',
  styleUrl: './badge-status.scss',
})
export class BadgeStatus {
  @Input() status: string = '';

  statusLabel(): string {
    const labels: Record<string, string> = {
      activo: 'Activo',
      cancelado: 'Cancelado',
      completado: 'Completado',
      confirmada: 'Confirmada',
      pendiente_pago: 'Pendiente de pago'
    };
    return labels[this.status] ?? this.status;
  }
}
