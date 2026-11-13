using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Microsoft.AspNetCore.Identity;
using Dominio.Entities;

namespace infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
           
           //repositorios
            services.AddScoped<IUsuarioRepository, UsuarioRepositorio>();
            services.AddScoped<IObraRepository, ObraRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            //SERVIÇOS
            services.AddScoped<UsuarioService>();
            services.AddScoped<ObraService>();
            services.AddScoped<ClienteService>();
           
           //login autenticação jwt
            services.AddIdentity<Usuarios, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
           
            return services;
        }
    }
}