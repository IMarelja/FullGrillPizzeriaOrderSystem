use GrillPizzeriaDB


-- ================================================
-- Seed FoodCategory (names only)
-- ================================================
INSERT INTO dbo.FoodCategory (Name) VALUES
(N'Pizza'),
(N'Grill'),
(N'Drinks');

-- ================================================
-- Seed Allergen (names only)
-- ================================================
INSERT INTO dbo.Allergen (Name) VALUES
(N'Eggs'),
(N'Peanuts'),
(N'Mustard');

SET XACT_ABORT ON;
BEGIN TRAN;

-- ================================================
-- Fetch Category IDs by Name
-- ================================================
DECLARE @CatPizza  INT = (SELECT Id FROM dbo.FoodCategory WHERE Name = N'Pizza');
DECLARE @CatGrill  INT = (SELECT Id FROM dbo.FoodCategory WHERE Name = N'Grill');
DECLARE @CatDrinks INT = (SELECT Id FROM dbo.FoodCategory WHERE Name = N'Drinks');

-- ================================================
-- Fetch Allergen IDs by Name
-- ================================================
DECLARE @AlgEggs    INT = (SELECT Id FROM dbo.Allergen WHERE Name = N'Eggs');
DECLARE @AlgPeanuts INT = (SELECT Id FROM dbo.Allergen WHERE Name = N'Peanuts');
DECLARE @AlgMustard INT = (SELECT Id FROM dbo.Allergen WHERE Name = N'Mustard');

-- ================================================
-- Prepare 6 foods (bind to categories by fetched IDs)
-- ================================================
DECLARE @Foods TABLE (
    Name NVARCHAR(100),
    Description NVARCHAR(1000),
    Price DECIMAL(10,2),
    ImagePath NVARCHAR(255),
    FoodCategoryId INT
);

INSERT INTO @Foods (Name, Description, Price, ImagePath, FoodCategoryId)
VALUES
(N'Margherita Pizza', N'Classic pizza with tomato, mozzarella and basil.', 7.50, NULL, @CatPizza),
(N'Pepperoni Pizza', N'Pizza with spicy pepperoni slices and cheese.',    8.90, NULL, @CatPizza),
(N'Grilled Steak',    N'Tender beef steak grilled to perfection.',        14.50, NULL, @CatGrill),
(N'BBQ Ribs',         N'Slow-cooked ribs with smoky BBQ sauce.',          13.20, NULL, @CatGrill),
(N'Lemonade',         N'Freshly squeezed homemade lemonade.',              3.20, NULL, @CatDrinks),
(N'Peanut Smoothie',  N'Smoothie made with peanuts and banana.',           4.80, NULL, @CatDrinks);

-- Insert into dbo.Food and capture (Name, Id) mapping
DECLARE @InsertedFoods TABLE (Name NVARCHAR(100), Id INT);

INSERT INTO dbo.Food (Name, Description, Price, ImagePath, FoodCategoryId)
OUTPUT INSERTED.Name, INSERTED.Id INTO @InsertedFoods (Name, Id)
SELECT Name, Description, Price, ImagePath, FoodCategoryId
FROM @Foods;

-- ================================================
-- Link foods to allergens by resolving IDs via names
-- ================================================
-- Margherita Pizza -> Eggs
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgEggs
FROM @InsertedFoods f
WHERE f.Name = N'Margherita Pizza';

-- Pepperoni Pizza -> Eggs
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgEggs
FROM @InsertedFoods f
WHERE f.Name = N'Pepperoni Pizza';

-- Grilled Steak -> Mustard
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgMustard
FROM @InsertedFoods f
WHERE f.Name = N'Grilled Steak';

-- BBQ Ribs -> Mustard
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgMustard
FROM @InsertedFoods f
WHERE f.Name = N'BBQ Ribs';

-- Lemonade -> no allergens

-- Peanut Smoothie -> Peanuts
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgPeanuts
FROM @InsertedFoods f
WHERE f.Name = N'Peanut Smoothie';

COMMIT TRAN;



