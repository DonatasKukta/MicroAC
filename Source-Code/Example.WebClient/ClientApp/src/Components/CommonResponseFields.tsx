import { BaseResult } from '../Domain/Models';
import { ProcessTimestamps, PrintTimestamp } from '../Domain/TimestampProcessor';

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
        {ProcessTimestamps(response.timestamps).map((t, i) => (
          <div key={i + t.origin.date.toString()}>{PrintTimestamp(t)}</div>
        ))}
      </div>
    </div>
  );
}
