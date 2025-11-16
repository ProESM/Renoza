import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  WorkerProfile,
  CreateWorkerProfileRequest,
  UpdateWorkerProfileRequest
} from '../models/profile.models';

@Injectable({
  providedIn: 'root'
})
export class WorkerProfileService {
  private readonly apiUrl = `${environment.apiUrl}/workerprofiles`;

  constructor(private http: HttpClient) { }

  /**
   * Получить все профили работников
   */
  getProfiles(isAvailable?: boolean): Observable<WorkerProfile[]> {
    let params = new HttpParams();
    if (isAvailable !== undefined) {
      params = params.set('isAvailable', isAvailable.toString());
    }
    return this.http.get<WorkerProfile[]>(this.apiUrl, { params });
  }

  /**
   * Получить профиль по идентификатору
   */
  getProfileById(id: string): Observable<WorkerProfile> {
    return this.http.get<WorkerProfile>(`${this.apiUrl}/${id}`);
  }

  /**
   * Получить профиль по идентификатору пользователя
   */
  getProfileByUserId(userId: string): Observable<WorkerProfile> {
    return this.http.get<WorkerProfile>(`${this.apiUrl}/user/${userId}`);
  }

  /**
   * Создать профиль работника
   */
  createProfile(request: CreateWorkerProfileRequest): Observable<WorkerProfile> {
    return this.http.post<WorkerProfile>(this.apiUrl, request);
  }

  /**
   * Обновить профиль работника
   */
  updateProfile(id: string, request: UpdateWorkerProfileRequest): Observable<WorkerProfile> {
    return this.http.put<WorkerProfile>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Деактивировать профиль работника
   */
  deactivateProfile(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }

  /**
   * Установить доступность работника
   */
  setAvailability(id: string, isAvailable: boolean): Observable<{ message: string; isAvailable: boolean }> {
    return this.http.patch<{ message: string; isAvailable: boolean }>(
      `${this.apiUrl}/${id}/availability`,
      isAvailable
    );
  }
}
