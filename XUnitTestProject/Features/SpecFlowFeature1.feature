Feature: Sprint1 feature

@mytag
Scenario: Launch CABI website
	Given user is on https://cabi.org
	When navigate menu Careers
	Then Jobs with us page should be loaded