using System;
using System.Linq;
using Exiled.API.Features;
using CommandSystem;
using RemoteAdmin;
using VoteKick;
using MEC;
using System.Collections.Generic;

namespace VotekickCommand
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class VotekickCommand : ICommand
    {
        private CoroutineHandle _voteStatusCoroutine;
        private CoroutineHandle _voteTimeoutCoroutine;
        private static DateTime LastVoteKickTime = DateTime.Now;
        private static readonly TimeSpan VoteKickCooldown = TimeSpan.FromSeconds(Plugin.Instance.Config.Cooldown);

        public string Command { get; } = "votekick";
        public string[] Aliases { get; } = { "vk" };
        public string Description { get; } = "Initiate or vote on a votekick against a player";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player playersender = Player.Get(sender);
            if (arguments.Count < 1)
            {
                response = "Usage: .votekick start <playername> OR .votekick <yes/no>";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "start":
                    return StartVoteKick(arguments, sender, out response, playersender);
                case "yes":
                case "no":
                    return Vote(arguments, sender, out response, playersender);
                default:
                    response = "Invalid subcommand. Usage: .votekick start <playername> OR .votekick <yes/no>";
                    return false;
            }
        }

        private bool StartVoteKick(ArraySegment<string> arguments, ICommandSender sender, out string response, Player playersender)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Only players can initiate a votekick";
                return false;
            }

            if (Plugin.Instance.IsBlacklistedInitiator(playersender))
            {
                response = "You are not allowed to start a votekick";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: .votekick start <playername> OR .votekick <yes/no>";
                return false;
            }

            string targetName = string.Join(" ", arguments.Skip(1));
            Player target = Player.Get(targetName);

            if (target == null)
            {
                response = $"Player '{targetName}' not found";
                return false;
            }

            if (Plugin.Instance.IsBlacklistedTarget(target))
            {
                response = "This player cannot be votekicked";
                return false;
            }

            if (Plugin.Instance.IsVotekickInProgress)
            {
                response = "There's already a votekick in progress";
                return false;
            }

            if (DateTime.Now - LastVoteKickTime < VoteKickCooldown)
            {
                TimeSpan remainingCooldown = VoteKickCooldown - (DateTime.Now - LastVoteKickTime);
                response = $"You must wait {remainingCooldown.TotalSeconds} seconds before starting another vote kick";
                return false;
            }

            Plugin.Instance.StartVoteKick(playersender, target);

            _voteStatusCoroutine = Timing.RunCoroutine(Plugin.Instance.BroadcastVoteStatus());
            _voteTimeoutCoroutine = Timing.RunCoroutine(StopVoteKickAfterTimeout());

            response = $"Votekick initiated against {target.Nickname}. Type '.vk yes' or '.vk no' to vote";
            return true;
        }

        private bool Vote(ArraySegment<string> arguments, ICommandSender sender, out string response, Player playersender)
        {
            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Only players can vote on a votekick";
                return false;
            }

            if (!Plugin.Instance.IsVotekickInProgress)
            {
                response = "There's no votekick in progress to vote on";
                return false;
            }

            if (Plugin.Instance.HasPlayerVoted(playersender))
            {
                response = "You have already voted on this votekick";
                return false;
            }

            bool vote = arguments.At(0).ToLower() == "yes";
            Plugin.Instance.Vote(playersender, vote);

            response = $"You've voted {(vote ? "yes" : "no")} on the current votekick";
            return true;
        }

        private IEnumerator<float> StopVoteKickAfterTimeout()
        {
            yield return Timing.WaitForSeconds(Plugin.Instance.Config.VKTime);
            LastVoteKickTime = DateTime.Now;
            Plugin.Instance.StopVoteKick();
            yield break;
        }
    }
}