Feature: ResetData
As a engineer,
I want to be able to reset test data,
so that I can consistently test the API

@ResetTestData
Scenario: Reset test data
	Given I have an existing order
	When I reset data
	Then I can no longer find that order

@Unauthorised
Scenario: Can't reset test data if not authorised
	Given I have an existing order
	But I am not authorised
	When I reset data
	Then resetting data was unsuccessful
