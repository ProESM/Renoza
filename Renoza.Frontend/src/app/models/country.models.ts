export interface Country {
  id: number;
  code: string;
  name: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface PhoneCountryCode {
  id: number;
  code: string;
  phoneFormat: string;
  countryId: number;
  countryCode: string;
  countryName: string;
}

export interface PhoneCountryCodeDisplay {
  id: number;
  code: string;
  phoneFormat: string;
  countryCode: string;
  countryName: string;
  flag: string;
  displayText: string;
}
