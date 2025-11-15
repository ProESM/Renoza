import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  // Получаем токен
  const token = authService.getToken();

  // Если токен существует, добавляем его в заголовок
  const authReq = token
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
    : req;

  // Обрабатываем запрос
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Если получили 401 ошибку, выходим из системы
      if (error.status === 401) {
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};
