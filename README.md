# Slot Machine API 

# Overview

- SlotMachineAPI is a .NET Core Web API that simulates a slot machine game. It is built using CQRS with MediatR and follows Clean Architecture principles. The API features:
- A Spin Method that generates a random slot machine result and calculates winnings based on predefined rules.
- An Update Balance Method to add funds to a player's balance.
- MongoDB as the database for storing player balances and game configurations.
- Configurable slot matrix size without requiring an application restart.
- Concurrent support for spin and balance updates.
- Unit tests to ensure reliable functionality.

#  Installation & Setup Guide
This section provides step-by-step instructions on how to set up the Slot Machine API locally, including required dependencies, environment setup, and database configuration.
Before running the project, ensure that you have the following installed:
- Visual Studio 2022+
- .NET 9 SDK
- MongoDB Compass 


# Technologies Used

- .NET Core Web API
- CQRS 
- MediatR
- MongoDB
- Fluent Validaton
- Authentication and Jwt
- Serilog
- Middlewares(Global exception, Logging)
- Clean Architecture Principles
- Unit Testing (xUnit)

# Summary 
1. Registration & Login: 
- Admin/Employer must register using the register method. 
- Only Admin users can access all endpoints. 
- After registration, a token is generated, which must be entered in the 
Bearer authorization header. 
- Login is performed using the Login endpoint.

2. Player Operations: 
- GET → List all players. 
- GET by ID → Retrieve a player by ID. 
- CREATE → Add a new player. 
- DELETE → Remove a player. 
- UPDATE-BALANCE → Modify a player’s balance (Use - for balance 
deduction). 

3. Spin Method: 
- Select a Player ID to start spinning. 
- Enter a bet amount, and the system returns a 5x3 matrix. 
- Every new player starts with a 100 bonus balance. 
- The system calculates winAmount and currentBalance dynamically. 
- If a player tries to bet more than their balance, they receive an error 
message. 
- If the balance reaches zero, the game stops and an error message is 
displayed. 

4. Key Points: 
- The system is designed for real-time operations. 
- All methods follow specific business rules to ensure proper functionality. 
- Admin users have full access to all features. 
�
� With this setup, all endpoint functionalities can be tested efficiently!



