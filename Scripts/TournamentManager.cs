using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tankathon.API;
using Tankathon.API.Internal;
using Tankathon.Scripts;

namespace Tankathon.Scripts;

public partial class TournamentManager : Node2D
{
	[ExportGroup("Tournament")]
	[Export]
	private TournamentBracket bracket;

	[ExportGroup("Scene References")]
	[Export]
	private PackedScene _battleScene;
	[Export]
	private NodePath animResultsPlayerPath;
    [Export]
    private NodePath animConfettiPath;
    [Export]
	private NodePath winnerLabelPath;
    [Export]
    private NodePath preWinnerLabelPath;

    private GameManager _gameManager;
	private Scoreboard _scoreboard;
	private AnimationPlayer _animPlayer;
	private AnimationPlayer _animConfetti;
    private Label _winnerLabel;
	private Label _preWinnerLabel;

	private TournamentState _state;

	private enum Phase
	{
		PreBattle,
		BattleRunning,
		BattleFrozen,
		ShowingResults,
		TieSelectFirst,
		TieSelectSecond
	}

	private Phase _phase = Phase.PreBattle;

	private int _tieFirstSelection = -1;

	public override void _Ready()
	{
		_animPlayer = GetNodeOrNull<AnimationPlayer>(animResultsPlayerPath);
		_animConfetti = GetNodeOrNull<AnimationPlayer>(animConfettiPath);
		_winnerLabel = GetNodeOrNull<Label>(winnerLabelPath);
		_preWinnerLabel = GetNodeOrNull<Label>(preWinnerLabelPath);


		if (_battleScene == null)
		{
			GD.PushError("TournamentManager: Battle scene PackedScene is not assigned.");
			return;
		}

		if (bracket == null)
		{
			GD.PushError("TournamentManager: TournamentBracket resource is not assigned.");
			return;
		}

		_state = BuildBracket(bracket);

		_phase = Phase.PreBattle;

		StartCurrentBattle();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is not InputEventKey { Pressed: true } key)
			return;

		switch (_phase)
		{
			case Phase.PreBattle:
				if (key.Keycode is Key.Space)
					StartCurrentBattle();
				break;

			case Phase.BattleFrozen:
				if (key.Keycode == Key.T)
				{
					BeginTieSelection();
					return;
				}

				var winnerIndex = ParseWinnerIndex(key.Keycode);
				if (winnerIndex >= 0)
					AssignWinner(winnerIndex);
				break;

			case Phase.TieSelectFirst:
				HandleTieFirstSelection(key.Keycode);
				break;

			case Phase.TieSelectSecond:
				HandleTieSecondSelection(key.Keycode);
				break;

			case Phase.ShowingResults:
				if (key.Keycode is Key.Space)
					AdvanceToNext();
				break;
		}
	}

	public void OnBattleFrozen()
	{
		if (_phase != Phase.BattleRunning)
			return;

		_gameManager.StopGame();

		_phase = Phase.BattleFrozen;
		GD.Print("Battle frozen. Press 1-4 to assign winner, or T for tiebreaker.");
	}

	private TournamentState BuildBracket(TournamentBracket config)
	{
		var state = new TournamentState();
		var round1 = new Round();

		foreach (var battle in config.seedBattles)
			if (battle != null)
				round1.Matches.Add(new MatchSlot { BattleInfo = battle });

		state.Rounds.Add(round1);

		var remaining = round1.Matches.Count;
		while (remaining > 1)
		{
			state.Rounds.Add(new Round());
			remaining = (remaining + config.teamsPerBattle - 1) / config.teamsPerBattle;
		}

		return state;
	}

	private void StartCurrentBattle()
	{
		if (_state.IsComplete)
		{
			ShowChampion();
			return;
		}

		var match = _state.CurrentMatch;
		if (match?.BattleInfo == null)
		{
			GD.PushError("TournamentManager: Current match or BattleInfo is null.");
			return;
		}

		// Free previous battle scene if one exists
		if (_gameManager != null)
		{
			_gameManager.QueueFree();
			_gameManager = null;
			_scoreboard = null;
		}


        // Instantiate a fresh battle scene with battleInfo set before entering the tree
        _gameManager = _battleScene.Instantiate<GameManager>();
		_gameManager.battleInfo = match.BattleInfo;
		AddChild(_gameManager);

		// Wire up scoreboard signal
		_scoreboard = _gameManager.GetNodeOrNull<Scoreboard>("%Scoreboard");
		if (_scoreboard != null)
			_scoreboard.BattleFrozen += OnBattleFrozen;

		//_gameManager.StartGame();
		_phase = Phase.BattleRunning;

		var names = string.Join(" vs ", match.BattleInfo.teams.Select(t => t?.teamName ?? "Unknown"));
		GD.Print($"Starting Round {_state.CurrentRoundIndex + 1}, Match {_state.CurrentBattleIndex + 1}: {names}");
	}

	private int ParseWinnerIndex(Key key)
	{
		return key switch
		{
			Key.Key1 => 0,
			Key.Key2 => 1,
			Key.Key3 => 2,
			Key.Key4 => 3,
			_ => -1,
		};
	}

	private void AssignWinner(int winnerIndex)
	{
		var currentMatch = _state.CurrentMatch;
		var teams = currentMatch.BattleInfo.teams;

		if (winnerIndex < 0 || winnerIndex >= teams.Count)
		{
			GD.Print($"Invalid winner index {winnerIndex + 1}. This match has {teams.Count} team(s).");
			return;
		}

		var winner = teams[winnerIndex];
		currentMatch.Winner = winner;

		GD.Print($"Winner assigned: {winner.teamName}");
		ShowResultsScreen(winner.teamName);
	}

	private void BeginTieSelection()
	{
		_tieFirstSelection = -1;
		_phase = Phase.TieSelectFirst;
		GD.Print("Tie mode: press first team index (1-4).");
	}

	private void HandleTieFirstSelection(Key key)
	{
		var index = ParseWinnerIndex(key);
		if (!IsValidTeamIndex(index))
			return;

		_tieFirstSelection = index;
		_phase = Phase.TieSelectSecond;
		GD.Print("Tie mode: press second team index (1-4).");
	}

	private void HandleTieSecondSelection(Key key)
	{
		var _tieSecondselection = ParseWinnerIndex(key);
		if (!IsValidTeamIndex(_tieSecondselection))
			return;

		if (_tieSecondselection == _tieFirstSelection)
		{
			GD.Print("Tie mode: second team must be different from first team.");
			return;
		}

		var teams = _state.CurrentMatch.BattleInfo.teams;
		InjectTiebreaker(teams[_tieFirstSelection], teams[_tieSecondselection]);
        ShowResultsScreen($"{teams[_tieFirstSelection].teamName} and {teams[_tieSecondselection].teamName}", "It's A Tie Between");
		_phase = Phase.ShowingResults;
	}

	private bool IsValidTeamIndex(int index)
	{
		var teamCount = _state.CurrentMatch?.BattleInfo?.teams?.Count ?? 0;
		if (index < 0 || index >= teamCount)
		{
			GD.Print($"Invalid team index. Current match has {teamCount} team(s).");
			return false;
		}

		return true;
	}

	private void InjectTiebreaker(TeamData teamA, TeamData teamB)
	{
		var tiebreaker = new BattleInfo
		{
			battleName = $"Tiebreaker",
			battleTime = bracket.tiebreakerTemplate != null
				? bracket.tiebreakerTemplate.battleTime
				: BattleInfo.BattleLength.SuddenDeath,
			injected = true,
			teams = new Godot.Collections.Array<TeamData> { teamA, teamB }
		};

		_state.CurrentRound.Matches.Insert(
			_state.CurrentBattleIndex + 1,
			new MatchSlot { BattleInfo = tiebreaker }
		);
	}

	private void AdvanceToNext()
	{
		Engine.TimeScale = 1;
		//Handle Tiebreaker rules. 
		if (_state.CurrentMatch.BattleInfo.injected)
		{
			//set previous round winner. 
			_state.CurrentRound.Matches[_state.CurrentBattleIndex - 1].Winner = _state.CurrentMatch.Winner;
			//remove the tiebreaker
			_state.CurrentRound.Matches.Remove(_state.CurrentMatch);
			_state.CurrentBattleIndex--;
		}

		_animPlayer.Play("RESET");

		_state.CurrentBattleIndex++;

		if (_state.CurrentBattleIndex < _state.CurrentRound.Matches.Count)
		{
			_phase = Phase.PreBattle;
			StartCurrentBattle();
			return;
		}

		_state.CurrentRoundIndex++;
		_state.CurrentBattleIndex = 0;

		if (_state.IsComplete)
		{
			ShowChampion();
			return;
		}

		BuildNextRound();
		_phase = Phase.PreBattle;
		StartCurrentBattle();
	}

	private void BuildNextRound()
	{
		var previousRound = _state.Rounds[_state.CurrentRoundIndex - 1];
		var winners = previousRound.Matches
			.Where(m => m.Winner != null)
			.Select(m => m.Winner)
			.ToList();

		var groupSize = Math.Max(2, bracket.teamsPerBattle);
		var nextRound = new Round();

		for (var i = 0; i < winners.Count; i += groupSize)
		{
			var count = Math.Min(groupSize, winners.Count - i);
			var teams = new Godot.Collections.Array<TeamData>();
			for (var j = 0; j < count; j++)
				teams.Add(winners[i + j]);

			var battle = new BattleInfo
			{
				battleName = $"Bracket {_state.CurrentRoundIndex + 1} - Battle {nextRound.Matches.Count + 1}",
				battleTime = bracket.templateBattle != null
					? bracket.templateBattle.battleTime
					: BattleInfo.BattleLength.Standard,
				teams = teams
			};

			nextRound.Matches.Add(new MatchSlot { BattleInfo = battle });
		}

		_state.Rounds[_state.CurrentRoundIndex] = nextRound;
	}

	private void ShowResultsScreen(string text, string preText = "And The Winner Is", bool isChamp = false)
	{
        Engine.TimeScale = 1f;

        if (_winnerLabel != null)
			_winnerLabel.Text = text;

        if (_preWinnerLabel != null)
            _preWinnerLabel.Text = preText;

		if(!isChamp)
			_animPlayer.Play("show_win_screen");
		else if(isChamp)
            _animPlayer.Play("show_champ_screen");

        _phase = Phase.ShowingResults;
	}

	private void ShowChampion()
	{
		var lastCompletedRound = _state.Rounds.LastOrDefault(r => r.Matches.Any(m => m.Winner != null));
		var champion = lastCompletedRound?.Matches.LastOrDefault()?.Winner;
		var championName = champion?.teamName ?? "Unknown";

		ShowResultsScreen(championName, "And The Champion Is", true);
        _animConfetti.Play("show_champ_confetti");
        GD.Print($"Tournament complete. Champion: {championName}");
	}
}
