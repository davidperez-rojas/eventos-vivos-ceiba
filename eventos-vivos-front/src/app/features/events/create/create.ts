import {Component, inject, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {EventService} from '../../../core/services/events.service';
import {Router, RouterLink} from '@angular/router';
import {VenueService} from '../../../core/services/venue.service';
import {CreateEventRequest} from '../../../core/models/event.model';

@Component({
  selector: 'app-create',
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './create.html',
  styleUrl: './create.scss',
})
export class Create {
  private readonly fb = inject(FormBuilder);
  private readonly eventService = inject(EventService);
  private readonly router = inject(Router);
  readonly venueService = inject(VenueService);

  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
    venueId: ['', Validators.required],
    maxCapacity: [null as number | null, [Validators.required, Validators.min(1)]],
    startDateTime: ['', Validators.required],
    endDateTime: ['', Validators.required],
    ticketPrice: [null as number | null, [Validators.required, Validators.min(0.01)]],
    eventType: ['', Validators.required]
  });

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control?.invalid && control?.touched);
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.loading.set(true);
    this.error.set(null);
    const val = this.form.value;
    const request = {
      title: val.title!,
      description: val.description!,
      venueId: Number(val.venueId),
      maxCapacity: val.maxCapacity!,
      startDateTime: new Date(val.startDateTime!).toISOString(),
      endDateTime: new Date(val.endDateTime!).toISOString(),
      ticketPrice: val.ticketPrice!,
      eventType: val.eventType!
    }

    if(this.validateRequest(request))
      return;

    this.eventService.create(request).subscribe({
      next: (event) => this.router.navigate(['/events', event.id]),
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }

  private validateRequest(request: CreateEventRequest): boolean {
    debugger;
    const now = new Date();
    const startDate = new Date(request.startDateTime);
    const endDate = new Date(request.endDateTime);

    // Validar que la fecha de inicio sea futura
    if (startDate <= now) {
      this.error.set('La fecha de inicio debe ser futura');
      this.loading.set(false);
      return true;
    }

    // Validar que la fecha de fin sea posterior al inicio
    if (endDate <= startDate) {
      this.error.set('La fecha de fin debe ser posterior a la fecha de inicio');
      this.loading.set(false);
      return true;
    }

    // RN-03: Fin de semana no puede iniciar después de las 22:00
    const dayOfWeek = startDate.getDay(); // 0=domingo, 6=sábado
    if ((dayOfWeek === 0 || dayOfWeek === 6) && startDate.getHours() >= 22) {
      this.error.set('Los eventos en fin de semana no pueden iniciar después de las 22:00');
      this.loading.set(false);
      return true;
    }

    // RN-01: Capacidad no puede exceder la del venue
    const venue = this.venueService.venues.find(v => v.id === request.venueId);
    if (venue && request.maxCapacity > venue.capacity) {
      this.error.set(`La capacidad máxima (${request.maxCapacity}) no puede exceder la capacidad del venue "${venue.name}" (${venue.capacity})`);
      this.loading.set(false);
      return true;
    }

    return false;
  }
}
