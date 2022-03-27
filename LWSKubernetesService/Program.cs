using LWSKubernetesService.Configuration;
using LWSKubernetesService.Repository;
using LWSKubernetesService.Service;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add MassTransit
var rabbitMqSection = builder.Configuration.GetSection("RabbitMqSection")
    .Get<RabbitMqConfiguration>();
builder.Services.AddMassTransit(a =>
{
    a.AddConsumer<AccountCreatedConsumer>();
    a.AddConsumer<AccountDeletedConsumer>();
    a.AddConsumer<DeploymentCreatedConsumer>();
    
    a.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqSection.Host, rabbitMqSection.VirtualHost, h =>
        {
            h.Username(rabbitMqSection.UserName);
            h.Password(rabbitMqSection.Password);
        });
        
        cfg.ReceiveEndpoint("account.created:KubernetesConsumer", endpointConfig =>
        {
            endpointConfig.Bind("account.created");
            endpointConfig.ConfigureConsumer<AccountCreatedConsumer>(ctx);
        });
        
        cfg.ReceiveEndpoint("account.deleted:KubernetesConsumer", endpointConfig =>
        {
            endpointConfig.Bind("account.deleted");
            endpointConfig.ConfigureConsumer<AccountDeletedConsumer>(ctx);
        });
        
        cfg.ReceiveEndpoint("deployment.created:KubernetesConsumer", endpointConfig =>
        {
            endpointConfig.Bind("deployment.created");
            endpointConfig.ConfigureConsumer<DeploymentCreatedConsumer>(ctx);
        });
    });
});

// Add Configuration
var mongoSection = builder.Configuration.GetSection("MongoSection").Get<MongoConfiguration>();
builder.Services.AddSingleton(mongoSection);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Singleton(Mostly Data Logic)
builder.Services.AddSingleton<IKubernetesRepository, KubernetesRepository>();
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<IDeploymentRepository, DeploymentRepository>();
builder.Services.AddSingleton<KubernetesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();