using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BookStore.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using DataType = Microsoft.Azure.Search.Models.DataType;

namespace BookStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private SearchServiceClient _serviceClient;

        protected void Application_Start()
        {
            CloudStorageAccount account =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference("appstart");
            container.CreateIfNotExists();

            CloudBlockBlob blob = container.GetBlockBlobReference($"appstartinfo_{DateTime.UtcNow}.txt");
            blob.UploadText($"{DateTime.UtcNow} {HttpContext.Current.Server.MachineName}");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.ConfigureAndWatch(
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config")));

            TelemetryClient telemetry = new TelemetryClient();
            telemetry.TrackEvent("Event #1: App Start");

            _serviceClient = new SearchServiceClient("bookstore", new SearchCredentials("2BDBEA23B682B7C4EE0784C74FE839A9"));

            var bookFields = new List<Field> {
                new Field("Id", DataType.String) { IsKey = true },
                new Field("Name", DataType.String) { IsSearchable = true },
                new Field("Author", DataType.String) { IsSearchable = true },
                new Field("Price", DataType.String) { IsSearchable = true },
                //new Field("Person", DataType.String) {IsSearchable = true},
                //new Field("Address", DataType.String) {IsSearchable = true}
            };

            var bookIndex = _serviceClient.Indexes.CreateOrUpdate(new Index("book", bookFields));
            CreateIndexer("Books", bookIndex);
            //CreateIndexer("Purchases", bookIndex);

            var soccerFields = new List<Field>
            {
                new Field("Id", DataType.String) { IsKey = true },
                new Field("Name", DataType.String) { IsSearchable = true },
                new Field("Age", DataType.String) { IsSearchable = true },
                new Field("Position", DataType.String) { IsSearchable = true },
                //new Field("Coach", DataType.String) { IsSearchable = true },
            };

            var soccerIndex = _serviceClient.Indexes.CreateOrUpdate(new Index("soccer", soccerFields));
            CreateIndexer("Players", soccerIndex);
            //CreateIndexer("Teams", soccerIndex);

            var studentsFields = new List<Field>
            {
                new Field("Id", DataType.String) {IsKey = true},
                new Field("Name", DataType.String) {IsSearchable = true},
                new Field("Surname", DataType.String) {IsSearchable = true}
            };

            var studentsIndex = _serviceClient.Indexes.CreateOrUpdate(new Index("students", studentsFields));
            CreateIndexer("Students", studentsIndex);
            //CreateIndexer("Courses", studentsIndex);
        }

        private void CreateIndexer(string table, Index index)
        {
            var datasource = _serviceClient.DataSources.CreateOrUpdate(DataSource.AzureSql($"{table.ToLower()}storage",
                ConfigurationManager.ConnectionStrings["BookContext"].ConnectionString,
                table));

            _serviceClient.Indexers.CreateOrUpdate(new Indexer($"{table.ToLower()}indexer", datasource.Name, index.Name, null, new IndexingSchedule(new TimeSpan(0, 5, 0))));
        }
    }
}
