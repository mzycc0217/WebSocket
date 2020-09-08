using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COREWEBSOCKET.Model;
using COREWEBSOCKET.redis;
using COREWEBSOCKET.sqlBase;
using COREWEBSOCKET.websocket;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace COREWEBSOCKET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //private IHttpContextAccessor _accessor;
        //public WeatherForecastController(IHttpContextAccessor accessor)
        //{
        //    _accessor = accessor;
        //}
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Userid"></param>
        /// <param name="Roomsid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Post(string Userid,string Roomsid)
        {

          //给个人发送数据
            var msg = new { type = "waring", contntsd = "12121212" };
            //var msg = "111";
            var msjson = msg.ToString();
            var bufferd = new byte[10 * 1024];
            bufferd = Encoding.Default.GetBytes(msjson);
            // var p = _accessor.HttpContext;
            //// p.WebSockets
            //// HttpContext httpContext = new HttpContext();
            // //将TCP链接升级到websocket链接
            // WebSocket webSocket = await p.WebSockets.AcceptWebSocketAsync();
            // //这里可以DI注入持久化
            //  websoketservices websoketservices = new websoketservices();
         await   websoketservices.webline["s"].SendAsync(new ArraySegment<byte>(bufferd, 0, bufferd.Length),WebSocketMessageType.Text,true,CancellationToken.None); ;
            //  await  websoketservices.Echo(p, webSocket);
          //  var usid = Guid.NewGuid().ToString();
          //  var s = "1";
          
            //加入组
          //  var ps = "sssss";
            //   byte[] vs= Encoding.Default.GetBytes(ps);
           // websoketservices.AddGroup(Userid, Roomsid);
            //websoketservices.AddGroup("111", "1987");
            //websoketservices.AddGroup("123", "1987");
            //websoketservices.AddGroup("124", "1987");

            //websoketservices.AddGroup("888", "1999");
            //websoketservices.AddGroup("999", "1999");


            //  websoketservices.websocketLinesd.Add(usid,s);
            // var q = db.Queryable<Base_Action>().ToList();

            //  return q;
            // return Userid+"加入了"+Roomsid;

            //new TestUser
            //{
            //    SubjectId = "1",
            //    Username = "alice",
            //    Password = "password",
            //    Claims = new List<Claim>() { new Claim(JwtClaimTypes.Role, "superadmin") }
            //};
            //new TestUser
            //{
            //    SubjectId = "2",
            //    Username = "bob",
            //    Password = "password",
            //    Claims = new List<Claim>() { new Claim(JwtClaimTypes.Role, "admin") }
            //};

            // redishelper redishelper = new redishelper();
            //redishelper.StringAppend(Roomsid, Userid+",");


            // string s=  redishelper.StringGet(Roomsid);
            // string[] value = s.Split(",");
            // return value;




        }
        public static  string connectionString = "Server=.;Database=nxzjfx;Integrated Security=False;User ID=sa;Password=123456;";
         SqlSugarClient db = new SqlSugarClient(
              new ConnectionConfig
              {
                  ConnectionString = connectionString,
                  DbType = DbType.SqlServer,
                  IsShardSameThread = true
              }
          );
    }

}
