import {Component, inject, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {ReservationService} from '../../../core/services/reservation.service';
import {EventService} from '../../../core/services/events.service';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {Event} from '../../../core/models/event.model';
import {CurrencyPipe, DatePipe} from '@angular/common';
import {faCalendar, faMapPin, faMoneyBill} from '@fortawesome/free-solid-svg-icons';
import {FaIconComponent} from '@fortawesome/angular-fontawesome';

@Component({
  selector: 'app-create',
  imports: [
    RouterLink,
    DatePipe,
    CurrencyPipe,
    ReactiveFormsModule,
    FaIconComponent
  ],
  templateUrl: './create.html',
  styleUrl: './create.scss',
})
export class Create {
  faCalendarr = faCalendar;
  faMap = faMapPin;
  faMoney = faMoneyBill;
  private readonly emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

  private readonly fb = inject(FormBuilder);
  private readonly reservationService = inject(ReservationService);
  private readonly eventService = inject(EventService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  eventId!: number;
  event = signal<Event | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    buyerName: ['', Validators.required],
    buyerEmail: ['', [Validators.required, Validators.email, Validators.pattern(this.emailRegex)]],
    quantity: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.eventService.getById(this.eventId).subscribe({
      next: (e) => this.event.set(e),
      error: (err) => this.error.set(err.message)
    });
  }

  totalCost(): number {
    const quantity = this.form.get('quantity')?.value ?? 0;
    return quantity * (this.event()?.ticketPrice ?? 0);
  }

  isInvalid(field: string): boolean {
    const c = this.form.get(field);
    return !!(c?.invalid && c?.touched);
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set(null);
    const val = this.form.value;
    this.reservationService.create({
      eventId: this.eventId,
      quantity: val.quantity!,
      buyerName: val.buyerName!,
      buyerEmail: val.buyerEmail!
    }).subscribe({
      next: (r) => this.router.navigate(['/reservas', r.id]),
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }
}
