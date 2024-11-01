
using AutoMapper;
using DAPM.Authenticator.Data;
using DAPM.Authenticator.Interfaces.Repostory_Interfaces;
using DAPM.Authenticator.Models;
using DAPM.Authenticator.Repositories;
using DAPM.Authenticator.Services;
using DAPM.Authenticator.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace DAPM.Authenticator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration configuration = builder.Configuration;

            // Add services to the container.
            var connectionstring = configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring)));

            builder.Services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;

            })
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddEntityFrameworkStores<DataContext>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = configuration.GetSection("JWTTokenKey").Value;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding
                            .UTF8.GetBytes(configuration.GetSection("JWTTokenKey").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            builder.Services.AddScoped<TokenService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();


            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            var services = app.Services.CreateScope().ServiceProvider;
            try {
                var datacontext = services.GetRequiredService<DataContext>();
                await datacontext.Database.MigrateAsync();
                var usermanager = services.GetRequiredService<UserManager<User>>();
                var rolemanager = services.GetRequiredService<RoleManager<Role>>();

                //create some beginner roles incase they dont exist
                List<string> listOfRoles = new List<string>() { "Standard", "Admin", "Privileged" };
                foreach (var role in listOfRoles)
                {
                    await rolemanager.CreateAsync(new Role { Name = role });
                }

                //Admin user

                User user = new User
                {
                    FullName = "Admin Adminson",
                    UserName = "Admin"
                };

                var registerUserResult = await usermanager.CreateAsync(user, "verysafepassworD123123");
                var registerRoleResult = await usermanager.AddToRolesAsync(user, new List<string>() { "Standard", "Admin", "Privileged" });

            }
            catch (Exception ex)
            {
                Log.Error($"Error during migration {ex}");
            }

            app.Run();
        }
    }
}
