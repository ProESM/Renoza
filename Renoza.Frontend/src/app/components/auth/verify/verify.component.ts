import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-verify',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatSnackBarModule
  ],
  templateUrl: './verify.component.html',
  styleUrl: './verify.component.scss'
})
export class VerifyComponent implements OnInit {
  emailVerifyForm!: FormGroup;
  phoneVerifyForm!: FormGroup;
  loading = false;
  error = '';
  userId: string = '';
  email: string = '';
  phoneNumber: string = '';
  phoneCountryCode: string = '';
  selectedTabIndex = 0;
  emailVerified = false;
  phoneVerified = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    // Получаем данные из query параметров
    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'] || '';
      this.email = params['email'] || '';
      this.phoneNumber = params['phoneNumber'] || '';
      this.phoneCountryCode = params['phoneCountryCode'] || '';
    });

    this.emailVerifyForm = this.formBuilder.group({
      email: [this.email, [Validators.required, Validators.email]],
      code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
    });

    this.phoneVerifyForm = this.formBuilder.group({
      phoneNumber: [this.phoneNumber, Validators.required],
      phoneCountryCode: [this.phoneCountryCode, Validators.required],
      code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
    });
  }

  onEmailVerify(): void {
    if (this.emailVerifyForm.invalid) {
      this.markFormGroupTouched(this.emailVerifyForm);
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.verifyEmail(this.emailVerifyForm.value).subscribe({
      next: () => {
        this.loading = false;
        this.emailVerified = true;
        this.snackBar.open('Email успешно подтвержден!', 'OK', { duration: 3000 });

        // Если оба подтверждены, переходим на страницу логина
        if (this.phoneVerified) {
          setTimeout(() => {
            this.router.navigate(['/auth/login']);
          }, 2000);
        }
      },
      error: (error: any) => {
        this.loading = false;
        this.error = error.error?.message || 'Произошла ошибка при верификации email';
        console.error('Ошибка верификации email:', error);
      }
    });
  }

  onPhoneVerify(): void {
    if (this.phoneVerifyForm.invalid) {
      this.markFormGroupTouched(this.phoneVerifyForm);
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.verifyPhone(this.phoneVerifyForm.value).subscribe({
      next: () => {
        this.loading = false;
        this.phoneVerified = true;
        this.snackBar.open('Телефон успешно подтвержден!', 'OK', { duration: 3000 });

        // Если оба подтверждены, переходим на страницу логина
        if (this.emailVerified) {
          setTimeout(() => {
            this.router.navigate(['/auth/login']);
          }, 2000);
        }
      },
      error: (error: any) => {
        this.loading = false;
        this.error = error.error?.message || 'Произошла ошибка при верификации телефона';
        console.error('Ошибка верификации телефона:', error);
      }
    });
  }

  resendEmailCode(): void {
    if (!this.userId || !this.email) {
      this.snackBar.open('Не удалось определить пользователя', 'OK', { duration: 3000 });
      return;
    }

    this.authService.sendVerificationCode({
      userId: this.userId,
      verificationType: 'email',
      email: this.email
    }).subscribe({
      next: () => {
        this.snackBar.open('Код верификации отправлен на email', 'OK', { duration: 3000 });
      },
      error: (error: any) => {
        this.snackBar.open(error.error?.message || 'Ошибка отправки кода', 'OK', { duration: 3000 });
      }
    });
  }

  resendPhoneCode(): void {
    if (!this.userId || !this.phoneNumber || !this.phoneCountryCode) {
      this.snackBar.open('Не удалось определить пользователя', 'OK', { duration: 3000 });
      return;
    }

    this.authService.sendVerificationCode({
      userId: this.userId,
      verificationType: 'phone',
      phoneNumber: this.phoneNumber,
      phoneCountryCode: this.phoneCountryCode
    }).subscribe({
      next: () => {
        this.snackBar.open('Код верификации отправлен на телефон', 'OK', { duration: 3000 });
      },
      error: (error: any) => {
        this.snackBar.open(error.error?.message || 'Ошибка отправки кода', 'OK', { duration: 3000 });
      }
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  get ef() {
    return this.emailVerifyForm.controls;
  }

  get pf() {
    return this.phoneVerifyForm.controls;
  }
}
