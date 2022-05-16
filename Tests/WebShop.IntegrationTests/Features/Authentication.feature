Feature: Authentication

Scenario: Get external access token
	Given POST request
		And login path
		And test user credentials
	When request is sent
	Then response status code is 200
		And response contains access and refresh tokens
		And access token is valid
		And refresh token is valid

Scenario: Refresh external access token
	Given POST request
		And refresh path
		And refresh token
	When request is sent
	Then response status code is 200
		And response contains access token
		And access token is valid