-- ========================
-- TABLE: FoodCategory
-- ========================
CREATE TABLE FoodCategory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- ========================
-- TABLE: Food
-- ========================
CREATE TABLE Food (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(1000) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImagePath NVARCHAR(255),
    FoodCategoryId INT NOT NULL,
    FOREIGN KEY (FoodCategoryId) REFERENCES FoodCategory(Id)
);

-- ========================
-- TABLE: Allergen
-- ========================
CREATE TABLE Allergen (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- ========================
-- TABLE: FoodAllergen (M:N)
-- ========================
CREATE TABLE FoodAllergen (
    FoodId INT NOT NULL,
    AllergenId INT NOT NULL,
    PRIMARY KEY (FoodId, AllergenId),
    FOREIGN KEY (FoodId) REFERENCES Food(Id) ON DELETE CASCADE,
    FOREIGN KEY (AllergenId) REFERENCES Allergen(Id)  ON DELETE CASCADE
);

-- ========================
-- TABLE: [Roles]
-- ========================

CREATE TABLE [Role] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(50) NOT NULL -- e.g. "admin", "user"
);

-- ========================
-- TABLE: [User]
-- ========================
CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(50) NOT NULL -- e.g. "admin", "user"
);

-- ========================
-- TABLE: [Order]
-- ========================
CREATE TABLE [Order] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

-- ========================
-- TABLE: OrderFood (M:N)
-- ========================
CREATE TABLE OrderFood (
    OrderId INT NOT NULL,
    FoodId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    PRIMARY KEY (OrderId, FoodId),
    FOREIGN KEY (OrderId) REFERENCES [Order](Id) ON DELETE CASCADE,
    FOREIGN KEY (FoodId) REFERENCES Food(Id) ON DELETE CASCADE
);

-- ========================
-- OPTIONAL: Sample Seed Data
-- ========================

-- Insert categories
INSERT INTO FoodCategory (Name) VALUES ('Pizza'), ('Grill'), ('Burger');

-- Insert allergens
INSERT INTO Allergen (Name) VALUES ('Gluten'), ('Lactose'), ('Nuts');

-- Insert users
INSERT INTO [User] (Username, PasswordHash, Email, FirstName, LastName, Phone, Role)
VALUES ('admin', 'hashed_password', 'admin@example.com', 'Admin', 'User', '123456789', 'admin');
