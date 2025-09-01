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
-- Prepare foods (bind to categories by fetched IDs)
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
(N'Peanut Smoothie',  N'Smoothie made with peanuts and banana.',           4.80, NULL, @CatDrinks),
(N'Four Cheese Pizza',N'A blend of mozzarella, gorgonzola, parmesan and cheddar.', 9.20, NULL, @CatPizza),
(N'Veggie Pizza',     N'Peppers, onions, olives, mushrooms and sweetcorn.',        8.60, NULL, @CatPizza),
(N'Hawaiian Pizza',   N'Tomato, mozzarella, ham and pineapple.',                   9.00, NULL, @CatPizza),
(N'Mushroom Pizza',   N'Tomato, mozzarella and button mushrooms.',                 8.40, NULL, @CatPizza),
(N'Capricciosa Pizza',N'Tomato, mozzarella, ham, mushrooms and artichokes.',       9.30, NULL, @CatPizza),
(N'Grilled Chicken Breast', N'Seasoned chicken breast grilled and served with salad.', 11.50, NULL, @CatGrill),
(N'Grilled Vegetables Plate',N'Seasonal grilled veggies with olive oil.',              7.90,  NULL, @CatGrill),
(N'Grilled Salmon',   N'Fillet of salmon with lemon and herbs.',                   15.80, NULL, @CatGrill),
(N'Mineral Water',    N'Sparkling mineral water, 0.5L.',                           2.20,  NULL, @CatDrinks),
(N'Iced Tea',         N'House-brewed iced black tea, lightly sweetened.',          2.80,  NULL, @CatDrinks),
(N'Carbonara Pizza',  N'Creamy sauce, pancetta and egg.',                          9.80,  NULL, @CatPizza),
(N'Caesar Salad',     N'Romaine, croutons, parmesan with egg-based dressing.',     7.90,  NULL, @CatGrill),
(N'Honey Mustard Chicken', N'Grilled chicken with honey-mustard glaze.',           12.40, NULL, @CatGrill),
(N'BBQ Wings with Mustard Dip', N'Crispy wings served with mustard dip.',          9.60,  NULL, @CatGrill),
(N'Peanut Brownie Shake', N'Chocolate brownie shake blended with peanuts.',        5.20,  NULL, @CatDrinks);

-- ================================================
-- Insert into dbo.Food and capture (Name, Id) mapping
-- ================================================
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

-- Peanut Smoothie -> Peanuts
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgPeanuts
FROM @InsertedFoods f
WHERE f.Name = N'Peanut Smoothie';

-- Carbonara Pizza -> Eggs
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgEggs
FROM @InsertedFoods f
WHERE f.Name = N'Carbonara Pizza';

-- Caesar Salad -> Eggs
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgEggs
FROM @InsertedFoods f
WHERE f.Name = N'Caesar Salad';

-- Honey Mustard Chicken -> Mustard
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgMustard
FROM @InsertedFoods f
WHERE f.Name = N'Honey Mustard Chicken';

-- BBQ Wings with Mustard Dip -> Mustard
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgMustard
FROM @InsertedFoods f
WHERE f.Name = N'BBQ Wings with Mustard Dip';

-- Peanut Brownie Shake -> Peanuts
INSERT INTO dbo.FoodAllergen (FoodId, AllergenId)
SELECT f.Id, @AlgPeanuts
FROM @InsertedFoods f
WHERE f.Name = N'Peanut Brownie Shake';

COMMIT TRAN;
