import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { VenueService } from '../../../core/services/venue.service';
import { of } from 'rxjs';
import {EventService} from '../../../core/services/events.service';
import {Create} from '../../events/create/create';

describe('Create', () => {
  let fixture: ComponentFixture<Create>;
  let component: Create;
  let eventServiceMock: Partial<EventService>;
  let router: Router;

  // Fecha futura válida (lunes a las 10:00)
  const getFutureMonday = () => {
    const date = new Date();
    date.setDate(date.getDate() + 7);
    // Asegurarse de que sea lunes
    while (date.getDay() !== 1) date.setDate(date.getDate() + 1);
    date.setHours(10, 0, 0, 0);
    return date;
  };

  const getValidFormValue = () => {
    const start = getFutureMonday();
    const end = new Date(start);
    end.setHours(start.getHours() + 3);
    return {
      title: 'Conferencia de Tecnología',
      description: 'Una conferencia increíble sobre el futuro del software',
      venueId: '1',
      maxCapacity: 100,
      startDateTime: start.toISOString().slice(0, 16),
      endDateTime: end.toISOString().slice(0, 16),
      ticketPrice: 50,
      eventType: 'conferencia'
    };
  };

  beforeEach(async () => {
    eventServiceMock = {
      create: vi.fn().mockReturnValue(of({ id: 1, title: 'Test' }))
    };

    await TestBed.configureTestingModule({
      imports: [Create],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: EventService, useValue: eventServiceMock },
        VenueService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Create);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should show error if startDateTime is in the past', () => {
    const pastDate = new Date();
    pastDate.setDate(pastDate.getDate() - 1);
    const endDate = new Date(pastDate);
    endDate.setHours(pastDate.getHours() + 2);

    component.form.setValue({
      ...getValidFormValue(),
      startDateTime: pastDate.toISOString().slice(0, 16),
      endDateTime: endDate.toISOString().slice(0, 16)
    });

    component.onSubmit();

    expect(component.error()).toBe('La fecha de inicio debe ser futura');
    expect(eventServiceMock.create).not.toHaveBeenCalled();
  });

  it('should show error if endDateTime is before startDateTime', () => {
    const start = getFutureMonday();
    const end = new Date(start);
    end.setHours(start.getHours() - 1); // fin antes que inicio

    component.form.setValue({
      ...getValidFormValue(),
      startDateTime: start.toISOString().slice(0, 16),
      endDateTime: end.toISOString().slice(0, 16)
    });

    component.onSubmit();

    expect(component.error()).toBe('La fecha de fin debe ser posterior a la fecha de inicio');
    expect(eventServiceMock.create).not.toHaveBeenCalled();
  });

  it('should allow weekend event that starts before 22:00 (RN-03)', () => {
    // Buscar próximo sábado a las 20:00 — debe pasar
    const saturday = new Date();
    saturday.setDate(saturday.getDate() + 1);
    while (saturday.getDay() !== 6) saturday.setDate(saturday.getDate() + 1);
    saturday.setHours(20, 0, 0, 0);

    const end = new Date(saturday);
    end.setHours(22, 0, 0, 0);

    component.form.setValue({
      ...getValidFormValue(),
      startDateTime: saturday.toISOString().slice(0, 16),
      endDateTime: end.toISOString().slice(0, 16)
    });

    component.onSubmit();

    expect(eventServiceMock.create).toHaveBeenCalled();
  });

  it('should show error if maxCapacity exceeds venue capacity (RN-01)', () => {
    // Venue 2 tiene capacidad 50
    component.form.setValue({
      ...getValidFormValue(),
      venueId: '2',
      maxCapacity: 100 // excede capacidad de Sala Norte (50)
    });

    component.onSubmit();

    expect(component.error()).toContain('no puede exceder la capacidad del venue');
    expect(component.error()).toContain('Sala Norte');
    expect(eventServiceMock.create).not.toHaveBeenCalled();
  });

  it('should allow maxCapacity equal to venue capacity (RN-01)', () => {
    // Venue 2 tiene capacidad 50, maxCapacity = 50 debe pasar
    component.form.setValue({
      ...getValidFormValue(),
      venueId: '2',
      maxCapacity: 50
    });

    component.onSubmit();

    expect(eventServiceMock.create).toHaveBeenCalled();
  });
});
