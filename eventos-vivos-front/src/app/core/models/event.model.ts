import { Venue } from './venue.model';

export interface Event {
  id: number;
  title: string;
  description: string;
  venue: Venue;
  maxCapacity: number;
  availableTickets: number;
  startDateTime: string;
  endDateTime: string;
  ticketPrice: number;
  eventType: string;
  status: string;
}

export interface CreateEventRequest {
  title: string;
  description: string;
  venueId: number;
  maxCapacity: number;
  startDateTime: string;
  endDateTime: string;
  ticketPrice: number;
  eventType: string;
}

export interface EventFilter {
  type?: string;
  dateFrom?: string;
  dateTo?: string;
  venueId?: number;
  status?: string;
  title?: string;
}
