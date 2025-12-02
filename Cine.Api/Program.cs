using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cine;

namespace Cine.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<DbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DbContext.postgresql") ?? throw new InvalidOperationException("Connection string 'DbContext.postgresql' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Configure JSON options
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ReferenceLoopHandling
                            = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
