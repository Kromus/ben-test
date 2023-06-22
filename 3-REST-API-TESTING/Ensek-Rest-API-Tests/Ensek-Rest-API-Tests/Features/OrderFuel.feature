Feature: Order Energy
As a user,
I want to be able to order energy,
So that I can redistribute it to my customers

@OrderFuelSuccessfully
Scenario Outline: Order energy successfully
	When I order <energyName> <energyId> <quantity>
	Then the request is successful
	And the order is stored
	Examples: 
	| energyName | energyId | quantity |
	| gas        | 1        | 1        |
	| nuclear    | 2        | 1        |
	| Electric   | 3        | 1        |
	| Oil        | 4        | 1        |

@OrderFuelSuccessfully
Scenario Outline: Order energy validation
	When I order <energyName> <energyId> <quantity>
	Then the request is unsuccessful
	Examples: 
	| energyName | energyId | quantity |
	| gas        | 0        | 1        |
	| gas        | 1        | 0        |
	| gas        | 5        | 1        |