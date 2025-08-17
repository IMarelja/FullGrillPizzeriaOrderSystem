namespace ModelConstants
{
    public static class ValidationConstants
    {
        // [Log]
        public const int LevelLogMaxLenght = 20;
        public const int MessageLogMaxLenght = 255;

        // [User] etc.
        public const int NameMaxLength = 100; // Name (general), Username, First name, Last name... Other Models and Tables use it
        public const int DescriptionMaxLength = 1000;
        public const int ImagePathMaxLength = 255;
        public const int EmailMaxLength = 255;
        public const int PasswordHashMaxLength = 255;
        public const int PhoneMaxLength = 40;
        public const int PasswordPlainMinLenght = 6;
        public const int PasswordPlainMaxLenght = 100;
        
        // [Role]
        public const int RoleNameMaxLength = 20;

        // Price for [Food] and [Order]'s [OrderTotalPrice]
        public const int PriceDecimalInteger = 10;
        public const int PriceDecimalFraction = 2;

        // [Order] order lenght
        public const int ItemsRequestMin = 1;

        // [OrderFood]
        public const int QuantityOfFoodMin = 1;
        public const int QuantityOfFoodMax = 100;
    }

}
