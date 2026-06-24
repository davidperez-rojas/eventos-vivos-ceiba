import { Injectable } from '@angular/core';
import { Venue } from '../models/venue.model';

@Injectable({ providedIn: 'root' })
export class VenueService {
  readonly venues: Venue[] = [
    { id: 1, name: 'Auditorio Central', capacity: 200, city: 'Bogotá' },
    { id: 2, name: 'Sala Norte', capacity: 50, city: 'Bogotá' },
    { id: 3, name: 'Arena Sur', capacity: 500, city: 'Medellín' }
  ];
}
