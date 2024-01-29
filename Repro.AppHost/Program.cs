var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.Repro_ApiService>("apiservice");

builder.AddProject<Projects.Repro_Web>("webfrontend")
    .WithReference(apiservice);

builder.Build().Run();
