import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/auth.models';
import { Role, RoleId } from '../../models/role.models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatCardModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  currentUser: User | null = null;
  userRoles: Role[] = [];

  readonly RoleId = RoleId;

  constructor(
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Подписываемся на изменения текущего пользователя
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    // Подписываемся на изменения ролей
    this.authService.userRoles$.subscribe(roles => {
      this.userRoles = roles;
    });

    // Загружаем роли пользователя
    this.authService.loadUserRoles().subscribe();
  }

  logout(): void {
    this.authService.logout();
  }

  hasWorkerRole(): boolean {
    return this.authService.hasRole(RoleId.Worker);
  }

  hasCustomerRole(): boolean {
    return this.authService.hasRole(RoleId.Customer);
  }

  getRoleIcon(roleId: string): string {
    switch (roleId) {
      case RoleId.Administrator:
        return 'admin_panel_settings';
      case RoleId.Customer:
        return 'business';
      case RoleId.Worker:
        return 'engineering';
      case RoleId.TechnicalSupervisor:
        return 'supervisor_account';
      default:
        return 'person';
    }
  }
}
