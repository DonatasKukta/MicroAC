export interface BaseResult<T> {
  timestamps: Timestamp[];
  requestId: string;
  statusCode: number | undefined;
  body: T | undefined;
}

export interface LoginResult extends BaseResult<LoginBody> {}
export interface RefreshResult extends BaseResult<RefreshBody> {}

export interface LoginBody {
  accessJwt: Token;
  decodedAccessJwt: string;
  refreshJwt: Token;
  decodedRefreshJwt: string;
}

export interface RefreshBody extends LoginBody {}

type Token = string;

export interface Credentials {
  email: string;
  password: string;
}

export interface Timestamp {
  serviceName: string;
  message: string;
  time: string;
}
