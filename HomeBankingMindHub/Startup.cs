using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomeBankingMindHub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //inyectamos las dependencias (AddControllers, AddRazorPages, AddScoped, AddJsonOption, AddDbContext )
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(); // permite utilizar las paginas razor (C# + html) en en la app (Vistas)
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve); //permite que los controladores respondan a las peticiones http
            services.AddDbContext<HomeBankingContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("HomeBankingConexion"))); //agregamos el contexto de la base de datos
            services.AddScoped<IClientRepository, ClientRepository>(); //agregamos el repositorio
            services.AddScoped<IAccountRepository, AccountRepository>(); //agregamos el repositorio
            //autenticacion cuando el navegador envia una peticion para acceder a algun recurso protegido del navegador web
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            { options.ExpireTimeSpan = TimeSpan.FromMinutes(10);  // Tiempo de expiracion de la cookie 
            
            options.LoginPath = new PathString("/index.html");}); // Ruta de redireccion 
            //autorización, indica que puede hacer el usuario o con que recursos puede interactuar (permisos)
            services.AddAuthorization(options => { options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles(); // cuando se hace una request, responde utilizando lo que esta en el wwwrooot 

            app.UseRouting(); // determina que el endpoint debe responder a la request 
            
            app.UseAuthentication();//le decimos que use autenticación

            app.UseAuthorization(); //restrinje el acceso de acuerdo a los roles del usuario y autentificacion

            app.UseEndpoints(endpoints =>
            {
             //configurando las rutas finales para manejar las solicitudes de páginas Razor y solicitudes a controladores en una aplicación ASP.NET Core. 
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
