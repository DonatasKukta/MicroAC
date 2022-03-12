import { Timestamp } from '../Domain/Models';
import { ProcessTimestamps } from '../Domain/TimestampProcessor';

interface IProps {
  timestamps: Timestamp[];
}

export default function ProcessedTimestampTable(props: IProps) {
  return (
    <table>
      <thead>
        <tr>
          <th>Paslauga</th>
          <th>Veiksmas</th>
          <th>Bendras Laikas</th>
          <th>Trukmė</th>
          <th>Bendra</th>
        </tr>
      </thead>
      <tbody>
        {ProcessTimestamps(props.timestamps).map((t, i) => (
          <tr key={t.origin.date.getTime() + i} style={{ textAlign: 'right' }}>
            <td>{t.origin.serviceName}</td>
            <td>{t.origin.message}</td>
            <td>{t.origin.date.toISOString()}</td>
            <td>{t.msFromPrevTimestamp} ms</td>
            <td>{t.accumulatingMsSum} ms</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
