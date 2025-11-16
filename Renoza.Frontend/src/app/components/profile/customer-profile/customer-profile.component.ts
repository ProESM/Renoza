import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CustomerProfileService } from '../../../services/customer-profile.service';
import { AuthService } from '../../../services/auth.service';
import { CustomerProfile } from '../../../models/profile.models';

@Component({
  selector: 'app-customer-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './customer-profile.component.html',
  styleUrl: './customer-profile.component.scss'
})
export class CustomerProfileComponent implements OnInit {
  profileForm: FormGroup;
  profile: CustomerProfile | null = null;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private customerProfileService: CustomerProfileService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.profileForm = this.fb.group({
      companyName: ['', [Validators.maxLength(256)]],
      taxId: ['', [Validators.maxLength(50)]],
      billingAddress: ['', [Validators.maxLength(500)]],
      creditLimit: [null, [Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    const currentUser = this.authService.currentUserValue;
    if (!currentUser) {
      this.snackBar.open('Пользователь не авторизован', 'Закрыть', { duration: 3000 });
      return;
    }

    this.loading = true;
    this.customerProfileService.getProfileByUserId(currentUser.id).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.profileForm.patchValue({
          companyName: profile.companyName,
          taxId: profile.taxId,
          billingAddress: profile.billingAddress,
          creditLimit: profile.creditLimit
        });
        this.loading = false;
      },
      error: (error) => {
        if (error.status === 404) {
          // Профиль не найден - это нормально для нового пользователя
          this.loading = false;
        } else {
          this.snackBar.open('Ошибка загрузки профиля', 'Закрыть', { duration: 3000 });
          this.loading = false;
        }
      }
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      return;
    }

    const currentUser = this.authService.currentUserValue;
    if (!currentUser) {
      this.snackBar.open('Пользователь не авторизован', 'Закрыть', { duration: 3000 });
      return;
    }

    this.loading = true;
    const formValue = this.profileForm.value;
    const profileData = {
      companyName: formValue.companyName,
      taxId: formValue.taxId,
      billingAddress: formValue.billingAddress,
      creditLimit: formValue.creditLimit
    };

    if (this.profile) {
      // Обновление существующего профиля
      this.customerProfileService.updateProfile(this.profile.id, {
        id: this.profile.id,
        ...profileData
      }).subscribe({
        next: (profile) => {
          this.profile = profile;
          this.snackBar.open('Профиль успешно обновлен', 'Закрыть', { duration: 3000 });
          this.loading = false;
        },
        error: () => {
          this.snackBar.open('Ошибка обновления профиля', 'Закрыть', { duration: 3000 });
          this.loading = false;
        }
      });
    } else {
      // Создание нового профиля
      this.customerProfileService.createProfile({
        userId: currentUser.id,
        ...profileData
      }).subscribe({
        next: (profile) => {
          this.profile = profile;
          this.snackBar.open('Профиль успешно создан', 'Закрыть', { duration: 3000 });
          this.loading = false;
        },
        error: () => {
          this.snackBar.open('Ошибка создания профиля', 'Закрыть', { duration: 3000 });
          this.loading = false;
        }
      });
    }
  }
}
