import {
  Table,
  Paper,
  TableContainer,
  TableBody,
  TableCell,
  TableHead,
  TableRow
} from '@mui/material';
import { Timestamp } from '../Domain/Models';
import { ProcessTimestamps } from '../Domain/TimestampProcessor';

interface IProps {
  timestamps: Timestamp[];
}

export default function ProcessedTimestampTable(props: IProps) {
  function dateToTableCellString(date: Date) {
    return date.toISOString().replace('T', ' ').replace('Z', '');
  }

  return (
    <TableContainer component={Paper} sx={{ maxWidth: 650 }}>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Paslauga</TableCell>
            <TableCell>Veiksmas</TableCell>
            <TableCell>Laiko žymė</TableCell>
            <TableCell>Trukmė</TableCell>
            <TableCell>Bendra</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {ProcessTimestamps(props.timestamps).map((t, i) => (
            <TableRow key={t.origin.date.getTime() + i} style={{ textAlign: 'right' }}>
              <TableCell>{t.origin.serviceName}</TableCell>
              <TableCell>
                {t.origin.message === 'Start' ? (
                  <b>{t.origin.message}</b>
                ) : (
                  t.origin.message
                )}
              </TableCell>
              <TableCell>{dateToTableCellString(t.origin.date)}</TableCell>
              <TableCell>{t.msFromPrevTimestamp} ms</TableCell>
              <TableCell>{t.accumulatingMsSum} ms</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
