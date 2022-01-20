import { BaseResult, defaultBaseResult, Token } from './Models';
import { parseTimestamp } from './Parsing';

function SendRequest<T>(
  url: string,
  request: RequestInit,
  parseBody: (body: any) => T,
  responseCallback: (result: BaseResult<T>) => void
) {
  let result: BaseResult<T> = { ...defaultBaseResult };

  fetch(url, request)
    .then(response => {
      result.statusCode = response.status;
      result.requestId = response.headers.get('X-ServiceFabricRequestId') ?? '';

      const timestampsHeaders = response.headers.get('MicroAC-Timestamp')?.split(',');
      result.timestamps = timestampsHeaders
        ? timestampsHeaders?.map(t => parseTimestamp(t))
        : [];

      if (!response.ok) {
        console.error(response, response.text());
        throw new Error();
      }

      return response;
    })
    .then(response => response.text())
    .then(bodyStr => {
      result.body = parseBody(bodyStr);
    })
    .catch(error => console.error('Error from SendRequest.', url, request, result, error))
    .finally(() => {
      responseCallback(result);
    });
}

export default SendRequest;

export const CreatePostRequest = (body: any): RequestInit => {
  return {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
  };
};

export const CreateGetRequest = (authToken: Token): RequestInit => {
  return {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: authToken ?? ''
    }
  };
};
