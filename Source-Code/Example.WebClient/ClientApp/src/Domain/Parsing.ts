import { LoginBody, RefreshBody, Timestamp } from './Models';

export function parseJwt(token: string): any {
  var base64Url = token.split('.')[1];
  var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  var jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join('')
  );
  return JSON.parse(jsonPayload);
}

export function parseTimestamp(timestamp: string): Timestamp {
  var split = timestamp.split('-');
  return {
    serviceName: split[0],
    message: split[1],
    time: split[2]
  };
}

export function parseLoginBody(body: any): LoginBody {
  return {
    accessJwt: body.accessJwt,
    decodedAccessJwt: parseJwt(body.accessJwt),
    refreshJwt: body.refreshJwt,
    decodedRefreshJwt: parseJwt(body.refreshJwt)
  };
}

export function parseRefreshBody(body: any): RefreshBody {
  return parseLoginBody(body);
}

export function parseResourceApiActionBody(body: any): string {
  return body;
}
