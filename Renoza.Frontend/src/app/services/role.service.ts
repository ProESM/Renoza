import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Role } from '../models/role.models';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly apiUrl = `${environment.apiUrl}/roles`;

  constructor(private http: HttpClient) { }

  /**
   * Получить все роли
   */
  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl);
  }

  /**
   * Получить роль по идентификатору
   */
  getRoleById(id: string): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  /**
   * Получить роли пользователя
   */
  getUserRoles(userId: string): Observable<Role[]> {
    return this.http.get<Role[]>(`${this.apiUrl}/user/${userId}`);
  }

  /**
   * Назначить роль пользователю
   */
  assignRoleToUser(userId: string, roleId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/user/${userId}/role/${roleId}`, {});
  }

  /**
   * Отозвать роль у пользователя
   */
  revokeRoleFromUser(userId: string, roleId: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/user/${userId}/role/${roleId}`);
  }
}
