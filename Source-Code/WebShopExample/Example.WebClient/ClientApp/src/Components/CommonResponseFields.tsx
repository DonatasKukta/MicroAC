import { BaseResult } from '../Domain/Models';
import { ProcessTimestamps, PrintTimestamp } from '../Domain/TimestampProcessor';
import Accordion from './Accordion';
import ProcessedTimestampTable from './ProcessedTimestampTable';

interface IProps<T> {
  response: BaseResult<T>;
}

export default function RequestHandler<T>(props: React.PropsWithChildren<IProps<T>>) {
  const { response } = props;

  return (
    <div>
      <p>HTTP Būsenos kodas: {response.statusCode}</p>
      <p>Užklausos ID: {response.requestId}</p>
      <Accordion title="Užklausos žymos" main={false}>
        <ProcessedTimestampTable timestamps={response.timestamps} />
      </Accordion>
      {props.children}
    </div>
  );
}
