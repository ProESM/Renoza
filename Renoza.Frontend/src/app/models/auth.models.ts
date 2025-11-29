export interface LoginRequest {
  username: string;
  password: string;
  loginType?: 'username' | 'email' | 'phone';
}

export interface AuthResponse {
  token: string;
  expiresAt: Date;
  tokenType: string;
  userId: string;
  username: string;
}

export interface ResetPasswordRequest {
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface User {
  id: string;
  name: string;
  displayName: string;
  email: string;
  isEmailVerified: boolean;
  phoneNumber: string;
  phoneCountryCode: string;
  isPhoneNumberVerified: boolean;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface RegisterRequest {
  name: string;
  displayName: string;
  email: string;
  phoneNumber: string;
  phoneCountryCode: string;
  password: string;
}

export interface RegisterResponse {
  userId: string;
  username: string;
  email: string;
  phoneNumber: string;
  phoneCountryCode: string;
  message: string;
}

export interface VerifyEmailRequest {
  email: string;
  code: string;
}

export interface VerifyPhoneRequest {
  phoneNumber: string;
  phoneCountryCode: string;
  code: string;
}

export interface SendVerificationCodeRequest {
  userId: string;
  verificationType: 'email' | 'phone';
  email?: string;
  phoneNumber?: string;
  phoneCountryCode?: string;
}
