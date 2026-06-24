import {Component, inject, signal} from '@angular/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {EventService} from '../../../core/services/events.service';
import {Event} from '../../../core/models/event.model';
import {CurrencyPipe, DatePipe} from '@angular/common';
import {BadgeStatus} from '../../../shared/components/badge-status/badge-status';
import {OccupancyReport} from '../../../core/models/report.model';
import {
  faArrowsRotate,
  faCalendarCheck, faChartColumn, faCheck, faHourglass,
  faLocationDot,
  faMasksTheater,
  faMoneyBill, faTicketAlt,
  faTicketSimple, faTriangleExclamation, faX
} from '@fortawesome/free-solid-svg-icons';
import {FaIconComponent} from '@fortawesome/angular-fontawesome';
import {ReservationService} from '../../../core/services/reservation.service';
import {Reservation} from '../../../core/models/reservation.model';

@Component({
  selector: 'app-detail',
  imports: [
    CurrencyPipe,
    RouterLink,
    DatePipe,
    BadgeStatus,
    FaIconComponent
  ],
  templateUrl: './detail.html',
  styleUrl: './detail.scss',
})
export class Detail {
  faLocation= faLocationDot
  faMaskTheater = faMasksTheater;
  faCalendar = faCalendarCheck;
  faCoins = faMoneyBill;
  faTicket = faTicketSimple;
  faTicketReserve = faTicketAlt;
  faChart = faChartColumn;
  faUpdate = faArrowsRotate;
  faPending = faHourglass;
  faConfimed = faCheck;
  faCancel = faX;
  faWarning = faTriangleExclamation;

  private readonly route = inject(ActivatedRoute);
  private readonly eventService = inject(EventService);
  private readonly reservationService = inject(ReservationService);

  event = signal<Event | null>(null);
  report = signal<OccupancyReport | null>(null);
  reservations = signal<Reservation[]>([]);
  loading = signal(false);
  loadingReservations = signal(false);
  error = signal<string | null>(null);

  private eventId!: number;

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadEvent();
    this.loadReport();
    this.loadReservations();
  }

  loadEvent(): void {
    this.loading.set(true);
    this.eventService.getById(this.eventId).subscribe({
      next: (data) => { this.event.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }

  loadReport(): void {
    this.eventService.getReport(this.eventId).subscribe({
      next: (data) => this.report.set(data),
      error: () => {}
    });
  }

  loadReservations(): void {
    this.loadingReservations.set(true);
    this.reservationService.getByEvent(this.eventId).subscribe({
      next: (data) => { this.reservations.set(data); this.loadingReservations.set(false); },
      error: () => this.loadingReservations.set(false)
    });
  }

  countByStatus(status: string): number {
    return this.reservations().filter(r => r.status === status).length;
  }

  typeLabel(type: string): string {
    const labels: Record<string, string> = {
      conferencia: 'Conferencia', taller: 'Taller', concierto: 'Concierto'
    };
    return labels[type] ?? type;
  }
}
