using System.Web.Http;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using Microsoft.Bot.Connector;
using System.Reflection;
using Autofac.Integration.WebApi;
using VisionSample.Api;
using System.Configuration;
using System.Net.Http;
using System;

namespace VisonSample
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        const string VisionEndpointKey = "VisionEndpoint";
        const string VisionAccessKeyKey = "VisionAccessKey";

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Conversation.UpdateContainer(
            builder =>
            {
                builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: [https://github.com/Microsoft/BotBuilder-Azure](https://github.com/Microsoft/BotBuilder-Azure)
                var store = new InMemoryDataStore();

                // Other storage options
                // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
                // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

                builder.Register(c => new HttpClientHandler())
                    .As<HttpMessageHandler>()
                    .SingleInstance();

                builder.Register(c =>
                    {
                        var endpoint = ConfigurationManager.AppSettings[VisionEndpointKey] ?? Environment.GetEnvironmentVariable(VisionEndpointKey, EnvironmentVariableTarget.Process);
                        var accessKey = ConfigurationManager.AppSettings[VisionAccessKeyKey] ?? Environment.GetEnvironmentVariable(VisionAccessKeyKey, EnvironmentVariableTarget.Process);
                        return new AzureVisionApi(endpoint, accessKey, c.Resolve<HttpMessageHandler>());
                    })
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });

            // Register Autofac as the dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);
        }
    }
}
