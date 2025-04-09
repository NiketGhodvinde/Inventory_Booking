import { Component } from '@angular/core';
import { BookingService } from './bookingService';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'InventoryBookingApp';
  selectedFile: File | null = null;
  uploadStatus: string | null = null;
  bookings: any[] = [];

  constructor(private bookingService: BookingService) {
    this.bookings = [];
  }

  ngOnInit(): void {
    // Fetch the bookings when the component is initialized
    this.fetchBookings();
  }

  // Event handler for file selection
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input && input.files) {
      this.selectedFile = input.files[0];
    }
  }

  // Upload file to the backend
  onUpload() {
    if (!this.selectedFile) return;

    this.bookingService.uploadBookings(this.selectedFile).subscribe({
      next: () => {
        this.uploadStatus = 'Upload successful!';
        this.fetchBookings(); // Optionally refresh bookings after upload
      },
      error: () => {
        this.uploadStatus = 'Upload failed. Please try again.';
      },
    });
  }

  // Fetch bookings from the backend
  fetchBookings() {
    this.bookingService.getBookings().subscribe({
      next: (data: any[]) => {
        debugger;
        this.bookings = data; // Assign the fetched data to the bookings array
      },
      error: (error: any) => {
        console.error('Error fetching bookings:', error);
      },
    });
  }
}
