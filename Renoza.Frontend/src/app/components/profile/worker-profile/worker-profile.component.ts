import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { WorkerProfileService } from '../../../services/worker-profile.service';
import { AuthService } from '../../../services/auth.service';
import { WorkerProfile } from '../../../models/profile.models';

@Component({
  selector: 'app-worker-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatChipsModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './worker-profile.component.html',
  styleUrl: './worker-profile.component.scss'
})
export class WorkerProfileComponent implements OnInit {
  profileForm: FormGroup;
  profile: WorkerProfile | null = null;
  loading = false;
  certifications: string[] = [];
  newCertification = '';

  constructor(
    private fb: FormBuilder,
    private workerProfileService: WorkerProfileService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.profileForm = this.fb.group({
      specialization: ['', [Validators.maxLength(256)]],
      teamSize: [null, [Validators.min(1), Validators.max(1000)]],
      professionalStartDate: [null],
      isAvailable: [true],
      rating: [null, [Validators.min(0), Validators.max(5)]]
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
    this.workerProfileService.getProfileByUserId(currentUser.id).subscribe({
      next: (profile) => {
        this.profile = profile;
        this.certifications = profile.certifications || [];
        this.profileForm.patchValue({
          specialization: profile.specialization,
          teamSize: profile.teamSize,
          professionalStartDate: profile.professionalStartDate,
          isAvailable: profile.isAvailable,
          rating: profile.rating
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

  addCertification(): void {
    const cert = this.newCertification.trim();
    if (cert && !this.certifications.includes(cert)) {
      this.certifications.push(cert);
      this.newCertification = '';
    }
  }

  removeCertification(cert: string): void {
    const index = this.certifications.indexOf(cert);
    if (index >= 0) {
      this.certifications.splice(index, 1);
    }
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
      specialization: formValue.specialization,
      teamSize: formValue.teamSize,
      certifications: this.certifications,
      professionalStartDate: formValue.professionalStartDate,
      rating: formValue.rating
    };

    if (this.profile) {
      // Обновление существующего профиля
      this.workerProfileService.updateProfile(this.profile.id, {
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
      this.workerProfileService.createProfile({
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

  toggleAvailability(): void {
    if (!this.profile) {
      return;
    }

    const newAvailability = !this.profile.isAvailable;
    this.workerProfileService.setAvailability(this.profile.id, newAvailability).subscribe({
      next: () => {
        if (this.profile) {
          this.profile.isAvailable = newAvailability;
        }
        this.profileForm.patchValue({ isAvailable: newAvailability });
        this.snackBar.open(
          `Вы отмечены как ${newAvailability ? 'доступный' : 'недоступный'}`,
          'Закрыть',
          { duration: 3000 }
        );
      },
      error: () => {
        this.snackBar.open('Ошибка изменения доступности', 'Закрыть', { duration: 3000 });
      }
    });
  }
}
