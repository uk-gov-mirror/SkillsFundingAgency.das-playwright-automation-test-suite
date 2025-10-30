Feature: MS_05_UpdateContactInfo

@managingstandards
@regression
Scenario: MS_05_UpdateContactInfo
	Given the provider logs into portal
	Then the provider is able to update contact details
	And the provider is able to update phone number only
	And the provider is able to update email only
	And the provider is able to update contact details to all the standards
	And the updated contact details are visible in the standard information section
	
