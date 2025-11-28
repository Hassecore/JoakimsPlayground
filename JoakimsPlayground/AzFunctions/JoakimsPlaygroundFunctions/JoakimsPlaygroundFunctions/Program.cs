using Azure.Messaging.ServiceBus;
using JoakimsPlaygroundFunctions.Business.Classes;
using JoakimsPlaygroundFunctions.Business.Interfaces;
using JoakimsPlaygroundFunctions.Data.Contexts;
using JoakimsPlaygroundFunctions.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;






//var connectionString = "Endpoint=sb://joakimsserverbusnamespace13.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=sYl+EO/OORI5z0ZlcsDX/6gXzyx6hD/qR+ASbFC2NIo=";

//var client = new ServiceBusClient(connectionString);
//var admin = client.CreateSender("myqueue");


//try
//{
//	await admin.SendMessageAsync(new ServiceBusMessage("Test connection"));
//	Console.WriteLine("✅ Connection successful, message sent!");
//}
//catch (Exception ex)
//{
//	Console.WriteLine($"❌ Connection failed: {ex.Message}");
//}

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices((context,services) =>
    {
        // You can add scoped services here.
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<ILetterManager, LetterManager>();

		// EF CORE
		var connectionString = context.Configuration.GetConnectionString("SqlDbConnectionString");
        services.AddDbContext<Context>(options =>
            options.UseSqlServer(connectionString));
		
	})
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    dbContext.Database.SetCommandTimeout(180);
    dbContext.Database.Migrate();
}

host.Run();
