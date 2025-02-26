# Slot Machine API 

# Overview

- SlotMachineAPI is a .NET Core Web API that simulates a slot machine game. It is built using CQRS with MediatR and follows Clean Architecture principles. The API features:
- A Spin Method that generates a random slot machine result and calculates winnings based on predefined rules.
- An Update Balance Method to add funds to a player's balance.
- MongoDB as the database for storing player balances and game configurations.
- Configurable slot matrix size without requiring an application restart.
- Concurrent support for spin and balance updates.
- Unit tests to ensure reliable functionality.

# Technologies Used

- .NET Core Web API
- CQRS 
- MediatR
- MongoDB
- Clean Architecture Principles
- Unit Testing (xUnit)

# Summary 
- Registration & Login: 
o Admin/Employer must register using the register method. 
o Only Admin users can access all endpoints. 
o After registration, a token is generated, which must be entered in the 
Bearer authorization header. 
o Login is performed using the Login endpoint.

- Player Operations: 
o GET → List all players. 
o GET by ID → Retrieve a player by ID. 
o CREATE → Add a new player. 
o DELETE → Remove a player. 
o UPDATE-BALANCE → Modify a player’s balance (Use - for balance 
deduction). 

- Spin Method: 
o Select a Player ID to start spinning. 
o Enter a bet amount, and the system returns a 5x3 matrix. 
o Every new player starts with a 100 bonus balance. 
o The system calculates winAmount and currentBalance dynamically. 
o If a player tries to bet more than their balance, they receive an error 
message. 
o If the balance reaches zero, the game stops and an error message is 
displayed. 

- Key Points: 
o The system is designed for real-time operations. 
o All methods follow specific business rules to ensure proper functionality. 
o Admin users have full access to all features. 
�
� With this setup, all endpoint functionalities can be tested efficiently!



