Feature: Sprint2 feature

@mytag2
Scenario: Launch CABI website
	Given user is on https://cabi.org
	When navigate menu Careers
	Then Jobs with us page should be loaded