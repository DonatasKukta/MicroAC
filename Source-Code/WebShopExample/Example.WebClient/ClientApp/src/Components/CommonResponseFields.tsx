import { BaseResult } from '../Domain/Models';
import { ProcessTimestamps, PrintTimestamp } from '../Domain/TimestampProcessor';
import ProcessedTimestampTable from './ProcessedTimestampTable';

interface IProps<T> {
  response: BaseResult<T>;
}

export default function RequestHandler<T>(props: React.PropsWithChildren<IProps<T>>) {
  const { response } = props;

  return (
    <div>
      {props.children}
      <p>HTTP Būsenos kodas: {response.statusCode}</p>
      <p>Užklausos ID: {response.requestId}</p>
      <div>
        Užklausos žymos:
        <ProcessedTimestampTable timestamps={response.timestamps} />
      </div>
    </div>
  );
}
