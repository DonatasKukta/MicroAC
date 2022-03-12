Feature: Carts

Scenario: Create Cart
	Given POST request
	When request is sent
	Then response status code is 200
		And the response contains a cart

Scenario: Get Cart
	Given GET request
		And cart id in url
	When request is sent
	Then response status code is 200
		And the response contains a cart
		And a list of cart items

Scenario: Add Cart Item
	Given POST request
		And cart id in url
		And new cart item
	When request is sent
	Then response status code is 200
	
Scenario: Delete Cart
	Given DELETE request
		And cart id in url
	When request is sent
	Then response status code is 200
	
Scenario: Delete Cart Item
	Given DELETE request
		And cart id in url
		And with product id
	When request is sent
	Then response status code is 200
