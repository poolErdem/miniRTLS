import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  private telemetrySource = new BehaviorSubject<any>(null);
  telemetry$ = this.telemetrySource.asObservable();

  constructor() {
    this.startConnection();
    this.registerOnServerEvents();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/assethub') // Backend SignalR hub URL'si (kendi backend adresinle değiştir)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch(err => console.error('Error while starting SignalR connection: ' + err));
  }

  private registerOnServerEvents(): void {
    this.hubConnection.on('ReceiveTelemetryUpdate', (data) => {
      console.log('Received telemetry update:', data);
      this.telemetrySource.next(data);
    });
  }
}
