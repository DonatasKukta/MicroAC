import { BaseResult } from './Models';
import { parseTimestamp } from './Parsing';

export function CreateRequest(method: string, body: any): RequestInit {
  return {
    method: method,
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
  };
}

function SendRequest<T>(
  url: string,
  request: RequestInit,
  initial: BaseResult<T>,
  parseBody: (body: any) => T,
  responseCallback: (result: BaseResult<T>) => void
) {
  let result: BaseResult<T> = { ...initial };

  fetch(url, request)
    .then(response => {
      result.statusCode = response.status;
      result.requestId = response.headers.get('X-ServiceFabricRequestId') ?? '';

      var timestampsHeaders = response.headers.get('MicroAC-Timestamp')?.split(',');
      result.timestamps = timestampsHeaders
        ? timestampsHeaders?.map(t => parseTimestamp(t))
        : [];

      return response;
    })
    .then(response => response.json())
    .then(body => {
      result.body = parseBody(body);
    })
    .finally(() => {
      responseCallback(result);
    });
}

export default SendRequest;
