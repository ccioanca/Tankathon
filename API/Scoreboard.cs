using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Tankathon.API
{
	public partial class Scoreboard : Node2D, IScoreboard
	{
		[Export]
		public int blueScore { get; set; }
		[Export]
		public int redScore { get; set; }
		[Export]
		public int TimeRemaining { get; set; }

		public int timer => TimeRemaining;

		public Score score => new Score(blueScore, redScore);

		public override void _Ready()
		{
			base._Ready();
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
		}
	}
}
