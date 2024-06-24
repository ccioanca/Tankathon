using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Tankathon.API.Internal;

namespace Tankathon.API
{
	public partial class Scoreboard : Control, IScoreboard
	{
		[Export]
		private int blueScore { get; set; }
		[Export]
		private int redScore { get; set; }

		public int timeLeft => (int)_timer.TimeLeft;


        //private members
        Timer _timer = new Timer();
		Label _timeLeft;
		Label _blueScore;
		Label _redScore;

        public override void _Ready()
		{
			base._Ready();

			//Add the 5 Minute Round Timer
            AddChild(_timer);
            _timer.Timeout += () => Timeout();
            _timer.Start(5*60); //start the 5 minute timeLeft
			_timeLeft = GetNode<Label>("Panel/TimeLeft");

			//score
            _blueScore = GetNode<Label>("Panel/Panel2/BlueScore");
			_redScore = GetNode<Label>("Panel/Panel/RedScore");

        }

        public override void _Process(double delta)
		{
			base._Process(delta);
		}

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

			_timeLeft.Text = TimeSpan.FromSeconds(_timer.TimeLeft).ToString(@"mm\:ss");
        }

        private void Timeout()
		{
			//timeLeft is done, restart? 
			GD.Print("Restarting");
            GetTree().ReloadCurrentScene();
        }

		public void ScoreChanged(TankTeam teamHurt)
		{
			//score changed
			switch (teamHurt)
			{
				case TankTeam.Red:
					blueScore++;
					_blueScore.Text = (blueScore).ToString();
					break;
				case TankTeam.Blue:
					redScore++;
                    _redScore.Text = (redScore).ToString();
                    break;
				default:
					break;
			}
		}

		public int GetScoreForTeam(TankTeam team)
        {
            switch(team)
			{
				case TankTeam.Red:
					return redScore;
				case TankTeam.Blue:
					return blueScore;
				default: return -1;
			}
        }

		public void RestartPressed()
		{
            GetTree().ReloadCurrentScene();
        }
        
    }
}
