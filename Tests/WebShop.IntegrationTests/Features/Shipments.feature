Feature: Shipments

Scenario: Get Shipments 
	Given GET request
	When request is sent
	Then response status code is 200
		And response contains a list of Shipments

Scenario: Get Shipment
	Given GET request
		And Guid in path
	When request is sent
	Then response status code is 200
		And response contains a Shipment

Scenario: Create Shipment
	Given POST request
		And Guid in path
		And Shipment Details in body
	When request is sent
	Then response status code is 201
		
Scenario: Update Shipment
	Given PUT request
		And Guid in path
		And Shipment in Body
	When request is sent
	Then response status code is 200

Scenario: Delete Shipment
	Given DELETE request
		And Guid in path
	When request is sent
	Then response status code is 200
