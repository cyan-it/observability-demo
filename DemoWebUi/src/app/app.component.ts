import {Component, OnDestroy} from '@angular/core';
import {HttpClientModule} from '@angular/common/http';
import {ApiService} from './api.service';
import {RouterOutlet} from "@angular/router";
import {NgClass, NgForOf} from "@angular/common";
import {Subscription} from "rxjs";
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {MatExpansionModule} from '@angular/material/expansion';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HttpClientModule, RouterOutlet, NgClass, NgForOf, MatCardModule, MatButtonModule, MatExpansionModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnDestroy {
  declare window: any;
  title = 'Observability Demo';
  responseMessage = '';
  responseColor = 'green';
  baseUrl = this.getBaseUrl();
  private subscriptions: Subscription[] = [];

  endpoints = [
    {label: 'Call /ok', url: '/ok', status: null},
    {label: 'Call /unauthorized', url: '/unauthorized', status: null},
    {label: 'Call /not-found', url: '/not-found', status: null},
    {label: 'Call /bad-request', url: '/bad-request', status: null},
    {label: 'Call /internal-server-error', url: '/internal-server-error', status: null},
  ];

  constructor(private apiService: ApiService) {
  }

  private getBaseUrl(): string {
    return (window as any).__env?.baseUrl || 'https://obs.testingstuff.site/api';
  }

  callService(endpoint: any) {
    const url = this.baseUrl + endpoint.url;
    const subscription = this.apiService.callEndpoint(url).subscribe({
      next: (data) => {
        this.responseMessage = data;
        this.responseColor = 'green';
        endpoint.status = 'ok'; // Update status only on new response
      },
      error: (error) => {
        this.responseMessage = `Error: Status Code ${error.status || 'Server error'}`;
        this.responseColor = 'red';
        endpoint.status = 'error'; // Update status only on new response
      }
    });

    this.subscriptions.push(subscription);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

}
