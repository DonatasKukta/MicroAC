Feature: Orders

Scenario: Get Orders 
	Given GET request
	When request is sent
	Then response status code is 200

Scenario: Get Order
	Given GET request
		And Guid in path
	When request is sent
	Then response status code is 200

Scenario: Create Order
	Given POST request
		And Guid in path
	When request is sent
	Then response status code is 201
		And response contains order with items

Scenario: Delete Order
	Given DELETE request
		And Guid in path
	When request is sent
	Then response status code is 200

Scenario: Submit Shipment Details 
	Given PUT request
		And Guid in path
		And Shipment in path
		And Shipment Details in body
	When request is sent
	Then response status code is 201
		And response contains Shipment Details

Scenario: Submit Payment Details 
	Given PUT request
		And Guid in path
		And Payment in path
		And Payment Details in body
	When request is sent
	Then response status code is 201
		And response contains Payment Details

Scenario: Submit Order
	Given PUT request
		And Guid in path
		And Payment Details in body
	When request is sent
	Then response status code is 200
