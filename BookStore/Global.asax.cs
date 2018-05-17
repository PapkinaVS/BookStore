using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BookStore.Models;
using Microsoft.ApplicationInsights;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BookStore
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
            CloudStorageAccount account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference("appstart");
            container.CreateIfNotExists();

            CloudBlockBlob blob = container.GetBlockBlobReference($"appstartinfo_{DateTime.UtcNow}.txt");
            blob.UploadText($"{DateTime.UtcNow} {HttpContext.Current.Server.MachineName}");

            AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config")));

             TelemetryClient telemetry = new TelemetryClient();
            telemetry.TrackEvent("App Start");
    }
	}
}
