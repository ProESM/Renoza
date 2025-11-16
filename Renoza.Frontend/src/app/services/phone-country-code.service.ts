import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, shareReplay } from 'rxjs';
import { environment } from '../../environments/environment';
import { PhoneCountryCode, PhoneCountryCodeDisplay } from '../models/country.models';

@Injectable({
  providedIn: 'root'
})
export class PhoneCountryCodeService {
  private cache$?: Observable<PhoneCountryCodeDisplay[]>;

  // Маппинг кодов стран на эмодзи флагов
  private readonly FLAG_EMOJI_MAP: { [key: string]: string } = {
    'RU': '🇷🇺', 'KZ': '🇰🇿', 'BY': '🇧🇾', 'UA': '🇺🇦',
    'US': '🇺🇸', 'CA': '🇨🇦', 'GB': '🇬🇧', 'DE': '🇩🇪',
    'FR': '🇫🇷', 'IT': '🇮🇹', 'ES': '🇪🇸', 'PL': '🇵🇱',
    'TR': '🇹🇷', 'CN': '🇨🇳', 'JP': '🇯🇵', 'IN': '🇮🇳',
    'BR': '🇧🇷', 'AU': '🇦🇺', 'NZ': '🇳🇿', 'IL': '🇮🇱',
    'AE': '🇦🇪', 'KG': '🇰🇬', 'UZ': '🇺🇿', 'TJ': '🇹🇯',
    'AM': '🇦🇲', 'GE': '🇬🇪', 'AZ': '🇦🇿', 'MD': '🇲🇩',
    'EE': '🇪🇪', 'LV': '🇱🇻', 'LT': '🇱🇹'
  };

  constructor(private http: HttpClient) { }

  /**
   * Получить список активных международных телефонных кодов
   * Результат кэшируется для повышения производительности
   */
  getPhoneCountryCodes(): Observable<PhoneCountryCodeDisplay[]> {
    if (!this.cache$) {
      this.cache$ = this.http.get<PhoneCountryCode[]>(`${environment.apiUrl}/phonecountrycodes`)
        .pipe(
          map(codes => this.mapToDisplayFormat(codes)),
          shareReplay(1)
        );
    }
    return this.cache$;
  }

  /**
   * Преобразование данных API в формат для отображения
   */
  private mapToDisplayFormat(codes: PhoneCountryCode[]): PhoneCountryCodeDisplay[] {
    return codes.map(code => ({
      id: code.id,
      code: code.code,
      phoneFormat: code.phoneFormat,
      countryCode: code.countryCode,
      countryName: code.countryName,
      flag: this.getFlag(code.countryCode),
      displayText: `${this.getFlag(code.countryCode)} ${code.countryName} (${code.code})`
    }));
  }

  /**
   * Получить эмодзи флага по коду страны
   */
  private getFlag(countryCode: string): string {
    return this.FLAG_EMOJI_MAP[countryCode] || '🏳️';
  }

  /**
   * Найти телефонный код по коду страны
   */
  findByCountryCode(countryCode: string): Observable<PhoneCountryCodeDisplay | undefined> {
    return this.getPhoneCountryCodes().pipe(
      map(codes => codes.find(code => code.countryCode === countryCode))
    );
  }

  /**
   * Получить телефонный код по умолчанию (Россия)
   */
  getDefaultPhoneCountryCode(): Observable<PhoneCountryCodeDisplay | undefined> {
    return this.findByCountryCode('RU');
  }

  /**
   * Очистить кэш
   */
  clearCache(): void {
    this.cache$ = undefined;
  }
}
