using AutoMapper;
using DTO.Allergen;
using DTO.Food;
using DTO.FoodCategory;
using DTO.Order;
using Microsoft.Extensions.Options;
using Models;

namespace GrillPizzeriaOrderMiddleware
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {

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

            // Order
            CreateMap<Order, OrderReadDto>()
                .ForMember(destination => destination.Username,
                            options => options.MapFrom(
                                source => source.User.Username
                                )
                            )
                .ForMember(destination => destination.FoodNames,
                            options => options.MapFrom(
                                source => source.OrderFoods.Select(af => af.Food.Name).ToList()
                                )
                            );

            CreateMap<OrderCreateDto, Order>()
                .ForMember(destination => destination.OrderFoods, options => options.Ignore())
                .ForMember(destination => destination.OrderDate, 
                            options => options.MapFrom(
                                _ => DateTime.UtcNow
                                )
                            );
        }
    }
}
