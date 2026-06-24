import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { VenueService } from './venue.service';

describe('VenueService', () => {
  let service: VenueService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [VenueService] });
    service = TestBed.inject(VenueService);
  });

  it('should be created', () => { expect(service).toBeTruthy(); });

  it('should have exactly 3 venues', () => {
    expect(service.venues.length).toBe(3);
  });

  it('should contain Auditorio Central with capacity 200', () => {
    const venue = service.venues.find(v => v.id === 1);
    expect(venue).toBeDefined();
    expect(venue!.name).toBe('Auditorio Central');
    expect(venue!.capacity).toBe(200);
    expect(venue!.city).toBe('Bogotá');
  });

  it('should contain Sala Norte with capacity 50', () => {
    const venue = service.venues.find(v => v.id === 2);
    expect(venue).toBeDefined();
    expect(venue!.name).toBe('Sala Norte');
    expect(venue!.capacity).toBe(50);
  });

  it('should contain Arena Sur with capacity 500', () => {
    const venue = service.venues.find(v => v.id === 3);
    expect(venue).toBeDefined();
    expect(venue!.name).toBe('Arena Sur');
    expect(venue!.capacity).toBe(500);
    expect(venue!.city).toBe('Medellín');
  });
});
