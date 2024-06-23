-- Step 1: Connect to the 'shoper' database
USE [shoper (1)];

-- Step 2: Create the 'Cart' table
CREATE TABLE Cart
(
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity >= 1),
    
    CONSTRAINT FK_Cart_Customer FOREIGN KEY (CustomerID)
    REFERENCES Customer(CustomerID),
    
    CONSTRAINT FK_Cart_Product FOREIGN KEY (ProductID)
    REFERENCES Product(ProductID)
);
