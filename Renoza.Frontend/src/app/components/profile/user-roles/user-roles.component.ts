import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RoleService } from '../../../services/role.service';
import { AuthService } from '../../../services/auth.service';
import { Role, RoleId } from '../../../models/role.models';

@Component({
  selector: 'app-user-roles',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatChipsModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './user-roles.component.html',
  styleUrl: './user-roles.component.scss'
})
export class UserRolesComponent implements OnInit {
  allRoles: Role[] = [];
  userRoles: Role[] = [];
  loading = false;

  // Карта идентификаторов ролей для удобного доступа
  readonly RoleId = RoleId;

  constructor(
    private roleService: RoleService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadRoles();
    this.loadUserRoles();
  }

  loadRoles(): void {
    this.loading = true;
    this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.allRoles = roles;
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Ошибка загрузки ролей', 'Закрыть', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  loadUserRoles(): void {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) {
      return;
    }

    this.roleService.getUserRoles(currentUser.id).subscribe({
      next: (roles) => {
        this.userRoles = roles;
      },
      error: () => {
        this.snackBar.open('Ошибка загрузки ролей пользователя', 'Закрыть', { duration: 3000 });
      }
    });
  }

  hasRole(roleId: string): boolean {
    return this.userRoles.some(role => role.id === roleId);
  }

  toggleRole(role: Role): void {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) {
      this.snackBar.open('Пользователь не авторизован', 'Закрыть', { duration: 3000 });
      return;
    }

    const hasRole = this.hasRole(role.id);

    if (hasRole) {
      // Отзываем роль
      this.roleService.revokeRoleFromUser(currentUser.id, role.id).subscribe({
        next: () => {
          this.userRoles = this.userRoles.filter(r => r.id !== role.id);
          this.snackBar.open(`Роль "${role.name}" отозвана`, 'Закрыть', { duration: 3000 });
        },
        error: () => {
          this.snackBar.open('Ошибка при отзыве роли', 'Закрыть', { duration: 3000 });
        }
      });
    } else {
      // Назначаем роль
      this.roleService.assignRoleToUser(currentUser.id, role.id).subscribe({
        next: () => {
          this.userRoles.push(role);
          this.snackBar.open(`Роль "${role.name}" назначена`, 'Закрыть', { duration: 3000 });
        },
        error: () => {
          this.snackBar.open('Ошибка при назначении роли', 'Закрыть', { duration: 3000 });
        }
      });
    }
  }

  getRoleColor(roleId: string): string {
    switch (roleId) {
      case RoleId.Administrator:
        return 'warn';
      case RoleId.Customer:
        return 'primary';
      case RoleId.Worker:
        return 'accent';
      case RoleId.TechnicalSupervisor:
        return 'primary';
      default:
        return '';
    }
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
