using AutoMapper;
using DTO.Allergen;
using DTO.Food;
using DTO.FoodCategory;
using DTO.Order;
using DTO.User;
using Microsoft.Extensions.Options;
using Models;

namespace GrillPizzeriaOrderMiddleware
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {

            // User
            CreateMap<User, UserReadDto>()
                .ForMember(destination => destination.RoleName, option => option.MapFrom(source => source.Role.Name));

            CreateMap<UserUpdateDto, User>()
                .ForMember(destination => destination.Id, option => option.Ignore())
                .ForMember(destination => destination.Username, option => option.Ignore())
                .ForMember(destination => destination.PasswordHash, option => option.Ignore())
                .ForMember(destination => destination.RoleId, option => option.Ignore())
                .ForMember(destination => destination.Role, option => option.Ignore())
                .ForMember(destination => destination.CreationDate, option => option.Ignore())
                .ForMember(destination => destination.Orders, option => option.Ignore());

            // Food
            CreateMap<Food, FoodReadDto>()
                .ForMember(destination => destination.FoodCategoryName,
                            options => options.MapFrom(
                                source => source.FoodCategory.Name
                                )
                            )
                .ForMember(destination => destination.AllergenNames, 
                            option => option.MapFrom(
                                source => source.FoodAllergens.Select(fa => fa.Allergen.Name).ToList()
                                )
                            );

            CreateMap<FoodCreateDto, Food>()
                .ForMember(destination => destination.FoodAllergens, options => options.Ignore());

            CreateMap<FoodUpdateDto, Food>()
                .ForMember(destination => destination.FoodAllergens, options => options.Ignore());

            // FoodCategory
            CreateMap<FoodCategory, FoodCategoryReadDto>();
            CreateMap<FoodCategoryCreateDto, FoodCategory>();

            // Allergen
            CreateMap<Allergen, AllergenReadDto>();
            CreateMap<AllergenCreateDto, Allergen>();

            // Read mapping for Order → OrderReadDto
            CreateMap<Order, OrderReadDto>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderFoods))
                .ForMember(d => d.OrderTotal, o => o.MapFrom(s => s.OrderTotalPrice));

            // Read mapping for OrderFood → OrderItemReadDto
            CreateMap<OrderFood, OrderItemReadDto>()
                .ForMember(d => d.FoodId, o => o.MapFrom(s => s.FoodId))
                .ForMember(d => d.FoodName, o => o.MapFrom(s => s.Food.Name))
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.Food.Price))
                .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Quantity * s.Food.Price));

            // Create mapping: OrderItemCreateDto → OrderFood
            CreateMap<OrderItemCreateDto, OrderFood>();

            // Create mapping: OrderCreateDto → Order
            // (We'll set UserId/OrderDate in controller; here we only map items)
            CreateMap<OrderCreateDto, Order>()
                .ForMember(d => d.OrderFoods, o => o.MapFrom(s => s.Items));

        }
    }
}
