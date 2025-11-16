import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  // Перенаправление по умолчанию
  { path: '', redirectTo: '/home', pathMatch: 'full' },

  // Публичные маршруты (авторизация)
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'reset-password',
        loadComponent: () => import('./components/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
      }
    ]
  },

  // Защищенные маршруты
  {
    path: 'home',
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard]
  },

  // Маршруты профилей
  {
    path: 'profile',
    canActivate: [authGuard],
    children: [
      {
        path: 'roles',
        loadComponent: () => import('./components/profile/user-roles/user-roles.component').then(m => m.UserRolesComponent)
      },
      {
        path: 'worker',
        loadComponent: () => import('./components/profile/worker-profile/worker-profile.component').then(m => m.WorkerProfileComponent)
      },
      {
        path: 'customer',
        loadComponent: () => import('./components/profile/customer-profile/customer-profile.component').then(m => m.CustomerProfileComponent)
      }
    ]
  },

  // Перенаправление на 404 или home
  { path: '**', redirectTo: '/home' }
];
