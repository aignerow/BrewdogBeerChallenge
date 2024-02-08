Feature: Brewdog beer validation
  As a user
  I want to validate beers brewed after December 2015
  So that I can ensure returned data meets expected standards

	Scenario: Returned beers have correct abv format and value
		When I have list of beers brewed after 12-2015
		Then beers abv value is a double over 4.0

	Scenario: Returned beers have correct name value
		When I have list of beers brewed after 12-2015
		Then beers names are a non empty strings

	Scenario: Returned beers have ingredients listed
		When I have list of beers brewed after 12-2015
		Then beers ingredients are listed

	Scenario: Returned beers have image url in correct format
		When I have list of beers brewed after 12-2015
		Then beers image has correct url format
	

	