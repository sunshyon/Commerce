using Base.JWT;
using Base.ThirdTool;
using Base.WebExtend.Filters;
using Domain.DbModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Panda.DynamicWebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAndUserMicroservice
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
            //services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthAndUserMicroservice", Version = "v1" });
                // 动态api展示Swagger 要有,不然可以调用,但是不展示
                c.DocInclusionPredicate((docName, description) => true);
            });
            // 注入动态api，还要引入Service项目
            services.AddDynamicWebApi();

            #region JWT HS256 对称加密
            //生成token服务
            services.Configure<JwtOptions>(this.Configuration.GetSection("JwtOptions"));
            services.AddScoped<IJwtService, JwtHS256Service>();
            //验证token服务
            JwtOptions jwtOptions = new JwtOptions();
            Configuration.Bind("JwtOptions", jwtOptions);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                        ValidateIssuer = false,//是否验证Issuer
                        ValidateAudience = false,//是否验证Audience
                        ValidateLifetime = false,//是否验证失效时间
                        ValidateIssuerSigningKey = false,//是否验证SecurityKey
                        ValidAudience = jwtOptions.Audience,//
                        ValidIssuer = jwtOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),//拿到SecurityKey
                    };
                });
            //授权策略
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("AdminPolicy",
                    policyBuilder => policyBuilder.RequireUserName("sun"));
            });
            #endregion

            #region ThirdTool
            services.AddScoped<RedisClient>();
            //services.AddScoped<ElasticSearchService>();
            services.AddScoped<RabbitMQClient>();
            #endregion

            #region EF+Mysql
            //services.AddDbContext<OrangeContext>(opts =>
            //{
            //    opts.UseMySql(Configuration.GetConnectionString("Mysql00"), Microsoft.EntityFrameworkCore.ServerVersion.FromString("5.7.32-mysql"));
            //});
            services.AddTransient<OrangeContext>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAndUserMicroservice v1"));
            }

            app.UseRouting();

            app.UseCors("Default");//使用跨域

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
