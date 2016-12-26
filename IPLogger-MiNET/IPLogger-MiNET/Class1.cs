using log4net;
using Microsoft.AspNet.Identity;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Security;
using Newtonsoft.Json;
using System;
using System.Collections;
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

            player.SetGameMode(MiNET.Worlds.GameMode.Creative);

            Dictionary<string, Dictionary<string, ArrayList>> array;
            Dictionary<string, ArrayList> dic = new Dictionary<string, ArrayList>();
            array = new Dictionary<string, Dictionary<string, ArrayList>>();

            ArrayList ip = new ArrayList();
            ip.Add(player.EndPoint.Address.MapToIPv4().ToString());

            //dic.Add("uuid", player.ClientUuid.ToString());
            //dic.Add("cid", player.ClientId.ToString());
            dic.Add("ip",ip);
            //dic.Add("port", player.EndPoint.Port.ToString());
            array.Add(player.Username, dic);

            string json;
            json = JsonConvert.SerializeObject(array);
            Console.WriteLine(json);

            Dictionary<string, Dictionary<string, ArrayList>> jsoned;
            jsoned = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ArrayList>>>(json);

            string name = player.Username;

            string[] iparray = (string[])jsoned[name]["ip"].ToArray(typeof(string));
            string ips = string.Join(",", iparray);

            _log.Warn("/*/*/*/*/IPLIST/*/*/*/*/");
            _log.Warn("PlayerName: " +name);
            _log.Warn(ips);

            
        }

        [Command(Name = "ipl", Description = "Iplogger for MiNET ", Permission = "com.haniokasai.ipl")]
        public void ipl(Player player)
        {

        }
    }
}
