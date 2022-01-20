import { useState } from 'react';
import { LoginResult, Credentials } from '../Domain/Models';
import { parseLoginBody } from '../Domain/Parsing';
import { Button, TextField } from '@mui/material';
import SendRequest, { CreateRequest } from '../Domain/SendRequest';

//TODO: Move to env config
const authUrl =
  'http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login';

const defaultLoginResult: LoginResult = {
  timestamps: [],
  requestId: '',
  statusCode: undefined,
  body: undefined
};

const defaultCredentialsState: Credentials = {
  email: 'Jonas.Jonaitis@gmail.com',
  password: 'passwrd'
};

export default function RequestHandler() {
  const [loginResult, setLoginResult] = useState(defaultLoginResult);
  const [credentials, setCredentials] = useState(defaultCredentialsState);

  const handleSendRequest = () => {
    setLoginResult(defaultLoginResult);
    SendRequest(
      authUrl,
      CreateRequest('POST', credentials),
      defaultLoginResult,
      parseLoginBody,
      result => {
        setLoginResult(result);
      }
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
      <Button variant="contained" onClick={() => handleSendRequest()}>
        Siųsti
      </Button>
      <p>HTTP Būsenos kodas: {loginResult.statusCode}</p>
      <p>Užklausos ID: {loginResult.requestId}</p>
      <p style={{ overflowWrap: 'anywhere' }}>
        Gautas išorinis žetonas: {loginResult?.body?.accessJwt}
      </p>
      Dekoduotas išorinis preigos žetonas:
      <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
        {JSON.stringify(loginResult?.body?.decodedAccessJwt, null, '\t')}
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
