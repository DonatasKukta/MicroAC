import { useState } from 'react';
import { BaseResult } from '../Domain/Models';
import { parseLoginBody } from '../Domain/Parsing';
import { Button, TextField } from '@mui/material';
import SendRequest, { CreateRequest } from '../Domain/SendRequest';

interface IProps<T> {
  response: BaseResult<T>;
}

export default function RequestHandler<T>(props: React.PropsWithChildren<IProps<T>>) {
  const { response } = props;

  return (
    <div>
      {props.children}
      <p>HTTP Būsenos kodas: {response.statusCode}</p>
      <p>Užklausos ID: {response.requestId}</p>
      <div>
        Užklausos žymos:
        {response.timestamps.map((t, i) => (
          <div key={i + t.time}>
            {t.serviceName}-{t.message}-{t.time}
          </div>
        ))}
      </div>
    </div>
  );
}
