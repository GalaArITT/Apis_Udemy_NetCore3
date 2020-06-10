using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MiPrimerWebApiM3.Contexts;
using MiPrimerWebApiM3.Controllers;
using MiPrimerWebApiM3.Entities;
using MiPrimerWebApiM3.Helpers;
using MiPrimerWebApiM3.Models;
using MiPrimerWebApiM3.Services;

[assembly:ApiConventionType(typeof(DefaultApiConventions))]

namespace MiPrimerWebApiM3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<MiFiltroDeAccion>();
            //habilidad servicio para filtros en caché
            services.AddResponseCaching();

            //configurar automapper
            services.AddAutoMapper(configuration => 
            {
                configuration.CreateMap<Autor, AutorDTO>();
                configuration.CreateMap<AutorCreacionDTO, Autor>().ReverseMap();
            },
                typeof(Startup));

            //agregar autenticacion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
          
            //para referencias circulares 
            services.AddControllers()
               .AddNewtonsoftJson(options =>
               options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            //instalar newtonsoftjson 

            //nuevos servicios Transient
            services.AddTransient<ClaseB>();
      

            services.AddMvc(options =>
            {
                //options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                options.Filters.Add(new MiFiltroDeExcepcion());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);



            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<HATEOASAuthorFilterAttribute>();
            services.AddScoped<HATEOASAuthorsFilterAttribute>();
            services.AddScoped<GeneradorEnlaces>();

            //configurando swagger 

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Version = "V1",
                    Title = "Mi Web API",
                    Description = "Esta es una descripción del Web API",
                    TermsOfService = new Uri("https://www.udemy.com/user/felipegaviln/"),
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("http://bfy.tw/4nqh")
                    },
                    Contact = new OpenApiContact()
                    {
                        Name = "Oliver Galaviz",
                        Email = "oliver.jga@gmail.com",
                        Url = new Uri("https://gavilan.blog/")
                    }
                });
                config.SwaggerDoc("v2", new OpenApiInfo { Title = "Mi Web API", Version = "V2" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });

            //servicio Scoped 
            //services.AddScoped<IClaseB, ClaseB>();
            //services.AddScoped<AutoresController>();

            //singleton
            //services.AddSingleton<IClaseB, ClaseB>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //usando swagger y configurando

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi Api V1");
                config.SwaggerEndpoint("/swagger/v2/swagger.json", "Mi Api V2");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCaching();


            app.UseRouting();
            //app.UseAuthentication(); //cambio por authorization
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
        {
            public void Apply(ControllerModel controller)
            {
                // Ejemplo: "Controllers.V1"
                var controllerNamespace = controller.ControllerType.Namespace;
                var apiVersion = controllerNamespace.Split('.').Last().ToLower();
                controller.ApiExplorer.GroupName = apiVersion;
            }
        }
    }
}
