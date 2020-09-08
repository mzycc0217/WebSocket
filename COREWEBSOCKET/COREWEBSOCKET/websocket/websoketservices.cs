using COREWEBSOCKET.Model;
using COREWEBSOCKET.redis;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COREWEBSOCKET.websocket
{
    public class websoketservices
    {
        /// <summary>
        /// 群发单发类型
        /// </summary>
        #region 消息发送类型
        public string Type { get; set; } = "user";
        public WebSocketReceiveResult result { get; set; }
        public WebSocket webs { get; set; }

        #endregion
        #region 优化部分，新建一个model，处理消息
        ///【接受人，发送人，消息类型，消息内容】
        
        ///接受房间
        public string Roomid { get; set; }



        /// <summary>
        /// 发送者
        /// </summary>
        public string SendId { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public string accpet { get; set; }

        #endregion
        #region [可以新建一个容器/类，处理客户端]
        ///存放链接人
        public static Dictionary<string, WebSocket> webline = new Dictionary<string, WebSocket>();
       
        #endregion
        #region 已完成
        //房间

        public static List<WebsocketLine> websocketLinesds = new List<WebsocketLine>();
        /// <summary>
        /// 房间，个人id//加入组
        /// </summary>
       // 暂时没有用
        //public static Dictionary<string, string> websocketLinesd = new Dictionary<string, string>();


        #endregion
        #region 总方法
        public async Task Echo(HttpContext context, WebSocket webSocket)
        {
          
           
           
           await AddWebLine(context,webSocket);
            #region 消息接受
            //消息缓冲
            var buffer = new byte[1024 * 4];
            //等待消息接受
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            #endregion
            #region [redis消息缓存]
            // redishelper redishelper = new redishelper();
            //  redishelper.StringGet(RoomId);
            //string sp = redishelper.StringGet("4");
            //string[] value = sp.Split(",");
            #endregion
            #region 处理消息发送
            webs = webSocket;
            while (!result.CloseStatus.HasValue)
            {
                webs = webSocket;
                //转换消息
                var messangContent = ReceiveMessages(buffer);
                switch (Type)
                {

                    case "Group":
                        await sendMessageAsync(messangContent);
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        break;
                    case "user":
                        await sendMessagetoUserAsync(messangContent);
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        break;
                    case "All":
                        try
                        {
                            await sendAllMessageAsync(messangContent);
                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        break;
                }
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        #endregion

        #endregion
       
        #region 给指定的房间发送消息
        /// <summary>
        /// 给组发送消息
        /// </summary>
        public async Task sendMessageAsync(byte[] buffer3)
        {
          

            foreach (var item in websocketLinesds)
            {
                if (item.Room == this.accpet)
                {   
                    if (item.userid.Contains(SendId))
                    {
                       foreach (var items in item.userid)
                       {
                          WebSocket sValue = null;
                          if (webline.TryGetValue(items, out sValue))
                           {
                           var ws = webline[items];//找到这个人
                           if (ws!=null)
                            {
                           await webline[items].SendAsync(new ArraySegment<byte>(buffer3, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                          }
                           }
                      
                     }
                    }

                    else
                    {
                        //踢下线移除数据库
                        var msg = new { type = "waring", contntsd = "您不是本组成员无法发送消息" };
                        //var msg = "111";
                       var msjson = msg.ToString();
                            var bufferd = new byte[10*1024];
                            bufferd = Encoding.Default.GetBytes(msjson);
                        await webs.SendAsync(new ArraySegment<byte>(bufferd, 0, bufferd.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    
                    
                    }
                }
            }

        }
        #endregion
        #region 广播消息
        /// <summary>
        /// 广博1消息
        /// </summary>
        public async Task sendAllMessageAsync(byte[] buffer1)
        {
            //消息缓冲
            var buffer = new byte[1024 * 4];
            foreach (var item in webline)
            {
                try
                {
                    await webline[item.Key].SendAsync(new ArraySegment<byte>(buffer1, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    // result = await webs.ReceiveAsync(new ArraySegment<byte>(buffer1), CancellationToken.None);
                }
                catch (Exception)
                {
                    throw;
                }

                // 
            }

        }

        #endregion
        #region 给指定的人发送消息
        /// <summary>
        /// 给指定的人发送消息
        /// </summary>

        public async Task sendMessagetoUserAsync(byte[] buffer2)
        {
            #region 单独发送消息

            WebSocket sValued = null;
            try
            {
                if (webline.TryGetValue(this.accpet, out sValued))
                {
                    if (this.accpet!=this.SendId)
                    {
                     await webline[this.accpet].SendAsync(new ArraySegment<byte>(buffer2, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                     await webline[this.SendId].SendAsync(new ArraySegment<byte>(buffer2, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                 
                    // result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                }
                else
                {
                    //踢下线移除数据库
                    var msg = new { type = "waring", contntsd = "对方已经下线" };
                    //var msg = "111";
                    var msjson = msg.ToString();
                    var bufferd = new byte[10 * 1024];
                    bufferd = Encoding.Default.GetBytes(msjson);
                    await webs.SendAsync(new ArraySegment<byte>(bufferd, 0, bufferd.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                }


            }
            catch (Exception ex)
            {
                //踢下线移除数据库
                var msg = new { type = "waring", contntsd = "网络异常" };
                //var msg = "111";
                var msjson = msg.ToString();
                var bufferd = new byte[10 * 1024];
                bufferd = Encoding.Default.GetBytes(msjson);
                await webs.SendAsync(new ArraySegment<byte>(bufferd, 0, bufferd.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

            }

            #endregion
        }

        #endregion
        #region 消息处理
        /// <summary>
        /// 接受处理消息
        /// </summary>
        public byte[] ReceiveMessages(byte[] buffer)
        {
            var msgString = Encoding.UTF8.GetString(buffer);
            dynamic p = JsonConvert.DeserializeObject(msgString);
            //处理消息
            foreach (var item in p)
            {
                this.Roomid = p.Roomid;
                this.accpet = p.accpet;
                this.SendId = p.SendId;

            }

            // this.accpet = p.accpet;//接收者
            //  string msg = p.content;
                  #region 消息入库
            //处理各个消息细节
            //Base_Action base_ = new Base_Action
            //{
            //    Id = "12334f566",
            //    Name = "1234",
            //    Deleted = false,
            //    CreatorId = "1",
            //    Type = ActionType.菜单,
            //    CreateTime = DateTime.Now,
            //    NeedAction = false,
            //    Sort = 10
            //};
            //int i = db.Insertable<Base_Action>(base_).ExecuteCommand();
            #endregion
            try
            {
                byte[] byteArray = Encoding.Default.GetBytes(msgString);
                return byteArray;
            }
            catch (Exception ex)
            {

                throw;
            }




        }


        #endregion
        #region 加入组
        /// <summary>
        /// 加入组
        /// </summary>
        public void AddGroup(string Userid, string Roomid)
        {

            try
            {
              
                if (websocketLinesds.Exists(x=>x.Room==Roomid))
                {
                  for (int i = 0; i < websocketLinesds.Count; i++)
                    {
                    if (websocketLinesds[i].Room == Roomid)
                    {
                        websocketLinesds[i].userid.Add(Userid);
                    }
                  

                }
                }
                else
                {
                    List<string> psd = new List<string>();
                    psd.Add(Userid);

                    websocketLinesds.Add(new WebsocketLine
                    {
                        Room = Roomid,
                        userid = psd
                    });
                }
              

                //}
           
              


                if (websocketLinesds.Count == 0)
            {
                List<string> psd = new List<string>();
                psd.Add(Userid);

                websocketLinesds.Add(new WebsocketLine
                {
                    Room = Roomid,
                    userid = psd
                });
            }

            }
            catch (Exception ex)
            {

                throw;
            }
         

          


        }

        #endregion
        #region 离线移除
        public void Remove(string Userid)
        {
            webline.Remove(Userid);
            foreach (var item in websocketLinesds)
            {
                foreach (var items in item.userid)
                {
                    if (items==Userid)
                    {
                      item.userid.Remove(Userid);
                    }
                 
                }
               
            }

        }

        #endregion
        #region 添加链接人
        public async Task AddWebLine(HttpContext context, WebSocket webSocket)
        {
            #region [客户端添加进入字典集合]
            var s = context.Request.Query["Id"];
            try
            {
                WebSocket sValue = null;
                if (!webline.TryGetValue(s, out sValue))
                {
                    webline.Add(s, webSocket);
                }

                //存入链接的人

            }
            catch (Exception ex)
            {

                throw;
            }
            #endregion
        }

        #endregion


        #region 移除群聊
        public async Task RemoveGroupAsync(string Userid)
        {
            foreach (var item in websocketLinesds)
            {
                foreach (var items in item.userid)
                {
                    if (items == Userid)
                    {
                        item.userid.Remove(Userid);
                    }

                }

            }
        }

        #endregion


        #region 数据库连接字符串
        public static string connectionString = "Server=.;Database=nxzjfx;Integrated Security=False;User ID=sa;Password=123456;";
        SqlSugarClient db = new SqlSugarClient(
             new ConnectionConfig
             {
                 ConnectionString = connectionString,
                 DbType = DbType.SqlServer,
                 IsShardSameThread = true
             }
            );
        #endregion
    }
}
