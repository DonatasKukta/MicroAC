import { Button } from '@mui/material';
import { useEffect, useState } from 'react';
import { BaseResult, defaultBaseResult, Token } from '../Domain/Models';
import { parseResourceApiActionBody } from '../Domain/Parsing';
import SendRequest, { CreateGetRequest } from '../Domain/SendRequest';
import RequestHandler from './RequestHandler';
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

  const handleSendRequest = () => {
    setActionResult(defaultBaseResult);
    SendRequest(
      resourceActionUrl,
      CreateGetRequest(accessJwt),
      parseResourceApiActionBody,
      onResultReceived
    );
  };

  const onResultReceived = (result: BaseResult<string>) => {
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
        <div>
          Gautas atsakymas: <p>{actionResult.body}</p>
        </div>
      </RequestHandler>
    </div>
  );
};

export default ResourceApiActionHandler;
