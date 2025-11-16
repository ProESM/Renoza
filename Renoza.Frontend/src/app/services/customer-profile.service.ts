import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CustomerProfile,
  CreateCustomerProfileRequest,
  UpdateCustomerProfileRequest
} from '../models/profile.models';

@Injectable({
  providedIn: 'root'
})
export class CustomerProfileService {
  private readonly apiUrl = `${environment.apiUrl}/customerprofiles`;

  constructor(private http: HttpClient) { }

  /**
   * Получить все профили заказчиков
   */
  getProfiles(): Observable<CustomerProfile[]> {
    return this.http.get<CustomerProfile[]>(this.apiUrl);
  }

  /**
   * Получить профиль по идентификатору
   */
  getProfileById(id: string): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/${id}`);
  }

  /**
   * Получить профиль по идентификатору пользователя
   */
  getProfileByUserId(userId: string): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/user/${userId}`);
  }

  /**
   * Создать профиль заказчика
   */
  createProfile(request: CreateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.post<CustomerProfile>(this.apiUrl, request);
  }

  /**
   * Обновить профиль заказчика
   */
  updateProfile(id: string, request: UpdateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Деактивировать профиль заказчика
   */
  deactivateProfile(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
