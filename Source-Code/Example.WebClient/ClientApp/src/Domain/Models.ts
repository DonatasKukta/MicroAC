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

export interface RefreshResult extends BaseResult<Token> {}

export interface ResouceApiResult extends BaseResult<ResourceApiBody> {}

export interface ResourceApiBody {
  message: string;
  internalAccessToken: Token;
  decodedInternalAccessToken: Token;
}

export interface LoginBody {
  accessJwt: Token;
  decodedAccessJwt: Token;
  refreshJwt: Token;
  decodedRefreshJwt: Token;
}

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
