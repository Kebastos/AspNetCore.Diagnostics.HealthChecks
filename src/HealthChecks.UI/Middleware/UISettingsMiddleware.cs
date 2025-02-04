using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace HealthChecks.UI.Middleware
{
    internal class UISettingsMiddleware
    {
        private static Settings Settings { get; set; }
        private readonly JsonSerializerSettings _jsonSerializationSettings;
        private readonly Lazy<dynamic> _uiOutputSettings = new(GetUIOutputSettings);

        public UISettingsMiddleware(RequestDelegate next, IOptions<Settings> settings)
        {
            _ = next;
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            Settings = settings.Value;

            _jsonSerializationSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string content = JsonConvert.SerializeObject(_uiOutputSettings.Value, _jsonSerializationSettings);
            context.Response.ContentType = Keys.DEFAULT_RESPONSE_CONTENT_TYPE;

            await context.Response.WriteAsync(content);
        }

        private static dynamic GetUIOutputSettings()
        {
            return new
            {
                PollingInterval = Settings.EvaluationTimeInSeconds,
                HeaderText = Settings.HeaderText
            };
        }
    }
}
