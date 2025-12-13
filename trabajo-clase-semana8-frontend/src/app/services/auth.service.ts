import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../config/api.config';
import { tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {

  constructor(private http: HttpClient) {}

  register(data: any) {
    return this.http.post(`${API_URL}/auth/register`, data);
  }

  login(data: any) {
    return this.http.post<any>(`${API_URL}/auth/login`, data).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  logout() {
    localStorage.removeItem('token');
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getPayload(): any {
    const token = this.getToken();
    if (!token) return null;
    return JSON.parse(atob(token.split('.')[1]));
  }

  getRole(): string | null {
    return this.getPayload()?.role ?? null;
  }

  getEmail(): string | null {
    return this.getPayload()?.email ?? null;
  }
}
