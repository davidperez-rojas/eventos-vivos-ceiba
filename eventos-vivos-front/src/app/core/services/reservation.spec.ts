import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ReservationService } from './reservation.service';
import { Reservation } from '../models/reservation.model';

describe('ReservationService', () => {
  let service: ReservationService;
  let httpMock: HttpTestingController;

  const baseUrl = 'https://eventos-vivos-ceiba-production.up.railway.app/api/Reservations';

  const mockReservation: Reservation = {
    id: 1, eventId: 1, eventTitle: 'Tech Conference',
    quantity: 2, buyerName: 'David Pérez',
    buyerEmail: 'david@test.com', status: 'pendiente_pago',
    createdAt: '2025-11-01T10:00:00Z', ticketsLost: false
  };

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [ReservationService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ReservationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('create - should POST new reservation', () => {
    const request = {
      eventId: 1, quantity: 2,
      buyerName: 'David Pérez', buyerEmail: 'david@test.com'
    };
    service.create(request).subscribe(r => {
      expect(r.status).toBe('pendiente_pago');
      expect(r.quantity).toBe(2);
    });
    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    req.flush(mockReservation);
  });

  it('getById - should GET reservation by id', () => {
    service.getById(1).subscribe(r => {
      expect(r.id).toBe(1);
      expect(r.buyerName).toBe('David Pérez');
    });
    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockReservation);
  });

  it('confirmPayment - should PUT to confirm payment', () => {
    const confirmed = { ...mockReservation, status: 'confirmada', reservationCode: 'EV-123456' };
    service.confirmPayment(1).subscribe(r => {
      expect(r.status).toBe('confirmada');
      expect(r.reservationCode).toBe('EV-123456');
    });
    const req = httpMock.expectOne(`${baseUrl}/1/confirm`);
    expect(req.request.method).toBe('PUT');
    req.flush(confirmed);
  });

  it('cancel - should PUT to cancel reservation', () => {
    const cancelled = { ...mockReservation, status: 'cancelada', cancelledAt: '2025-11-10T10:00:00Z' };
    service.cancel(1).subscribe(r => {
      expect(r.status).toBe('cancelada');
      expect(r.cancelledAt).toBeTruthy();
    });
    const req = httpMock.expectOne(`${baseUrl}/1/cancel`);
    expect(req.request.method).toBe('PUT');
    req.flush(cancelled);
  });

  it('cancel - should mark tickets as lost when cancelled within 48h', () => {
    const lost = { ...mockReservation, status: 'cancelada', ticketsLost: true };
    service.cancel(1).subscribe(r => {
      expect(r.ticketsLost).toBe(true);
    });
    const req = httpMock.expectOne(`${baseUrl}/1/cancel`);
    req.flush(lost);
  });
});
