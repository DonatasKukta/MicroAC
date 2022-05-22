import { Service, Action } from './WebShopRequestCreator';
import { IProps as WebShopHandlerVariant } from '../Containers/WebShopHandler';

const productVariants: WebShopHandlerVariant[] = [
  {
    title: 'Gauti prekes',
    service: Service.Products,
    action: Action.GetList,
    accessJwt: undefined
  },
  {
    title: 'Gauti prekę',
    service: Service.Products,
    action: Action.GetOne,
    accessJwt: undefined
  },
  {
    title: 'Sukurti prekę',
    service: Service.Products,
    action: Action.Create,
    accessJwt: undefined
  },
  {
    title: 'Atnaujinti prekę',
    service: Service.Products,
    action: Action.Update,
    accessJwt: undefined
  },
  {
    title: 'Ištrinti prekę',
    service: Service.Products,
    action: Action.Delete,
    accessJwt: undefined
  }
];

const cartVariants: WebShopHandlerVariant[] = [
  {
    title: 'Sukurti krepšelį',
    service: Service.Cart,
    action: Action.Create,
    accessJwt: undefined
  },
  {
    title: 'Gauti krepšelį',
    service: Service.Cart,
    action: Action.GetOne,
    accessJwt: undefined
  },
  {
    title: 'Pridėti prekę į krepšelį',
    service: Service.Cart,
    action: Action.AddCartItem,
    accessJwt: undefined
  },
  {
    title: 'Ištrinti krepšelį',
    service: Service.Cart,
    action: Action.Delete,
    accessJwt: undefined
  },
  {
    title: 'Pašalinti prekę iš krepšelio',
    service: Service.Cart,
    action: Action.DeleteCartItem,
    accessJwt: undefined
  }
];

const shipmentVariants: WebShopHandlerVariant[] = [
  {
    title: 'Gauti siuntas',
    service: Service.Shipments,
    action: Action.GetList,
    accessJwt: undefined
  },
  {
    title: 'Gauti siuntą',
    service: Service.Shipments,
    action: Action.GetOne,
    accessJwt: undefined
  },
  {
    title: 'Sukurti siuntą',
    service: Service.Shipments,
    action: Action.Create,
    accessJwt: undefined
  },
  {
    title: 'Atnaujinti siuntą',
    service: Service.Shipments,
    action: Action.Update,
    accessJwt: undefined
  },
  {
    title: 'Ištrinti siuntą',
    service: Service.Shipments,
    action: Action.Delete,
    accessJwt: undefined
  }
];

const orderVariants: WebShopHandlerVariant[] = [
  {
    title: 'Gauti užsakymą',
    service: Service.Orders,
    action: Action.GetOne,
    accessJwt: undefined
  },
  {
    title: 'Gauti užsakymus',
    service: Service.Orders,
    action: Action.GetList,
    accessJwt: undefined
  },
  {
    title: 'Sukurti užsakymą',
    service: Service.Orders,
    action: Action.Create,
    accessJwt: undefined
  },
  {
    title: 'Ištrinti užsakymą',
    service: Service.Orders,
    action: Action.Delete,
    accessJwt: undefined
  },
  {
    title: 'Pateikti siuntos duomenis',
    service: Service.Orders,
    action: Action.SubmitShipment,
    accessJwt: undefined
  },
  {
    title: 'Pateikti mokėjimo duomenis siuntą',
    service: Service.Orders,
    action: Action.SubmitPayment,
    accessJwt: undefined
  },
  {
    title: 'Pateikti užsakymą siuntą',
    service: Service.Orders,
    action: Action.Update,
    accessJwt: undefined
  }
];

const allVariants: WebShopHandlerVariant[] = productVariants.concat(
  cartVariants,
  shipmentVariants,
  orderVariants
);

export default allVariants;
