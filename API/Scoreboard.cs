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
		[Signal]
		public delegate void BattleFrozenEventHandler();

		public int timeLeft => (int)_timer.TimeLeft;


		//private members
		Timer _timer = new Timer();
		Label _timeLeft;
		//Label _blueScore;
		//Label _redScore;

		public override void _Ready()
		{
			base._Ready();

			//Add the 5 Minute Round Timer
			AddChild(_timer);
			_timer.OneShot = true;
			_timer.Timeout += () => Timeout();
			_timeLeft = GetNode<Label>("TimeLeft");
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
			Engine.TimeScale = 0;
			EmitSignal(SignalName.BattleFrozen);
		}
		public void RestartPressed()
		{
			GetTree().ReloadCurrentScene();
		}

		public void StartTimer(double time = (3*60))
		{
			_timer.Start(time); //start the timer
		}
        
    }
}
