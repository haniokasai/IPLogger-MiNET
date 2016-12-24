using log4net;
using Microsoft.AspNet.Identity;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLogger_MiNET
{
    [Plugin(PluginName = "IPLogger_MiNET", Description = "IPLogger for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        protected static ILog _log = LogManager.GetLogger("IPLogger_MiNET");


        // 生徒名簿となる連想配列
        Dictionary<string, Dictionary<string, string>> array;

        protected override void OnEnable()
        {
            Context.Server.PlayerFactory.PlayerCreated += PlayerFactory_PlayerCreated;
            _log.Warn("Loaded");

        }

        private void PlayerFactory_PlayerCreated(object sender, PlayerEventArgs e)
        {
            var player = e.Player;
            player.PlayerJoin += Player_PlayerJoin;//generate player join event
        }

        private void Player_PlayerJoin(object sender, PlayerEventArgs e)
        {
            Player player = e.Player;
            _log.Warn(player.ClientUuid.ToString());
            _log.Warn(player.ClientId.ToString());
            _log.Warn(player.EndPoint.Address.MapToIPv4());
            _log.Warn(player.EndPoint.Port.ToString());


            string json;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            array = new Dictionary<string, Dictionary<string, string>>();

            dic.Add("uuid", player.ClientUuid.ToString());
            dic.Add("cid", player.ClientId.ToString());
            dic.Add("ip", player.EndPoint.Address.MapToIPv4().ToString());
            dic.Add("port", player.EndPoint.Port.ToString());
            array.Add(player.Username, dic);
            json = JsonConvert.SerializeObject(array);
            Console.WriteLine(json);
        }

        [Command(Name = "ipl", Description = "Iplogger ", Permission = "com.haniokasai.ipl")]
        public void ipl(Player player)
        {

            player.SendMessage("Removed items from your Inventory!");
        }
    }
}
