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
import { MatSelectModule } from '@angular/material/select';
import { AuthService } from '../../../services/auth.service';
import { PhoneCountryCodeService } from '../../../services/phone-country-code.service';
import { PhoneCountryCodeDisplay } from '../../../models/country.models';

@Component({
  selector: 'app-login',
  standalone: true,
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
    MatSelectModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  phoneLoginForm!: FormGroup;
  loading = false;
  error = '';
  hidePassword = true;
  hidePhonePassword = true;
  returnUrl: string = '';
  phoneCountryCodes: PhoneCountryCodeDisplay[] = [];
  selectedCountry?: PhoneCountryCodeDisplay;
  selectedTabIndex = 0;
  formattedPhonePreview: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private phoneCountryCodeService: PhoneCountryCodeService
  ) {}

  ngOnInit(): void {
    // Инициализация формы входа по имени пользователя
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    // Инициализация формы входа по телефону
    this.phoneLoginForm = this.formBuilder.group({
      phoneCountryCode: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      password: ['', Validators.required]
    });

    // Загружаем список телефонных кодов стран
    this.phoneCountryCodeService.getPhoneCountryCodes().subscribe({
      next: (codes) => {
        console.log('Loaded phone country codes:', codes);
        this.phoneCountryCodes = codes;
        // Устанавливаем Россию по умолчанию
        const defaultCode = codes.find(c => c.countryCode === 'RU');
        if (defaultCode) {
          console.log('Setting default country:', defaultCode);
          this.selectedCountry = defaultCode;
          this.phoneLoginForm.patchValue({ phoneCountryCode: defaultCode.id });
          this.updateFormattedPhonePreview();
        } else {
          console.warn('Default country (RU) not found');
        }
      },
      error: (error) => {
        console.error('Ошибка при загрузке телефонных кодов:', error);
      }
    });

    // Отслеживаем изменения выбора кода страны
    this.phoneLoginForm.get('phoneCountryCode')?.valueChanges.subscribe(id => {
      this.selectedCountry = this.phoneCountryCodes.find(c => c.id === id);
      console.log('Selected country changed:', this.selectedCountry);
      this.updateFormattedPhonePreview();
    });

    // Отслеживаем изменения номера телефона для обновления превью и автоформатирования
    this.phoneLoginForm.get('phoneNumber')?.valueChanges.subscribe((value) => {
      if (this.selectedCountry?.phoneFormat) {
        const digitsOnly = (value || '').replace(/\D/g, '');
        let formatted = '';
        let digitIndex = 0;

        for (let char of this.selectedCountry.phoneFormat) {
          if (char === 'X' && digitIndex < digitsOnly.length) {
            formatted += digitsOnly[digitIndex++];
          } else if (char !== 'X') {
            formatted += char;
          } else {
            break;
          }
        }

        // Обновляем значение только если оно изменилось
        if (formatted !== value) {
          this.phoneLoginForm.get('phoneNumber')?.setValue(formatted, { emitEvent: false });
        }
      }
      this.updateFormattedPhonePreview();
    });

    // Получаем URL для возврата после успешного входа
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/home';

    // Если пользователь уже аутентифицирован, перенаправляем
    if (this.authService.hasValidToken()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  get f() {
    return this.loginForm.controls;
  }

  get pf() {
    return this.phoneLoginForm.controls;
  }

  /**
   * Обновляет превью форматированного телефонного номера
   */
  updateFormattedPhonePreview(): void {
    if (!this.selectedCountry?.phoneFormat) {
      this.formattedPhonePreview = '';
      return;
    }

    const phoneNumber = this.phoneLoginForm.get('phoneNumber')?.value || '';
    const digitsOnly = phoneNumber.replace(/\D/g, '');
    let format = this.selectedCountry.phoneFormat;
    let digitIndex = 0;

    // Заменяем X на введённые цифры, оборачивая их в span для стилизации
    let result = '';
    for (let i = 0; i < format.length; i++) {
      const char = format[i];
      if (char === 'X') {
        if (digitIndex < digitsOnly.length) {
          result += `<span class="entered-digit">${digitsOnly[digitIndex++]}</span>`;
        } else {
          result += `<span class="placeholder-digit">X</span>`;
        }
      } else {
        result += char;
      }
    }

    this.formattedPhonePreview = result;
  }

  onSubmit(): void {
    this.error = '';

    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    const loginRequest = {
      ...this.loginForm.value,
      loginType: 'username' as const
    };

    this.authService.login(loginRequest).subscribe({
      next: () => {
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        console.error('Ошибка входа:', error);
        this.error = error.error?.message || 'Неверное имя пользователя или пароль';
        this.loading = false;
      }
    });
  }

  onPhoneSubmit(): void {
    this.error = '';

    if (this.phoneLoginForm.invalid) {
      return;
    }

    this.loading = true;

    const formValue = this.phoneLoginForm.value;
    // Извлекаем только цифры из номера телефона
    const digitsOnly = (formValue.phoneNumber || '').replace(/\D/g, '');
    // Формируем полный номер телефона в формате: код страны + номер
    const phoneLogin = {
      username: `${this.selectedCountry?.code}${digitsOnly}`,
      password: formValue.password,
      loginType: 'phone' as const
    };

    this.authService.login(phoneLogin).subscribe({
      next: () => {
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        console.error('Ошибка входа:', error);
        this.error = error.error?.message || 'Неверный номер телефона или пароль';
        this.loading = false;
      }
    });
  }
}
