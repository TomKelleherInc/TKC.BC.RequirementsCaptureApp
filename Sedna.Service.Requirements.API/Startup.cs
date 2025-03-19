using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sedna.Service.Requirements.API.Data;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using Sedna.Service.Requirements.API.Middleware;

namespace Sedna.Service.Requirements.API
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
            services.AddCors(); // Make sure you call this previous to AddMvc 

            string connString = Configuration.GetConnectionString("DatabaseConnectionString");

            services.AddDbContext<RequirementsDbContext>(options =>
                options.UseNpgsql(connString)
                    //.UseLazyLoadingProxies()
                );

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );


            services.AddControllers();
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sedna.Service.Requirements", Version = "v1" });
            });

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddAutoMapper(typeof(Startup));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // These are custom middleware to log requests and ensure that we have proper HttpHeaders for non-GET operations.

            app.UseLogger();
            app.UseHttpHeaderChecker();

            app.UseSwagger();



            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseCors(
                options => options.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
            ) ;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
