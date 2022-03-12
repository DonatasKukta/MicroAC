import { Timestamp, ProcessedTimestamp } from './Models';

export const PrintTimestamp = (t: ProcessedTimestamp): string =>
  `${t.origin.serviceName}-${t.origin.message}   ${t.minutes}:${t.seconds}.${t.ms}; from prev:${t.msFromPrevTimestamp} sum:${t.accumulatingMsSum}`;

export function ProcessTimestamps(timestamps: Timestamp[]): ProcessedTimestamp[] {
  var msSum = 0;
  var result = timestamps.map((current, i, array) => {
    var prev = i - 1 < 0 ? undefined : array[i - 1];
    var processed = ProcessTimestamp(prev, current, msSum);
    msSum = processed.accumulatingMsSum;
    return processed;
  });
  return result;
}

export function ProcessTimestamp(
  prev: Timestamp | undefined,
  current: Timestamp,
  msSum: number
): ProcessedTimestamp {
  var currentDate = current.date;
  var msFromPrevTimestamp = prev ? GetDiffMs(prev.date, currentDate) : 0;
  return {
    origin: current,
    minutes: currentDate.getMinutes(),
    seconds: currentDate.getSeconds(),
    ms: currentDate.getMilliseconds(),
    msFromPrevTimestamp: msFromPrevTimestamp,
    accumulatingMsSum: msSum + msFromPrevTimestamp
  };
}

export const GetDiffMs = (past: Date, future: Date) => future.getTime() - past.getTime();
