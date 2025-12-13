import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../config/api.config';

@Injectable({ providedIn: 'root' })
export class UsersService {

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<any[]>(`${API_URL}/users`);
  }
}
