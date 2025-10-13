# CSV Processor

## Overview
CSV Processor is a .NET-based system for processing CSV files asynchronously using a message queue architecture. It allows users to upload CSV file paths, tracks the processing progress, and stores CSV rows in a relational database. The system uses a web dashboard for user interaction and a background service for CSV processing.

---

## **Features**

- User Registration (Signup) with minimal details  
- Login for authenticated users  
- Dashboard listing processed files with status: **In Progress, Completed, Error**  
- Start CSV file processing by entering a network file path  
- Background worker consumes messages from RabbitMQ and processes CSV files  


---

## **Technical Stack**

- **Framework:** .NET 7 (can work with .NET 6 or above)  
- **UI:** Razor Pages  
- **Database:** SQL Server (configurable via EF Core)  
- **ORM:** Entity Framework Core  
- **Message Queue:** RabbitMQ  
- **Dependency Injection:** Unity Container  
 

---




