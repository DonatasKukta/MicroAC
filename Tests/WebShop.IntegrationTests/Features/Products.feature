Feature: Products

Scenario: Get Products
	Given GET request
	When request is sent
	Then response status code is 200
		And the response contains a list of products

Scenario: Get Product
	Given GET request
		And product Id
	When request is sent
	Then response status code is 200
		And the response contains a product

Scenario: Create Product
	Given POST request
		And product
	When request is sent
	Then response status code is 201
		And the response contains a product

Scenario: Update Product
	Given PUT request
		And product
	When request is sent
	Then response status code is 200

Scenario: Delete Product
	Given DELETE request
		And product Id
	When request is sent
	Then response status code is 200