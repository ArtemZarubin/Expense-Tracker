-- Database creation
CREATE DATABASE PersonalExpenses;
GO

-- Using the created database
USE PersonalExpenses;
GO

-- Create Expenses table
CREATE TABLE Expenses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Category NVARCHAR(255) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Date DATE NOT NULL,
    Description NVARCHAR(MAX)
);
GO

-- Creating the Categories table
CREATE TABLE Categories
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);
GO

-- Inserting initial data into the Categories table
INSERT INTO Categories(Name)
VALUES 
  ('Clothing'),
  ('Debt Repayment'),
  ('Dining Out'),
  ('Education'),
  ('Entertainment'),
  ('Gifts & Donations'),
  ('Groceries'),
  ('Health'),
  ('Household Supplies'),
  ('Insurance'),
  ('Investments'),
  ('Other'),
  ('Personal Care'),
  ('Rent'),
  ('Savings'),
  ('Subscriptions'),
  ('Transportation'),
  ('Travel'),
  ('Utilities');
