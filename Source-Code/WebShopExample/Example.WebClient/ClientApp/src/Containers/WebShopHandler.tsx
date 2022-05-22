import { Button } from '@mui/material';
import { useEffect, useState } from 'react';
import Accordion from '../Components/Accordion';
import RequestHandler from '../Components/CommonResponseFields';
import { BaseResult, defaultBaseResult, Token } from '../Domain/Models';
import { Service, Action, SendWebShopRequest } from '../Domain/WebShopRequestCreator';

export interface IProps {
  title: string;
  service: Service;
  action: Action;
  accessJwt: Token;
}

export const WebShopHandler = (props: IProps) => {
  const { title, accessJwt, service, action } = props;

  const [actionResult, setActionResult] = useState(defaultBaseResult);
  const [isButtonDisabled, setIsButtonDisabled] = useState(true);

  useEffect(() => {
    if (accessJwt != undefined) setIsButtonDisabled(false);
    else setIsButtonDisabled(true);
  }, [accessJwt]);

  const handleSendRequest = () => {
    setIsButtonDisabled(true);
    setActionResult(defaultBaseResult);
    SendWebShopRequest(service, action, accessJwt, onResultReceived);
  };

  const onResultReceived = (result: BaseResult<string>) => {
    setIsButtonDisabled(false);
    setActionResult(result);
    console.log(result);
  };

  return (
    <Accordion title={title} main>
      <div>
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
          <Accordion title="Gautas Atsakymas" main={false}>
            <pre style={{ textAlign: 'left', whiteSpace: 'pre-wrap' }}>
              {JSON.stringify(actionResult?.body, null, '\t')}
            </pre>
          </Accordion>
        </RequestHandler>
      </div>
    </Accordion>
  );
};
