import { Button } from '@mui/material';
import { useEffect, useState } from 'react';
import { BaseResult, defaultBaseResult, ResouceApiResult, Token } from '../Domain/Models';
import { parseResourceApiActionBody } from '../Domain/Parsing';
import SendRequest from '../Domain/SendRequest';
import RequestHandler from '../Components/CommonResponseFields';
//TODO: Move to env config
const resourceActionUrl =
  'http://localhost:19083/MicroAC.ServiceFabric/MicroAC.RequestManager/ResourceApi/Action';

interface IProps {
  accessJwt: Token;
}

const ResourceApiActionHandler = (props: IProps) => {
  const { accessJwt } = props;

  const [actionResult, setActionResult] = useState(defaultBaseResult);
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  useEffect(() => {
    if (accessJwt != undefined) setIsButtonDisabled(false);
    else setIsButtonDisabled(true);
  }, [accessJwt]);

  const createRequest = (): RequestInit => {
    return {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        Authorization: accessJwt ?? ''
      }
    };
  };

  const handleSendRequest = () => {
    setActionResult(defaultBaseResult);
    SendRequest(
      resourceActionUrl,
      createRequest(),
      parseResourceApiActionBody,
      onResultReceived
    );
  };

  const onResultReceived = (result: ResouceApiResult) => {
    setIsButtonDisabled(false);
    setActionResult(result);
  };

  return (
    <div>
      <h1> Resurso mikropaslaugos kvietimas </h1>
      <p>Įvestis - prieigos žetonas: {accessJwt}</p>
      <Button variant="contained" onClick={handleSendRequest} disabled={isButtonDisabled}>
        Siųsti
      </Button>
      <RequestHandler response={actionResult}>
        <p>Gauta žinutė: {actionResult?.body?.message}</p>
        <p style={{ overflowWrap: 'anywhere' }}>
          Gautas vidinis priegos žetonas: {actionResult?.body?.internalAccessToken}
        </p>
        Dekoduotas vidinis preigos žetonas:
        <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
          {JSON.stringify(actionResult?.body?.decodedInternalAccessToken, null, '\t')}
        </pre>
      </RequestHandler>
    </div>
  );
};

export default ResourceApiActionHandler;
