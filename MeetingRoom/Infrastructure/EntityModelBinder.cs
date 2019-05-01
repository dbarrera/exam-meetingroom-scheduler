using MeetingRoom.Data;
using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace MeetingRoom.Infrastructure
{
    public class EntityModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var original = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (original != ValueProviderResult.None)
            {
                int id;
                var originalValue = original.FirstValue;

                if (int.TryParse(originalValue, out id))
                {
                    IEntity entity = null;
                    var dbContext = bindingContext.HttpContext.RequestServices.GetService<ExamContext>();

                    if (bindingContext.ModelType == typeof(Room))
                    {
                        entity = await dbContext.Set<Room>().FindAsync(id);
                    }
                    else if (bindingContext.ModelType == typeof(RoomAttribute))
                    {
                        entity = await dbContext.Set<RoomAttribute>().FindAsync(id);
                    }
                    else if (bindingContext.ModelType == typeof(Food))
                    {
                        entity = await dbContext.Set<Food>().FindAsync(id);
                    }
                    else if (bindingContext.ModelType == typeof(Schedule))
                    {
                        entity = await dbContext.Set<Schedule>().FindAsync(id);
                    }

                    bindingContext.Result = entity != null ? ModelBindingResult.Success(entity) : bindingContext.Result;
                }
            }
        }
    }
}
