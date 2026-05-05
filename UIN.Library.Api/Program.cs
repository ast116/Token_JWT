using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace UIN.Library.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ajouter les services
            builder.Services.AddControllers();

            // Ajouter Swagger
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            var key = Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_12345678901234567890");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        ClockSkew = TimeSpan.Zero 

                    };
                });

            var app = builder.Build();

            // 🔹 Configuration pipeline HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // Pour Authentification des requetes

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}