﻿:SETVAR NameType NVARCHAR(100) -- Name (general), Username, First name, Last name
:SETVAR DescriptionType NVARCHAR(1000)
:SETVAR ImagePathType NVARCHAR(255)
:SETVAR EmailType NVARCHAR(255)
:SETVAR PasswordHashType NVARCHAR(255)
:SETVAR PhoneType NVARCHAR(40)
:SETVAR RoleNameType NVARCHAR(20)
:SETVAR PriceType DECIMAL(10,2)

-- ========================
-- TABLE: Allergen
-- ========================
CREATE TABLE [dbo].[Allergen] (
    [Id]   INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
	
    CONSTRAINT [PrimaryKey_Allergen] PRIMARY KEY CLUSTERED ([Id])
);
GO

-- ========================
-- TABLE: FoodCategory
-- ========================
CREATE TABLE [dbo].[FoodCategory] (
    [Id]   INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
	
    CONSTRAINT [PrimaryKey_FoodCategory] PRIMARY KEY CLUSTERED ([Id])
);
GO

-- ========================
-- TABLE: Food
-- ========================
CREATE TABLE [dbo].[Food] (
    [Id]             INT IDENTITY(1,1) NOT NULL,
    [Name]           NVARCHAR(100) NOT NULL,
    [Description]    NVARCHAR(1000) NOT NULL,
    [Price]          DECIMAL(10,2) NOT NULL,
    [ImagePath]      NVARCHAR(255) NULL,
    [FoodCategoryId] INT NOT NULL,
	
    CONSTRAINT [PrimaryKey_Food] PRIMARY KEY CLUSTERED ([Id]),
    
	CONSTRAINT [ForeignKey_Food_Category] FOREIGN KEY ([FoodCategoryId]) REFERENCES [dbo].[FoodCategory]([Id])
);
GO

-- ========================
-- TABLE: FoodAllergen (M:N)
-- ========================
CREATE TABLE [dbo].[FoodAllergen] (
    [FoodId]     INT NOT NULL,
    [AllergenId] INT NOT NULL,
	
    CONSTRAINT [PrimaryKey_FoodAllergen] PRIMARY KEY CLUSTERED ([FoodId], [AllergenId]),
    
	CONSTRAINT [ForeignKey_FoodAllergen_Food] FOREIGN KEY ([FoodId]) REFERENCES [dbo].[Food]([Id]) ON DELETE CASCADE,
    CONSTRAINT [ForeignKey_FoodAllergen_Allergen] FOREIGN KEY ([AllergenId]) REFERENCES [dbo].[Allergen]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[Log] (
    [Id]        INT IDENTITY(1,1) NOT NULL,
    [Timestamp] DATETIME NOT NULL DEFAULT GETDATE(),
    [Level]     NVARCHAR(20) NOT NULL,
    [Message]   NVARCHAR(255) NOT NULL,
    
	CONSTRAINT [PrimaryKey_Log] PRIMARY KEY CLUSTERED ([Id])
);

-- ========================
-- TABLE: Role
-- ========================
CREATE TABLE [dbo].[Role] (
    [Id]   INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
	
    CONSTRAINT [PrimaryKey_Role] PRIMARY KEY CLUSTERED ([Id])
);
GO

-- ========================
-- TABLE: User
-- ========================
CREATE TABLE [dbo].[User] (
    [Id]           INT IDENTITY(1,1) NOT NULL,
    [Username]     NVARCHAR(100) NOT NULL,
    [PasswordHash] VARCHAR(255) NOT NULL,
    [Email]        NVARCHAR(255) NOT NULL,
    [FirstName]    NVARCHAR(100) NOT NULL,
    [LastName]     NVARCHAR(100) NOT NULL,
    [Phone]        NVARCHAR(40) NOT NULL,
    [RoleID]       INT NOT NULL,
	
    CONSTRAINT [PrimaryKey_User] PRIMARY KEY CLUSTERED ([Id]),
    
	CONSTRAINT [ForeignKey_User_Role] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role]([Id])
);
GO

-- ========================
-- TABLE: [Order]
-- ========================
CREATE TABLE [dbo].[Order] (
    [Id]        INT IDENTITY(1,1) NOT NULL,
    [UserId]    INT NOT NULL,
    [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
	
    CONSTRAINT [PrimaryKey_Order] PRIMARY KEY CLUSTERED ([Id]),
    
	CONSTRAINT [ForeignKey_Order_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id])
);

-- ========================
-- TABLE: OrderFood (M:N)
-- ========================
CREATE TABLE [dbo].[OrderFood] (
    [OrderId]  INT NOT NULL,
    [FoodId]   INT NOT NULL,
    [Quantity] INT NOT NULL CHECK ([Quantity] > 0),
	
    CONSTRAINT [PrimaryKey_OrderFood] PRIMARY KEY CLUSTERED ([OrderId], [FoodId]),
    
	CONSTRAINT [ForeignKey_OrderFood_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order]([Id]) ON DELETE CASCADE,
    CONSTRAINT [ForeignKey_OrderFood_Food] FOREIGN KEY ([FoodId]) REFERENCES [dbo].[Food]([Id]) ON DELETE CASCADE
);

-- Insert Role
INSERT INTO [dbo].[Role] ([Name])
VALUES ('user'), ('admin');
GO

-- Insert first admin user (admin)
INSERT INTO [dbo].[User] (
    [Username], [PasswordHash], [Email], [FirstName], [LastName], [Phone], [RoleID]
)
VALUES 
(
'admin', 
'd033e22ae348aeb5660fc2140aec35850c4da997', /*SHA1 encrypted password: admin*/
'admin@example.com', 
'Andy', 
'Andincen', 
'+38599777865', 
(SELECT [Id] FROM [dbo].[Role] WHERE [Name] = 'admin')
); 
GO

-- ========================================
-- Indexes
-- ========================================

CREATE UNIQUE INDEX [Unique_Role_Name] 
    ON [dbo].[Role]([Name]);
GO
CREATE UNIQUE INDEX [Unique_User_Username] 
    ON [dbo].[User]([Username]);
GO
CREATE UNIQUE INDEX [Unique_User_Email] 
    ON [dbo].[User]([Email]);
GO
CREATE UNIQUE INDEX [Unique_FoodCategory_Name] 
    ON [dbo].[FoodCategory]([Name]);
GO
CREATE UNIQUE INDEX [Unique_Food_Name] 
    ON [dbo].[Food]([Name]);
GO
CREATE UNIQUE INDEX [Unique_Allergen_Name] 
    ON [dbo].[Allergen]([Name]);
GO

-- ========================================
-- Constraints
-- ========================================
ALTER TABLE [User]
	ADD CONSTRAINT Default_User_RoleId DEFAULT 1 FOR RoleId;
GO
