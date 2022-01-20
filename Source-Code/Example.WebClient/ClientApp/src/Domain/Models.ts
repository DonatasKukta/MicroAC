export interface ResponseResult {
  token: string;
  decodedToken: any;
  timestamps: Timestamp[];
  requestId: string;
  statusCode: number | undefined;
}

export interface LoginResult extends ResponseResult {}

export interface Credentials {
  email: string;
  password: string;
}

export interface Timestamp {
  serviceName: string;
  message: string;
  time: string;
}

export interface ResouceResult {
  timestamps: string[];
  message: string;
  time: string;
  requestId: string;
}
