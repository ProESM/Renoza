export interface CustomerProfile {
  id: string;
  userId: string;
  companyName?: string;
  taxId?: string;
  billingAddress?: string;
  creditLimit?: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface WorkerProfile {
  id: string;
  userId: string;
  specialization?: string;
  teamSize?: number;
  certifications?: string[];
  professionalStartDate?: string; // ISO date string (DateOnly on backend)
  isAvailable: boolean;
  rating?: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateCustomerProfileRequest {
  userId: string;
  companyName?: string;
  taxId?: string;
  billingAddress?: string;
  creditLimit?: number;
}

export interface UpdateCustomerProfileRequest {
  id: string;
  companyName?: string;
  taxId?: string;
  billingAddress?: string;
  creditLimit?: number;
}

export interface CreateWorkerProfileRequest {
  userId: string;
  specialization?: string;
  teamSize?: number;
  certifications?: string[];
  professionalStartDate?: string;
  rating?: number;
}

export interface UpdateWorkerProfileRequest {
  id: string;
  specialization?: string;
  teamSize?: number;
  certifications?: string[];
  professionalStartDate?: string;
  rating?: number;
}
