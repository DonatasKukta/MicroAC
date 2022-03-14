Feature: Orders

Scenario: Get
	Given GET request
	When request is sent
	Then response status code is 200
