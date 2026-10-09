export interface User {
  email: string;
  username: string;
  roles: string[];
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiration: string;
  email: string;
  username: string;
  roles: string[];
}
