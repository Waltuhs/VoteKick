using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VoteKick
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; } = null;
        public override string Author => "sexy waltuh";
        public override string Name => "VoteKick";
        public override string Prefix => "VK";
        public override Version Version => new Version(1, 0, 0);
        private Dictionary<Player, Player> _voteKicksInProgress = new Dictionary<Player, Player>();
        private Dictionary<Player, bool> _votedPlayers = new Dictionary<Player, bool>();
        private HashSet<Player> _votedYes = new HashSet<Player>();
        private HashSet<Player> _votedNo = new HashSet<Player>();
        private CoroutineHandle? _voteStatusCoroutine;
        private CoroutineHandle? _voteTimeoutCoroutine;
        private int _yesVotes = 0;
        private int _noVotes = 0;

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Instance = null;
        }

        public bool IsVotekickInProgress => _voteKicksInProgress.Any();
        public int VoteYesCount => _votedYes.Count;
        public int VoteNoCount => _votedNo.Count;

        public void StartVoteKick(Player initiator, Player target)
        {
            _voteKicksInProgress.Clear();
            _votedYes.Clear();
            _votedNo.Clear();

            _voteKicksInProgress.Add(initiator, target);
        }

        public Player GetVoteKickTarget()
        {
            return _voteKicksInProgress.Values.FirstOrDefault();
        }

        public bool HasPlayerVoted(Player player)
        {
            return _votedPlayers.ContainsKey(player);
        }

        public bool IsCoroutineRunning(CoroutineHandle handle)
        {
            return Timing.IsRunning(handle);
        }


        public void StopVoteKick()
        {
            _voteKicksInProgress.Clear();
            _votedPlayers.Clear();
            _votedYes.Clear();
            _votedNo.Clear();
            _yesVotes = 0;
            _noVotes = 0;

            if (_voteStatusCoroutine.HasValue) 
            {
                Timing.KillCoroutines(_voteStatusCoroutine.Value);
                _voteStatusCoroutine = null;
            }

            if (_voteTimeoutCoroutine.HasValue) 
            {
                Timing.KillCoroutines(_voteTimeoutCoroutine.Value);
                _voteTimeoutCoroutine = null;
            }
        }

        public IEnumerator<float> BroadcastVoteStatus()
        {
            while (Plugin.Instance.IsVotekickInProgress)
            {
                Player target = Plugin.Instance.GetVoteKickTarget();
                string targetName = target != null ? target.Nickname : "Unknown";
                Map.ShowHint($"<align=left>\n \n \n \n \n <color=#A9A9A9>Vote kick initiated!</color> \n target: {targetName} \n <color=#008000>yes: {_yesVotes}</color> \n <color=red>no: {_noVotes}</color></align>", 1);
                Map.Broadcast(1, $"<color=#A9A9A9><size=25>type .vk yes to vote yes or .vk no to vote no!</color></size>");
                yield return Timing.WaitForSeconds(0.9f);
            }
        }

        public void Vote(Player voter, bool vote)
        {
            if (!_voteKicksInProgress.ContainsValue(voter) || _votedPlayers.ContainsKey(voter))
                return;

            _votedPlayers[voter] = vote;

            if (vote)
            {
                _yesVotes++;
            }
            else
            {
                _noVotes++;
            }

            Player target = GetVoteKickTarget();
            string targetName = target != null ? target.Nickname : "Unknown";
            if (_yesVotes >= Player.List.Count * Plugin.Instance.Config.ListPercent)
            {
                if (target != null)
                {
                    target.Kick($"You were votekicked by the server.");
                    StopVoteKick();
                    Map.ShowHint($"{targetName} was successfully kicked");
                }
            }
        }
    }
}
