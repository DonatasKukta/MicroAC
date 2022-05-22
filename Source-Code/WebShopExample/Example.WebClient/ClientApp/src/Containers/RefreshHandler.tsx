import { Button } from '@mui/material';
import { useEffect, useState } from 'react';
import { defaultBaseResult, RefreshResult, Token } from '../Domain/Models';
import { parseJwt, parseRefreshBody } from '../Domain/Parsing';
import SendRequest from '../Domain/SendRequest';
import RequestHandler from '../Components/CommonResponseFields';
import Accordion from '../Components/Accordion';
import { getUrl, Service, Action } from '../Domain/WebShopRequestCreator';

const authUrl = getUrl(Service.Authentication, Action.Refresh);

interface IProps {
  refreshJwt: Token;
}

const RefreshHandler = (props: IProps) => {
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
      body: refreshJwt ?? ''
    };
  };

  const handleSendRequest = () => {
    setIsButtonDisabled(true);
    SendRequest<Token>(authUrl, createRequest(), parseRefreshBody, onResultReceived);
  };

  const onResultReceived = (result: RefreshResult) => {
    setIsButtonDisabled(false);
    setRefreshResult(result);
  };

  return (
    <Accordion title="Prieigos žetono atnaujinimas" main>
      <Accordion title="Įvestis - atnaujinimo žetonas" main={false}>
        <p style={{ overflowWrap: 'anywhere' }}> {refreshJwt}</p>
      </Accordion>
      <Button
        style={{ margin: '10px' }}
        variant="contained"
        onClick={handleSendRequest}
        disabled={isButtonDisabled}>
        Siųsti
      </Button>
      <RequestHandler response={refreshResult}>
        <p style={{ overflowWrap: 'anywhere' }}>
          Gautas išorinis priegos žetonas: {refreshResult?.body}
        </p>
        Dekoduotas išorinis preigos žetonas:
        <pre style={{ textAlign: 'left', overflowWrap: 'anywhere' }}>
          {refreshResult?.body &&
            JSON.stringify(parseJwt(refreshResult.body), null, '\t')}
        </pre>
      </RequestHandler>
    </Accordion>
  );
};
export default RefreshHandler;
