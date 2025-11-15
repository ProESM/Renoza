import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { LoginRequest, AuthResponse, User, ResetPasswordRequest } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'current_user';

  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser$: Observable<User | null>;

  private isAuthenticatedSubject: BehaviorSubject<boolean>;
  public isAuthenticated$: Observable<boolean>;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const storedUser = this.getUserFromStorage();
    this.currentUserSubject = new BehaviorSubject<User | null>(storedUser);
    this.currentUser$ = this.currentUserSubject.asObservable();

    this.isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());
    this.isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  }

  /**
   * Вход пользователя
   */
  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          this.setAuthData(response);
        })
      );
  }

  /**
   * Выход пользователя
   */
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/auth/login']);
  }

  /**
   * Запрос на восстановление пароля
   */
  requestPasswordReset(request: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/reset-password`, request);
  }

  /**
   * Получение информации о текущем пользователе
   */
  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/auth/me`)
      .pipe(
        tap(user => {
          this.setUser(user);
        })
      );
  }

  /**
   * Получение токена
   */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Проверка наличия действительного токена
   */
  hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    // Проверка срока действия токена (декодирование JWT)
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationDate = new Date(payload.exp * 1000);
      return expirationDate > new Date();
    } catch (error) {
      return false;
    }
  }

  /**
   * Получение значения текущего пользователя
   */
  get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  /**
   * Сохранение данных авторизации
   */
  private setAuthData(authResponse: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authResponse.token);

    // Получаем полную информацию о пользователе
    this.getCurrentUser().subscribe({
      next: () => {
        this.isAuthenticatedSubject.next(true);
        this.router.navigate(['/home']);
      },
      error: (error) => {
        console.error('Ошибка при получении данных пользователя:', error);
        this.logout();
      }
    });
  }

  /**
   * Сохранение пользователя
   */
  private setUser(user: User): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  /**
   * Получение пользователя из хранилища
   */
  private getUserFromStorage(): User | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (userJson) {
      try {
        return JSON.parse(userJson);
      } catch (error) {
        console.error('Ошибка при разборе данных пользователя:', error);
        return null;
      }
    }
    return null;
  }
}
