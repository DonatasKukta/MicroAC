import { Button } from '@mui/material';
import { useEffect, useState } from 'react';
import { Credentials, defaultBaseResult, LoginResult, Token } from '../Domain/Models';
import { parseLoginBody } from '../Domain/Parsing';
import SendRequest from '../Domain/SendRequest';
import RequestHandler from './CommonResponseFields';
//TODO: Move to env config
const authUrl =
  'http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/Authentication/Login';

const defaultCredentialsState: Credentials = {
  email: 'Jonas.Jonaitis@gmail.com',
  password: 'passwrd'
};

interface IProps {
  refreshJwt: Token;
}

const LoginHandler = (props: IProps) => {
  const { refreshJwt } = props;

  const [refreshResult, setRefreshResult] = useState(defaultBaseResult);
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  useEffect(() => {
    if (refreshJwt != undefined) setIsButtonDisabled(false);
    else setIsButtonDisabled(true);
  }, [refreshJwt]);

  const createRequest = (): RequestInit => {
    return {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(refreshJwt)
    };
  };

  const handleSendRequest = () => {
    SendRequest(authUrl, createRequest(), parseLoginBody, setRefreshResult);
  };

  return (
    <div>
      <h1> Resurso mikropaslaugos kvietimas </h1>
      <p>Įvestis - atnaujinimo žetonas: {refreshJwt}</p>
      <Button variant="contained" onClick={handleSendRequest} disabled={isButtonDisabled}>
        Siųsti
      </Button>
      <RequestHandler response={refreshResult}>
        <p style={{ overflowWrap: 'anywhere' }}>
          Gautas išorinis priegos žetonas: {refreshResult?.body?.accessJwt}
        </p>
        Dekoduotas išorinis preigos žetonas:
        <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
          {JSON.stringify(refreshResult?.body?.decodedAccessJwt, null, '\t')}
        </pre>
        <p style={{ overflowWrap: 'anywhere' }}>
          Gautas išorinis atnaujinimo žetonas: {refreshResult?.body?.refreshJwt}
        </p>
        Dekoduotas išorinis atnaujinimo žetonas:
        <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
          {JSON.stringify(refreshResult?.body?.decodedRefreshJwt, null, '\t')}
        </pre>
      </RequestHandler>
    </div>
  );
};
export default LoginHandler;
