export enum RoleEnum {
  Administrator = 0,
  Customer = 1,
  Worker = 2,
  TechnicalSupervisor = 3
}

export class RoleId {
  static readonly Administrator = '00000000-0000-0000-0000-000000000001';
  static readonly Customer = '00000000-0000-0000-0000-000000000002';
  static readonly Worker = '00000000-0000-0000-0000-000000000003';
  static readonly TechnicalSupervisor = '00000000-0000-0000-0000-000000000004';
}

export interface Role {
  id: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface UserRole {
  userId: string;
  roleId: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}
