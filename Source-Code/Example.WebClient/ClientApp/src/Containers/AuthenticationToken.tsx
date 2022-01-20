import { useCallback, useEffect, useState } from 'react';
import { LoginResult, Credentials } from '../Domain/Models';
import { parseJwt, parseTimestamp } from '../Domain/Parsing';
import { Button, TextField } from '@mui/material';

//TODO: Move to env config
const authUrl =
  'http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login';

const defaultLoginResult: LoginResult = {
  token: '',
  decodedToken: '',
  timestamps: [],
  requestId: '',
  statusCode: undefined
};
const defaultCredentialsState: Credentials = {
  email: 'Jonas.Jonaitis@gmail.com',
  password: 'passwrd'
};

function AuthenticationToken() {
  const [loginResult, setLoginResult] = useState(defaultLoginResult);
  const [credentials, setCredentials] = useState(defaultCredentialsState);

  const sendLoginRequest = () => {
    let result: LoginResult = {
      token: '',
      decodedToken: '',
      timestamps: [],
      requestId: '',
      statusCode: undefined
    };
    setLoginResult(defaultLoginResult);
    const requestOptions = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(credentials)
    };

    fetch(authUrl, requestOptions)
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
      .then(data => {
        result.token = data.accessJwt;
        result.decodedToken = parseJwt(data.accessJwt);
      })
      .finally(() => {
        setLoginResult(result);
      });
  };

  void function SendResourceRequest() {
    const requestOptions = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(credentials)
    };

    fetch(authUrl, requestOptions)
      .then(response => response.json())
      .then(data =>
        setLoginResult({
          ...loginResult,
          token: data.accessJwt
        })
      );
  };

  return (
    <div>
      <h1> Kliento autentifikavimas</h1>
      <TextField
        label="Email"
        variant="filled"
        value={credentials.email}
        onChange={d => setCredentials({ ...credentials, email: d.target.value })}
      />
      <TextField
        label="Password"
        variant="filled"
        type="password"
        value={credentials.password}
        onChange={d => setCredentials({ ...credentials, password: d.target.value })}
      />
      <Button variant="contained" onClick={() => sendLoginRequest()}>
        Siųsti
      </Button>
      <p>HTTP Būsenos kodas: {loginResult.statusCode}</p>
      <p>Užklausos ID: {loginResult.requestId}</p>
      <p style={{ overflowWrap: 'anywhere' }}>
        Gautas išorinis žetonas: {loginResult.token}
      </p>
      Dekoduotas išorinis preigos žetonas:
      <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
        {JSON.stringify(loginResult.decodedToken, null, '\t')}
      </pre>
      <div>
        Užklausos žymos:
        {loginResult.timestamps.map((t, i) => (
          <div key={i + t.time}>
            {t.serviceName}-{t.message}-{t.time}
          </div>
        ))}
      </div>
    </div>
  );
}

export default AuthenticationToken;
