import {Component, inject, signal} from '@angular/core';
import {RouterLink} from '@angular/router';
import {BadgeStatus} from '../../../shared/components/badge-status/badge-status';
import {CurrencyPipe, DatePipe} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {EventService} from '../../../core/services/events.service';
import {VenueService} from '../../../core/services/venue.service';
import {Event, EventFilter} from '../../../core/models/event.model';

@Component({
  selector: 'app-list',
  imports: [
    RouterLink,
    BadgeStatus,
    DatePipe,
    CurrencyPipe,
    FormsModule
  ],
  templateUrl: './list.html',
  styleUrl: './list.scss',
})
export class List {
  private readonly eventService = inject(EventService);
  readonly venueService = inject(VenueService);

  events = signal<Event[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  filters: EventFilter = {};

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.loading.set(true);
    this.error.set(null);
    this.eventService.getAll(this.filters).subscribe({
      next: (data) => { this.events.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err.message); this.loading.set(false); }
    });
  }

  onFilterChange(): void { this.loadEvents(); }

  clearFilters(): void { this.filters = {}; this.loadEvents(); }

  typeLabel(type: string): string {
    const labels: Record<string, string> = {
      conferencia: 'Conferencia', taller: 'Taller', concierto: 'Concierto'
    };
    return labels[type] ?? type;
  }

  typeIcon(type: string): string {
    const icons: Record<string, string> = {
      conferencia: 'fa-solid fa-microphone',
      taller: 'fa-solid fa-screwdriver-wrench',
      concierto: 'fa-solid fa-music'
    };
    return icons[type] ?? 'fa-solid fa-calendar';
  }
}
