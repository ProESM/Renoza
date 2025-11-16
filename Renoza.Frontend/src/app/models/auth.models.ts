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
