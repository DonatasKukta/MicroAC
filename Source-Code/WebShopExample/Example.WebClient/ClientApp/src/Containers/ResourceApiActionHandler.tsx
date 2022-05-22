import { Button } from '@mui/material';
import { useEffect, useState } from 'react';
import { BaseResult, defaultBaseResult, ResouceApiResult, Token } from '../Domain/Models';
import { parseResourceApiActionBody } from '../Domain/Parsing';
import SendRequest from '../Domain/SendRequest';
import RequestHandler from '../Components/CommonResponseFields';
import Accordion from '../Components/Accordion';
import { Action, getUrl, Service } from '../Domain/WebShopRequestCreator';

const resourceActionUrl = getUrl(Service.ResourceApi, Action.GetOne);

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
    <Accordion title="Resurso mikropaslaugos kvietimas" main>
      <Accordion title="Įvestis - prieigos žetonas" main={false}>
        <p style={{ overflowWrap: 'anywhere' }}> {accessJwt}</p>
      </Accordion>
      <Button
        style={{ margin: '10px' }}
        variant="contained"
        onClick={handleSendRequest}
        disabled={isButtonDisabled}>
        Siųsti
      </Button>
      <RequestHandler response={actionResult}>
        <Accordion title={`Gauta žinutė: ${actionResult?.body?.message}`} main={false}>
          <p style={{ overflowWrap: 'anywhere' }}>
            Gautas vidinis priegos žetonas: {actionResult?.body?.internalAccessToken}
          </p>
          Dekoduotas vidinis preigos žetonas:
          <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
            {JSON.stringify(actionResult?.body?.decodedInternalAccessToken, null, '\t')}
          </pre>
        </Accordion>
      </RequestHandler>
    </Accordion>
  );
};

export default ResourceApiActionHandler;
