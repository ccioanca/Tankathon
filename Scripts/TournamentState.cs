using System.Collections.Generic;

namespace Tankathon.Scripts;

/// <summary>
/// Runtime bracket state. Lives in memory only — not a Godot Resource.
/// </summary>
public class TournamentState
{
    public List<Round> Rounds { get; set; } = new();
    public int CurrentRoundIndex { get; set; } = 0;
    public int CurrentBattleIndex { get; set; } = 0;
    public bool IsComplete => CurrentRoundIndex >= Rounds.Count;

    public Round CurrentRound => IsComplete ? null : Rounds[CurrentRoundIndex];
    public MatchSlot CurrentMatch => CurrentRound?.Matches[CurrentBattleIndex];
}

public class Round
{
    public List<MatchSlot> Matches { get; set; } = new();
    public bool IsComplete => Matches.TrueForAll(m => m.Winner != null);
}

public class MatchSlot
{
    public BattleInfo BattleInfo { get; set; }
    public TeamData Winner { get; set; } // null until manually assigned via keyboard
}
