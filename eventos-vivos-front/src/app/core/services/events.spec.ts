import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { Event } from '../models/event.model';
import {EventService} from './events.service';

describe('EventService', () => {
  let service: EventService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5217/api/events';

  const mockEvent: Event = {
    id: 1,
    title: 'Tech Conference',
    description: 'A great tech event',
    venue: { id: 1, name: 'Auditorio Central', capacity: 200, city: 'Bogotá' },
    maxCapacity: 100,
    availableTickets: 80,
    startDateTime: '2025-12-01T10:00:00Z',
    endDateTime: '2025-12-01T18:00:00Z',
    ticketPrice: 50,
    eventType: 'conferencia',
    status: 'activo'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EventService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(EventService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAll - should GET all events without filters', () => {
    service.getAll().subscribe(events => {
      expect(events.length).toBe(1);
      expect(events[0].title).toBe('Tech Conference');
    });
    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush([mockEvent]);
  });

  it('getAll - should apply filters as query params', () => {
    service.getAll({ type: 'conferencia', status: 'activo' }).subscribe();
    const req = httpMock.expectOne(r =>
      r.url === baseUrl &&
      r.params.get('type') === 'conferencia' &&
      r.params.get('status') === 'activo'
    );
    expect(req.request.method).toBe('GET');
    req.flush([mockEvent]);
  });

  it('getById - should GET event by id', () => {
    service.getById(1).subscribe(event => {
      expect(event.id).toBe(1);
      expect(event.title).toBe('Tech Conference');
    });
    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockEvent);
  });

  it('create - should POST new event', () => {
    const request = {
      title: 'Tech Conference',
      description: 'A great tech event',
      venueId: 1,
      maxCapacity: 100,
      startDateTime: '2025-12-01T10:00:00Z',
      endDateTime: '2025-12-01T18:00:00Z',
      ticketPrice: 50,
      eventType: 'conferencia'
    };
    service.create(request).subscribe(event => {
      expect(event.id).toBe(1);
    });
    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush(mockEvent);
  });

  it('getReport - should GET occupancy report', () => {
    const mockReport = {
      eventId: 1, eventTitle: 'Tech Conference',
      maxCapacity: 100, ticketsSold: 20,
      availableTickets: 80, occupancyPercentage: 20,
      totalRevenue: 1000, eventStatus: 'activo'
    };
    service.getReport(1).subscribe(report => {
      expect(report.ticketsSold).toBe(20);
      expect(report.occupancyPercentage).toBe(20);
    });
    const req = httpMock.expectOne(`${baseUrl}/1/report`);
    expect(req.request.method).toBe('GET');
    req.flush(mockReport);
  });
});
