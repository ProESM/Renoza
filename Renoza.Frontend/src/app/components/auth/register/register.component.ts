import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../services/auth.service';
import { PhoneCountryCodeService } from '../../../services/phone-country-code.service';
import { PhoneCountryCodeDisplay } from '../../../models/country.models';

@Component({
  selector: 'app-register',
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
    MatSelectModule,
    MatSnackBarModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  loading = false;
  error = '';
  hidePassword = true;
  phoneCountryCodes: PhoneCountryCodeDisplay[] = [];
  selectedCountry?: PhoneCountryCodeDisplay;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private phoneCountryCodeService: PhoneCountryCodeService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      displayName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      phoneCountryCode: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^[0-9]+$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.loadCountryCodes();
  }

  loadCountryCodes(): void {
    this.phoneCountryCodeService.getPhoneCountryCodes().subscribe({
      next: (codes: PhoneCountryCodeDisplay[]) => {
        this.phoneCountryCodes = codes;
        if (codes.length > 0) {
          const russiaCode = codes.find((c: PhoneCountryCodeDisplay) => c.code === '+7');
          this.selectedCountry = russiaCode || codes[0];
          if (this.selectedCountry) {
            this.registerForm.patchValue({
              phoneCountryCode: this.selectedCountry.code
            });
          }
        }
      },
      error: (error: any) => {
        console.error('Ошибка при загрузке кодов стран:', error);
        this.error = 'Не удалось загрузить коды стран';
      }
    });
  }

  onCountryChange(countryCode: string): void {
    this.selectedCountry = this.phoneCountryCodes.find(c => c.code === countryCode);
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched(this.registerForm);
      return;
    }

    this.loading = true;
    this.error = '';

    const formValue = this.registerForm.value;

    this.authService.register(formValue).subscribe({
      next: (response) => {
        this.loading = false;
        this.snackBar.open('Регистрация успешна! Проверьте ваш email и телефон для верификации.', 'OK', {
          duration: 5000
        });

        // Переходим на страницу верификации с данными пользователя
        this.router.navigate(['/auth/verify'], {
          queryParams: {
            userId: response.userId,
            email: response.email,
            phoneNumber: response.phoneNumber,
            phoneCountryCode: response.phoneCountryCode
          }
        });
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Произошла ошибка при регистрации';
        console.error('Ошибка регистрации:', error);
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

  get f() {
    return this.registerForm.controls;
  }
}
