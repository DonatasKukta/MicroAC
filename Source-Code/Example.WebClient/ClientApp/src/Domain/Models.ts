export interface BaseResult<T> {
  timestamps: Timestamp[];
  requestId: string;
  statusCode: number | undefined;
  body: T | undefined;
}

export const defaultBaseResult: BaseResult<any> = {
  timestamps: [],
  requestId: '',
  statusCode: undefined,
  body: undefined
};

export interface LoginResult extends BaseResult<LoginBody> {}

export interface RefreshResult extends BaseResult<RefreshBody> {}

export interface ResouceApiAction extends BaseResult<string> {}

export interface LoginBody {
  accessJwt: Token;
  decodedAccessJwt: string;
  refreshJwt: Token;
  decodedRefreshJwt: string;
}

export interface RefreshBody extends LoginBody {}

export type Token = string | undefined;

export interface Credentials {
  email: string;
  password: string;
}

export interface Timestamp {
  serviceName: string;
  message: string;
  time: string;
}
