import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reservation, CreateReservationRequest } from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://eventos-vivos-ceiba-production.up.railway.app/api/Reservations';
  //private readonly baseUrl = 'http://localhost:5217/api/Reservations';

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

  getByEvent(eventId: number, email?: string): Observable<Reservation[]> {
    let params = new HttpParams();
    if (email) params = params.set('email', email);
    return this.http.get<Reservation[]>(`${this.baseUrl}/event/${eventId}`, { params });
  }
}
