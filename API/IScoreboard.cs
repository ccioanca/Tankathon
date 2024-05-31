using System.Threading;

namespace Tankathon.API
{
    public interface IScoreboard
    {
        public int timer { get; }
        public Score score { get; }
    }
}