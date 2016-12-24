using log4net;
using Microsoft.AspNet.Identity;
using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Security;
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



        }

    }
}
