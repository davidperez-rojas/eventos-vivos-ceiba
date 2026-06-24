import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reservation, CreateReservationRequest } from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://eventos-vivos-ceiba-production.up.railway.app/api/Reservations';

  create(request: CreateReservationRequest): Observable<Reservation> {
    return this.http.post<Reservation>(this.baseUrl, request);
  }

  getById(id: number): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.baseUrl}/${id}`);
  }

  confirmPayment(id: number): Observable<Reservation> {
    return this.http.put<Reservation>(`${this.baseUrl}/${id}/confirm`, {});
  }

  cancel(id: number): Observable<Reservation> {
    return this.http.put<Reservation>(`${this.baseUrl}/${id}/cancel`, {});
  }

  getByEvent(eventId: number): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.baseUrl}/event/${eventId}`);
  }
}
