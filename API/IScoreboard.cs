using Godot;
using System.Threading;

namespace Tankathon.API
{
    public interface IScoreboard
    {
        public int timeLeft { get; }

        public int GetScoreForTeam(TankTeam team);
    }
}