export interface OccupancyReport {
  eventId: number;
  eventTitle: string;
  maxCapacity: number;
  ticketsSold: number;
  availableTickets: number;
  occupancyPercentage: number;
  totalRevenue: number;
  eventStatus: string;
}
