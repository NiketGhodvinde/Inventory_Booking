import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class BookingService {

  private apiUrl = environment.API_BASE_URL + 'Booking';

  constructor(private http: HttpClient) { }

  // Method to upload CSV file
  uploadBookings(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.apiUrl}/upload-bookings`, formData, {
      headers: new HttpHeaders(),
    }).pipe(
      catchError((error) => {
        console.error('Error uploading file:', error);
        throw error; // Re-throw the error for the component to handle
      })
    );
  }

  // Method to fetch all bookings
  getBookings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/bookings`).pipe(
      catchError((error) => {
        console.error('Error fetching bookings:', error);
        throw error; // Re-throw the error for the component to handle
      })
    );
  }
}
