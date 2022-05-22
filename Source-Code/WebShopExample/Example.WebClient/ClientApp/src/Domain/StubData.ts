export const stubGuid: string = '9d721d52-7ec1-4f07-981d-219db0bf0e7a';

export const stubPaymentDetails: any = {
  CardNumber: '1234',
  SecurityCode: '1234',
  ExpiryDate: '05/22',
  TotalSum: 300
};
export const stubOrderItem: any = {
  ProductId: 1,
  AddedAt: Date.now(),
  Quantity: 1,
  Price: 10,
  Discount: 0.1
};

export const stubProduct: any = {
  Id: 1,
  Name: 'StubProduct',
  Description: 'Description of Stub product',
  Price: 30,
  Quantity: 5
};

export const stubShipment: any = {
  Id: stubGuid,
  UserId: stubGuid,
  OrderId: stubGuid,
  Country: 'Country',
  AddressLine1: 'AddressLine1',
  AddressLine2: 'AddressLine2',
  City: 'City',
  PostCode: '58963',
  Cost: 16,
  Provider: 'SiuntosLT',
  Type: 'PVP'
};

export const stubOrder: any = {
  Id: stubGuid,
  UserId: stubGuid,
  Products: [stubProduct, stubProduct],
  Payment: stubPaymentDetails,
  Shipment: stubShipment
};

export const stubCartItem: any = {
  ProductId: stubGuid,
  AddedAt: Date.now(),
  Quantity: 5
};

export const stubCart: any = {
  Id: stubGuid,
  UserId: stubGuid,
  Items: [stubCartItem, stubCartItem]
};
