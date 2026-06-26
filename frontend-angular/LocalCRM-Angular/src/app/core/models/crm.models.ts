export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  username: string;
  permissions: string[];
}

export interface CompanyDto {
  companyId: number;
  companyRef: string;
  name: string;
  city: string;
  companyType: string;
  rating: number;
}
