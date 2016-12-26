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
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IPLogger_MiNET
{
    [Plugin(PluginName = "IPLogger_MiNET", Description = "IPLogger for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        protected static ILog _log = LogManager.GetLogger("IPLogger_MiNET");
        string db_file;

        protected override void OnEnable()
        {

            string pluginDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            db_file = pluginDirectory+"/iplogger.db";

            using (var conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS players(name TEXT , ip TEXT, cid TEXT, uuid TEXT)";
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }

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

            string ip = player.EndPoint.Address.MapToIPv4().ToString();
            string cid = player.ClientId.ToString();
            string uuid = player.ClientUuid.ToString();

            player.SetGameMode(MiNET.Worlds.GameMode.Creative);

            string name = player.Username;


            using (var conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                Boolean cont = true;

                using (SQLiteTransaction sqlt = conn.BeginTransaction())
                {

                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT * from players WHERE name='" + name + "'";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _log.Warn(reader["ip"].ToString());
                                if(reader["ip"].ToString() == ip)
                                {
                                    if (reader["cid"].ToString() == cid)
                                    {
                                        if (reader["uuid"].ToString() == uuid)
                                        {
                                            cont = false;
                                        }
                                    }
                                }
                            }
                        }

                        if (cont)
                        {
                            command.CommandText = "insert into players (name,ip,cid,uuid) values('" + name + "', '" + ip + "', '" + cid + "', '" + uuid + "')";
                            command.ExecuteNonQuery();
                        }
                    }
                    sqlt.Commit();
                }
                conn.Close();
            }

            

        }

        [Command(Name = "ipl", Description = "Iplogger for MiNET ", Permission = "com.haniokasai.ipl")]
        public bool ipl(Player player, string[] args)
        {
            if (args.Count() == 0)
            {
                player.SendMessage("/ipl <username>");
                return false;
            }
            Match mc = Regex.Match(@"/^[a-zA-Z0-9_]+$/u", args[0]);
            // 引っかかったか？
            if (!mc.Success)
            {
                player.SendMessage("invaild <username>");
                return false;
            }

                using (var conn = new SQLiteConnection("Data Source=" + db_file))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * from players WHERE name='" + args[0] + "'";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _log.Warn("/////");
                            _log.Warn(reader["ip"]);
                            _log.Warn(reader["cid"]);
                            _log.Warn(reader["uuid"]);
                        }
                    }
                }
                conn.Close();
            }
            return true;
        }
    }
}
