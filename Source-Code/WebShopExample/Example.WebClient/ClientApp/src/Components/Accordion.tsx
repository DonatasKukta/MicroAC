import { Accordion, AccordionDetails, AccordionSummary } from '@mui/material';

interface IProps<T> {
  title: string;
  main: boolean;
}

export default function CustomAccordion<T>(props: React.PropsWithChildren<IProps<T>>) {
  return (
    <Accordion square={props.main} style={{ maxWidth: props.main ? '100%' : '800px' }}>
      <AccordionSummary>
        {props.main ? <h1>{props.title}</h1> : props.title}
      </AccordionSummary>
      <AccordionDetails>{props.children}</AccordionDetails>
    </Accordion>
  );
}
