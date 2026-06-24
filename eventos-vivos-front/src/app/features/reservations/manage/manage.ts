import {Component, inject, signal} from '@angular/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {ReservationService} from '../../../core/services/reservation.service';
import {Reservation} from '../../../core/models/reservation.model';
import {BadgeStatus} from '../../../shared/components/badge-status/badge-status';
import {DatePipe} from '@angular/common';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {
  faCalendarCheck,
  faEnvelope,
  faTicketSimple,
  faTriangleExclamation,
  faUser
} from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-manage',
  imports: [
    RouterLink,
    BadgeStatus,
    DatePipe,
    FontAwesomeModule
  ],
  templateUrl: './manage.html',
  styleUrl: './manage.scss',
})
export class Manage {
  faUser = faUser;
  faEmail = faEnvelope;
  faTicket = faTicketSimple;
  faCalendar = faCalendarCheck;
  faWarning = faTriangleExclamation;

  private readonly route = inject(ActivatedRoute);
  private readonly reservationService = inject(ReservationService);

  reservation = signal<Reservation | null>(null);
  loading = signal(false);
  actionLoading = signal(false);
  error = signal<string | null>(null);
  actionError = signal<string | null>(null);
  actionSuccess = signal<string | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadReservation(id);
  }

  loadReservation(id: number): void {
    this.loading.set(true);
    this.reservationService.getById(id).subscribe({
      next: (data) => { this.reservation.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }

  confirmPayment(): void {
    this.actionLoading.set(true);
    this.actionError.set(null);
    this.actionSuccess.set(null);
    this.reservationService.confirmPayment(this.reservation()!.id).subscribe({
      next: (data) => {
        this.reservation.set(data);
        this.actionSuccess.set(`¡Pago confirmado! Código de reserva: ${data.reservationCode}`);
        this.actionLoading.set(false);
      },
      error: (err) => { this.actionError.set(err.message); this.actionLoading.set(false); }
    });
  }

  cancelReservation(): void {
    if (!confirm('¿Estás seguro de que deseas cancelar esta reserva?')) return;
    this.actionLoading.set(true);
    this.actionError.set(null);
    this.actionSuccess.set(null);
    this.reservationService.cancel(this.reservation()!.id).subscribe({
      next: (data) => {
        this.reservation.set(data);
        const msg = data.ticketsLost
          ? 'Reserva cancelada. Las entradas se marcaron como perdidas (cancelación con menos de 48h del evento).'
          : 'Reserva cancelada exitosamente.';
        this.actionSuccess.set(msg);
        this.actionLoading.set(false);
      },
      error: (err) => { this.actionError.set(err.message); this.actionLoading.set(false); }
    });
  }
}
