export interface Timestamp{
    serviceName: string;
    message:string;
    time: string;
}

export interface IAuthenticationState {
    token:string;
    decodedToken: any,
    credentials: Credentials;
    timestamps: Timestamp[];
}

export interface Credentials {
    email: string;
    password: string;
}

export interface ResouceResult {
    timestamps: string[];
    message: string;
    time: string;
    requestId: string;
}