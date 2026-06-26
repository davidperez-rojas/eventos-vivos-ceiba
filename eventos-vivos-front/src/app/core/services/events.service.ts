import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event, CreateEventRequest, EventFilter } from '../models/event.model';
import { OccupancyReport } from '../models/report.model';
import {Reservation} from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class EventService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://eventos-vivos-ceiba-production.up.railway.app/api/Events';
  //private readonly baseUrl = 'http://localhost:5217/api/Events';

  getAll(filters?: EventFilter): Observable<Event[]> {
    let params = new HttpParams();
    if (filters?.type) params = params.set('type', filters.type);
    if (filters?.dateFrom) params = params.set('dateFrom', filters.dateFrom);
    if (filters?.dateTo) params = params.set('dateTo', filters.dateTo);
    if (filters?.venueId) params = params.set('venueId', filters.venueId.toString());
    if (filters?.status) params = params.set('status', filters.status);
    if (filters?.title) params = params.set('title', filters.title);
    return this.http.get<Event[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Event> {
    return this.http.get<Event>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateEventRequest): Observable<Event> {
    return this.http.post<Event>(this.baseUrl, request);
  }

  getReport(id: number): Observable<OccupancyReport> {
    return this.http.get<OccupancyReport>(`${this.baseUrl}/${id}/report`);
  }

  cancelEvent(eventId: number): Observable<Event>{
    return this.http.put<Event>(`${this.baseUrl}/${eventId}`, {});
  }
}
