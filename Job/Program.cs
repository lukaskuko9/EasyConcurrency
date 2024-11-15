using Job;

var builder = Host.CreateApplicationBuilder(args);

Core.DiConfig.RegisterDi(builder.Services, builder.Configuration);
Infrastructure.Database.DiConfig.RegisterDatabase(builder.Services, builder.Configuration);
Infrastructure.ExternalRefundService.DiConfig.RegisterExternalRefundServiceClient(builder.Services, builder.Configuration);
DiConfig.RegisterJobDi(builder.Services, builder.Configuration);

builder.Services.AddHostedService<ConcurrentWorker>();

var host = builder.Build();
host.Run();