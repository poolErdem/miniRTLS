import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalrService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  template: `
    <h1>MiniRTLS Telemetry</h1>
    <div *ngIf="telemetry">
      <p>Asset ID: {{ telemetry.assetId }}</p>
      <p>X: {{ telemetry.x }}</p>
      <p>Y: {{ telemetry.y }}</p>
      <p>Temperature: {{ telemetry.temperature }}</p>
      <p>Timestamp: {{ telemetry.timestamp }}</p>
    </div>
  `
})
export class AppComponent implements OnInit {
  telemetry: any;

  constructor(private signalrService: SignalrService) {}

  ngOnInit() {
    this.signalrService.telemetry$.subscribe(data => {
      if (data) {
        this.telemetry = data;
      }
    });
  }
}
