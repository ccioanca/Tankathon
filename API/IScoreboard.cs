using System.Threading;

namespace TestTankathon.API
{
    public interface IScoreboard
    {
        public int timer { get; }
        public Score score { get; }
    }
}