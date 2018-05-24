// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace VisonSample
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;
    using VisionSample.Api;
    using VisonSample.Utilities;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Conversation.UpdateContainer(
            builder =>
            {
                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
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

                builder.RegisterType<HttpClient>()
                    .AsSelf()
                    .Keyed<HttpClient>(FiberModule.Key_DoNotSerialize)
                    .SingleInstance();

                // Replace resolution strategy for MicrosoftAppCredentials
                builder.RegisterType<MicrosoftAppCredentialsProvider>()
                    .AsImplementedInterfaces()
                    .Keyed<IMicrosoftAppCredentialsProvider>(FiberModule.Key_DoNotSerialize)
                    .SingleInstance();
                builder.Register(c =>
                {
                    var activity = c.Resolve<IActivity>();
                    return c.Resolve<IMicrosoftAppCredentialsProvider>().GetCredentials(activity.Recipient.Id);
                }).AsSelf();

                builder.Register(c =>
                    {
                        var endpoint = ConfigurationManager.AppSettings["VisionEndpoint"] ?? Environment.GetEnvironmentVariable("VisionEndpoint", EnvironmentVariableTarget.Process);
                        var accessKey = ConfigurationManager.AppSettings["VisionAccessKey"] ?? Environment.GetEnvironmentVariable("VisionAccessKey", EnvironmentVariableTarget.Process);
                        return new AzureVisionApi(endpoint, accessKey, c.Resolve<HttpClient>());
                    })
                    .AsImplementedInterfaces()
                    .Keyed<IVisionApi>(FiberModule.Key_DoNotSerialize)
                    .SingleInstance();

                // Register dialogs
                builder.RegisterType<Dialogs.CaptionDialog>()
                    .AsSelf()
                    .InstancePerDependency();
                builder.RegisterType<Dialogs.OcrDialog>()
                    .AsSelf()
                    .InstancePerDependency();
            });

            // Register Autofac as the dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);
        }
    }
}
