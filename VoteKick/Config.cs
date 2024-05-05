using Exiled.API.Interfaces;
using System;
using System.ComponentModel;

namespace VoteKick
{
    public sealed class Config : IConfig
    {
        [Description("is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("is the Debug mode enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Global VoteKick Cooldown (int) (default 20)")]
        public int Cooldown { get; set; } = 20;

        [Description("Votekick total time (int) (default 30)")]
        public int VKTime { get; set; } = 30;

        [Description("How much percent of the current players ingame need to vote yes for a kick (0.25 = 25%) (default 25%/0.25)")]
        public Double ListPercent { get; set; } = 0.25;
    }
}