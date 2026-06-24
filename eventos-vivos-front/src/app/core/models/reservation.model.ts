export interface Reservation {
  id: number;
  eventId: number;
  eventTitle: string;
  quantity: number;
  buyerName: string;
  buyerEmail: string;
  status: string;
  reservationCode?: string;
  createdAt: string;
  cancelledAt?: string;
  ticketsLost: boolean;
}

export interface CreateReservationRequest {
  eventId: number;
  quantity: number;
  buyerName: string;
  buyerEmail: string;
}
