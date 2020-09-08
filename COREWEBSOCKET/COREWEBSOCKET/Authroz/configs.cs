using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COREWEBSOCKET.Authroz
{
    public class configs
    {
        ///存放链接人
        public static Dictionary<string, WebSocket> webline = new Dictionary<string, WebSocket>();
        #region 总方法
        public async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var s = context.Request.Query["Id"];
            webline.Add(s, webSocket);
   
            #region 消息接受
            //消息缓冲
            var buffer = new byte[1024 * 4];
            //等待消息接受
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            #endregion
            #region [redis消息缓存]
            // redishelper redishelper = new redishelper();
            //  redishelper.StringGet(RoomId);
            //string sp = redishelper.StringGet("4");
            //string[] value = sp.Split(",");
            var msgString = Encoding.UTF8.GetString(buffer);
            dynamic p = JsonConvert.DeserializeObject(msgString);
                //处理消息
            
            #endregion
            #region 处理消息发送
     
            while (!result.CloseStatus.HasValue)
            {
                

                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        #endregion

        #endregion
    }
}
