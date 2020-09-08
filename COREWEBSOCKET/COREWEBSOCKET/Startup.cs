using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using COREWEBSOCKET.Authroz;
using COREWEBSOCKET.Model;
using COREWEBSOCKET.websocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace COREWEBSOCKET
{
    public class Startup
    {
        public websoketservices websoketservices = new websoketservices();
        readonly string ganweiCosr = "AllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 使用内存存储，密钥，客户端和资源来配置身份服务器。
            //services.AddIdentityServer()
            //    .AddTemporarySigningCredential()
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients());

            services.AddCors(options =>
            {
                options.AddPolicy(ganweiCosr,
                builder => builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());
            });
            //      services.AddIdentityServer()//注册服务
            //.AddDeveloperSigningCredential()
            //.AddInMemoryApiResources(Config.GetApiResources())//配置类定义的授权范围
            //.AddInMemoryClients(Config.GetClients())//配置类定义的授权客户端
            //.AddTestUsers(new List<TestUser> { new TestUser { Username = "Admin", Password = "123456", SubjectId = "001", IsActive = true } });//模拟测试用户，这里偷懒了，用户可以单独管理，最好不要直接在这里New
            //      services.AddControllers();
            //  }
            // 
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers();
//
            //将身份验证服务添加到DI并配置Bearer为默认方案。
            //services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.Authority = "http://localhost:5000";
            //        options.RequireHttpsMetadata = false;
            //        options.Audience = "api1";
            //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
          app.UseCors(ganweiCosr);

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        //将TCP链接升级到websocket链接
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                       // await Echo(context, webSocket);
                        await websoketservices.Echo(context, webSocket);
    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });

            app.UseHttpsRedirection();
           
            app.UseRouting();

            //将身份验证中间件添加到管道中，以便对主机的每次调用都将自动执行身份验证。
            app.UseAuthentication();
            //授权中间件，以确保匿名客户端无法访问我们的API端点。
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers().RequireCors(ganweiCosr);
               endpoints.MapControllers();
            });
        }

        ////链接房间
        //public List<WebsocketLine> websocketLines = new List<WebsocketLine>();

        ////链接人
        //public List<webLinsPeople> webline = new List<webLinsPeople>();

        //private async Task Echo(HttpContext context, WebSocket webSocket)
        //{
        //    var buffer = new byte[1024 * 4];
        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    //  var p= JsonConvert.SerializeObject(result);
        //   var s= context.Request.Query["Id"];
        //    this.webline.Add(new webLinsPeople
        //    {
        //        Id=s,
        //        Name="ssd",
        //        webSocket=webSocket
        //    });



        //    while (!result.CloseStatus.HasValue)
        //    {

        //        var msgString = Encoding.UTF8.GetString(buffer);
        //        dynamic p = JsonConvert.SerializeObject(msgString);
        //        string name = p.name;
        //        string Romm = p.Room;
        //        string Id = p.Id;  

        //       // var s = p;
        //        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        //        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    }
        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //}
    }
}
