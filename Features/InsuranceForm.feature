Feature: Insurance Form Automation

  Scenario: Fill and submit the insurance form successfully
    Given the browser is launched
    When the user fills the vehicle data form
    And the user fills the insurant data form
    And the user fills the product data form
    And the user selects a price option
    And the user fills and submits the quote
    Then the form should be submitted successfully

  Scenario Outline: Show errors for invalid data in vehicle data form
    Given the user is on the "<vehicleType>" vehicle data page
    When the user enters data like "<enginePerformance>" , "<dateOfManufacture>" , "<listPrice>" ,"<licensePlateNo>" , "<annualMileage>"
    Then the form should show errors in the respective fields

    Examples:
     | vehicleType | enginePerformance | dateOfManufacture | listPrice | licensePlateNo | annualMileage | 
     | Automobile  | 2500  | 09/21/2025 |  20000000 | TN 37 BE 5567 | 23 |

  
  Scenario Outline: Show errors for invalid data in insurant data form
    Given the user is on the "<vehicleType>" insurant data page 
    When the user enter the personal data like "<firstName>" , "<lastName>" , "<dob>" , "<zip>" , "website"
    Then the form should show errors in the fields

    Examples:
      | vehicleType | firstName | lastName | dob        | zip       | website |
      | Truck       | Arun12    | AK2      | 05/09/2012 | 222222222 | .com    |
      | Motorcycle  | Prakash37 | I0       | 05/09/1950 | Hello     | abcd    |
  
   
   Scenario: User views the generated quote in a new tab
    Given the browser is launched
    When the user fills data forms and selects premimum
    And the user clicks the view quote link
    Then the quote should open in a new browser tab
  
   
   Scenario: Prevent progression without selecting a price option
    Given the browser is launched
    When the user fills all forms up to price option
    And tries to proceed without selecting a price option
    Then the form should display a message to select a price option 
  
 
  Scenario: User skips required steps and tries to access the Price Option page
    Given the browser is launched
    When the user directly navigates to the select price option step
    Then the application should show a message indicating incomplete steps
 
  


