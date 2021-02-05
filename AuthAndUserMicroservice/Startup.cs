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
            //�������
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
                // ��̬apiչʾSwagger Ҫ��,��Ȼ���Ե���,���ǲ�չʾ
                c.DocInclusionPredicate((docName, description) => true);
            });
            // ע�붯̬api����Ҫ����Service��Ŀ
            services.AddDynamicWebApi();

            #region JWT HS256 �ԳƼ���
            //����token����
            services.Configure<JwtOptions>(this.Configuration.GetSection("JwtOptions"));
            services.AddScoped<IJwtService, JwtHS256Service>();
            //��֤token����
            JwtOptions jwtOptions = new JwtOptions();
            Configuration.Bind("JwtOptions", jwtOptions);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
                        ValidateIssuer = false,//�Ƿ���֤Issuer
                        ValidateAudience = false,//�Ƿ���֤Audience
                        ValidateLifetime = false,//�Ƿ���֤ʧЧʱ��
                        ValidateIssuerSigningKey = false,//�Ƿ���֤SecurityKey
                        ValidAudience = jwtOptions.Audience,//
                        ValidIssuer = jwtOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),//�õ�SecurityKey
                    };
                });
            //��Ȩ����
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

            app.UseCors("Default");//ʹ�ÿ���

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
