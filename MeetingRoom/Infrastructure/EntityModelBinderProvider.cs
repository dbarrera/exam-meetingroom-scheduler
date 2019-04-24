using MeetingRoom.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MeetingRoom.Infrastructure
{
    public class EntityModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return typeof(IEntity).IsAssignableFrom(context.Metadata.ModelType) ? new EntityModelBinder() : null;
        }
    }
}
