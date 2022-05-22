import { BaseResult, Token } from './Models';
import { parseAnyBody } from './Parsing';
import SendRequest from './SendRequest';
import {
  stubCart,
  stubGuid,
  stubOrder,
  stubPaymentDetails,
  stubProduct,
  stubShipment
} from './StubData';

export enum Service {
  ResourceApi = 'ResourceApi',
  RequestManager = 'RequestManager',
  Authentication = 'Authentication',
  Authorization = 'Authorization ',
  Orders = 'Orders',
  Shipments = 'Shipments',
  Cart = 'Cart',
  Products = 'Products'
}

export enum Action {
  GetOne,
  GetList,
  Create,
  Update,
  Delete,
  SubmitPayment,
  SubmitShipment,
  AddCartItem,
  DeleteCartItem,
  Login,
  Refresh
}

const baseUlr = 'http://localhost:19081/MicroAC.ServiceFabric/MicroAC.RequestManager/';

const getHttpAction = (action: Action) => {
  switch (action) {
    case Action.SubmitPayment:
    case Action.SubmitShipment:
    case Action.Update:
      return 'PUT';
    case Action.DeleteCartItem:
    case Action.Delete:
      return 'DELETE';
    case Action.GetOne:
    case Action.GetList:
      return 'GET';
    case Action.Login:
    case Action.Refresh:
    case Action.AddCartItem:
    case Action.Create:
      return 'POST';
  }
};

const getBody = (service: Service, action: Action) => {
  if (
    action === Action.GetList ||
    action === Action.GetOne ||
    action === Action.Delete ||
    action === Action.DeleteCartItem
  )
    return null;
  if (service === Service.Orders && action === Action.Update) return null;

  switch (action) {
    case Action.AddCartItem:
      return stubCart;
    case Action.SubmitPayment:
      return stubPaymentDetails;
    case Action.SubmitShipment:
      return stubShipment;
  }
  switch (service) {
    case Service.Products:
      return stubProduct;
    case Service.Cart:
      return stubCart;
    case Service.Shipments:
      return stubShipment;
    case Service.Orders:
      return stubOrder;
  }
  return `Unrecognised pattern ${service}, ${action}`;
};

export const getUrl = (service: Service, action: Action) => {
  let url = baseUlr + service + '/';
  if (service == Service.Cart && action == Action.AddCartItem)
    return url + `${stubGuid}/products`;
  if (service == Service.Cart && action == Action.DeleteCartItem)
    return url + `${stubGuid}/products/${stubGuid}`;
  if (service == Service.Orders && action == Action.SubmitPayment)
    return url + `${stubGuid}/payment`;
  if (service == Service.Orders && action == Action.SubmitShipment)
    return url + `${stubGuid}/shipment`;
  if (service == Service.ResourceApi && action == Action.GetOne) return url + `Action`;
  if (service == Service.Authentication && action == Action.Login) return url + `Login`;
  if (service == Service.Authentication && action == Action.Refresh)
    return url + `Refresh`;
  if (
    (service == Service.Orders || service == Service.Shipments) &&
    action == Action.Create
  )
    return url + `${stubGuid}`;
  if (action == Action.Update || action == Action.Delete || action == Action.GetOne)
    return url + `${stubGuid}`;
  if (action == Action.GetList || action == Action.Create) return url;

  return `Unrecognised pattern ${service}, ${action}`;
};

let createRequest = (action: Action, body: any, accessJwt: Token): RequestInit => {
  let request: RequestInit = {
    method: getHttpAction(action),
    headers: {
      'Content-Type': 'application/json',
      Authorization: accessJwt ?? ''
    }
  };
  if (body) request.body = JSON.stringify(body);
  return request;
};

export const SendWebShopRequest = (
  service: Service,
  action: Action,
  accessJwt: Token,
  responseCallback: (result: BaseResult<string>) => void
) => {
  let url = getUrl(service, action);
  let body = getBody(service, action);
  let request = createRequest(action, body, accessJwt);
  SendRequest<string>(url, request, parseAnyBody, responseCallback);
};
