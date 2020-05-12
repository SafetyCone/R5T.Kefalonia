using System;

using Microsoft.Extensions.DependencyInjection;


namespace R5T.Kefalonia.Common
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualStudioProjectFileDeserializationSettings(this IServiceCollection services,
            Action<VisualStudioProjectFileDeserializationSettings> configureAction)
        {
            services.AddSingleton<IVisualStudioProjectFileDeserializationSettings, VisualStudioProjectFileDeserializationSettings>(serviceProvider =>
            {
                var settings = new VisualStudioProjectFileDeserializationSettings();

                configureAction(settings);

                return settings;
            });

            return services;
        }
    }
}
