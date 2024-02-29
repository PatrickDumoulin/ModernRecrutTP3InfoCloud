using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();

		services.AddSingleton(x =>
		{
			var configuration = x.GetService<IConfiguration>();
			string connectionString = configuration.GetConnectionString("BlobConnectionString");
			return new BlobServiceClient(connectionString);
		});


	})
	.Build();

host.Run();
