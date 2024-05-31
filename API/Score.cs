using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankathon.API
{
    public class Score
    {
        public int blueScore;
        public int redScore;
        public Score(int blueScore, int redScore)
        {
            this.blueScore = blueScore;
            this.redScore = redScore;
        }
    }
}
