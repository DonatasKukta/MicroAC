import { TextField, Button } from '@mui/material';
import { useState } from 'react';
import { Credentials, defaultBaseResult, LoginResult, Token } from '../Domain/Models';
import { parseLoginBody } from '../Domain/Parsing';
import SendRequest from '../Domain/SendRequest';
import RequestHandler from './RequestHandler';
//TODO: Move to env config
const authUrl =
  'http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login';

const defaultCredentialsState: Credentials = {
  email: 'Jonas.Jonaitis@gmail.com',
  password: 'passwrd'
};

interface IProps {
  onAccessJwtChange(token: Token): void;
  onRefreshJwtChange(token: Token): void;
}

const LoginHandler = (props: IProps) => {
  const { onAccessJwtChange, onRefreshJwtChange } = props;

  const [loginResult, setLoginResult] = useState(defaultBaseResult);
  const [credentials, setCredentials] = useState(defaultCredentialsState);

  const createRequest = (): RequestInit => {
    return {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(credentials)
    };
  };

  const cleanResultState = () => {
    setLoginResult(defaultBaseResult);
    onAccessJwtChange('');
    onRefreshJwtChange('');
  };
  const setResultState = (result: LoginResult) => {
    setLoginResult(result);
    onAccessJwtChange(result.body?.accessJwt);
    onRefreshJwtChange(result.body?.refreshJwt);
  };

  const handleSendRequest = () => {
    cleanResultState();
    SendRequest(authUrl, createRequest(), parseLoginBody, setResultState);
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
      <RequestHandler response={loginResult}>
        <p style={{ overflowWrap: 'anywhere' }}>
          Gautas išorinis priegos žetonas: {loginResult?.body?.accessJwt}
        </p>
        Dekoduotas išorinis preigos žetonas:
        <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
          {JSON.stringify(loginResult?.body?.decodedAccessJwt, null, '\t')}
        </pre>
        <p style={{ overflowWrap: 'anywhere' }}>
          Gautas išorinis atnaujinimo žetonas: {loginResult?.body?.refreshJwt}
        </p>
        Dekoduotas išorinis atnaujinimo žetonas:
        <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
          {JSON.stringify(loginResult?.body?.decodedRefreshJwt, null, '\t')}
        </pre>
      </RequestHandler>
    </div>
  );
};
export default LoginHandler;
