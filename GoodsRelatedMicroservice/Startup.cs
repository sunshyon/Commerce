using Base.ThirdTool;
using Base.WebExtend.Filters;
using BL.GoodsRelated;
using Domain.DbModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Panda.DynamicWebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsRelatedMicroservice
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
            //跨域服务
            services.AddCors(opt => opt.AddPolicy("Default", o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            services.AddControllers(o =>
            {
                o.Filters.Add(typeof(ExceptionFilter));
                o.Filters.Add(typeof(LogActionFilter));
                //o.Filters.Add(typeof(ActionFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoodsRelatedMicroservice", Version = "v1" });
                // 动态api展示Swagger 要有,不然可以调用,但是不展示
                c.DocInclusionPredicate((docName, description) => true);
            });
            // 注入动态api，还要引入Service项目
            services.AddDynamicWebApi();

            #region ThirdTool
            services.AddScoped<RedisClient>();
            services.AddScoped<ElasticSearchClient>();
            services.AddScoped<RabbitMQClient>();
            #endregion

            #region EF+Mysql
            //services.AddDbContext<OrangeContext>(opts =>
            //{
            //    opts.UseMySql(Configuration.GetConnectionString("Mysql00"), Microsoft.EntityFrameworkCore.ServerVersion.FromString("5.7.32-mysql"));
            //});
            services.AddTransient<OrangeContext>();
            #endregion

            services.AddTransient<GoodsService>();
            services.AddTransient<SpecService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoodsRelatedMicroservice v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
