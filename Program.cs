using Microsoft.AspNetCore.Mvc;
using Nacos.V2;
using Nacos.V2.DependencyInjection;
using Nacos.V2.Naming.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// config
builder.Services.AddNacosV2Config(x =>
{
    x.ServerAddresses = new List<string> { "http://localhost:8848/" };
    x.Namespace = "cs-test";

    // swich to use http or rpc
    x.ConfigUseRpc = true;
    // x.UserName = "nacos";
    // x.Password = "nacos";
});

// naming
builder.Services.AddNacosV2Naming(x =>
{
    x.ServerAddresses = new List<string> { "http://localhost:8848/" };
    x.Namespace = "cs-test";

    // swich to use http or rpc
    x.NamingUseRpc = true;
    // x.UserName = "nacos";
    // x.Password = "nacos";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/service/", async ([FromBody] Instance instance, INacosNamingService namingService) =>
{
    // var instance = new Instance
    // {
    //     InstanceId = instance.InstanceId,
    //     ip = "127.0.0.1",
    //     port = 5000,
    //     ServiceName = serviceName,

    // }
    await namingService.RegisterInstance(instance.ServiceName, "DEFAULT_GROUP", instance);
    return Results.Ok();
})
.WithTags("Services")
.WithName("RegisterSvc");

app.MapDelete("/api/service/{serviceName}", async (string serviceName, [FromBody] Instance instance, INacosNamingService namingService) =>
{
    await namingService.DeregisterInstance(serviceName, "DEFAULT_GROUP", instance.Ip, instance.Port);
    return Results.Ok();
})
.WithTags("Services")
.WithName("DeleteSvc");

app.MapGet("/api/service/{serviceName}", async (string serviceName, INacosNamingService namingService) =>
{
    var instances = await namingService.GetAllInstances(serviceName);
    return Results.Ok(instances);
})
.WithTags("Services")
.WithName("GetAllInstances")
.Produces<List<Instance>>(StatusCodes.Status200OK);

app.Run();


// new Dictionary<string, string>
// {
// { "version", "1.0.0" },
// { "env", "prod" }
// });