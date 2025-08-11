# BankSim – Banking Simulation Project

##  Overview
**BankSim** is a .NET Core–based banking simulation project with a modular architecture.  
It supports core banking features such as account management, transactions, currency conversion, and invoice payments.  
The solution also includes a **separate Invoice.API** service for managing and paying utility bills.

---

##  Project Structure
BankSim.sln
-
+¦¦ BankSim.API           # Main banking API (accounts, transactions, authentication)

+¦¦ BankSim.Application   # Business logic and services

+¦¦ BankSim.Domain        # Entities, enums, interfaces

+¦¦ BankSim.Infrastructure# Data access layer (EF Core, repositories)

+¦¦ BankSim.Ui            # MVC-based frontend

+¦¦ Invoice.API           # Independent API for invoice management

L¦¦ BankSim.Tests         # Unit tests (xUnit + Moq + InMemoryDb)

---

##  Features
### **BankSim.API**
-  JWT Authentication – Secure login and token-based access
-  Account Management – Create, view, and manage accounts
-  Transactions – Money transfers between accounts
-  Currency Conversion – Automatic FX rate handling for multi-currency transfers
-  Email Notifications – Automatic notifications after transfers

### **Invoice.API**
-  Invoice Listing – Retrieve invoices for a customer
-  Invoice Payment – Pay bills using linked bank accounts (BankSim API)
-  Validation – Prevents duplicate payments, validates TCKN, and checks due dates

---

## Testing
- **xUnit + Moq** for unit testing
- **EF Core InMemory** for database tests
- **Transaction service tests**:
  - Currency conversion usage
  - Insufficient balance
  - Negative/zero amounts
  - Account not found
- **Invoice API tests**:
  - Missing bearer token
  - Invoice not found
  - TCKN mismatch
  - Already paid invoices

Run tests:
dotnet test

---

## How to Run
1. Clone the repository:
   git clone https://github.com/emirkagantas/Banksim.git
   cd Banksim

2. Set up the database:
   dotnet ef database update --project BankSim.Infrastructure

3. Run the APIs:
   dotnet run --project BankSim.API
   dotnet run --project Invoice.API

4. Run the UI:
   dotnet run --project BankSim.Ui

5. Open in browser:  
   - BankSim API Swagger: https://localhost:7291/swagger  
   - Invoice API Swagger: https://localhost:7035/swagger  
   - UI: https://localhost:7284

---


Developed by **Emir Kagan Tas**  

