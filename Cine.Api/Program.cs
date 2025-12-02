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

            // IMPORTANTE: Aquí deberías usar tu propia clase, ej: <CineContext>
            // Si tu clase se llama realmente 'DbContext', asegúrate de que no sea la de Microsoft.
            builder.Services.AddDbContext<DbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DbContext.postgresql")
                ?? throw new InvalidOperationException("Connection string 'DbContext.postgresql' not found.")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                        options.SerializerSettings.ReferenceLoopHandling
                            = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            var app = builder.Build();

            // ---------------------------------------------------------
            // BLOQUE PARA APLICAR MIGRACIONES AUTOMÁTICAS EN RENDER
            // ---------------------------------------------------------
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Obtiene tu DbContext
                    // NOTA: Cambia <DbContext> por el nombre real de tu clase heredada (ej. <CineContext>)
                    var context = services.GetRequiredService<DbContext>();

                    // Aplica cualquier migración pendiente automáticamente
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error al migrar la base de datos.");
                }
            }
            // ---------------------------------------------------------

            //if(app.Environment.IsDevelopment()) // Comentado para ver Swagger en Render si lo deseas
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
