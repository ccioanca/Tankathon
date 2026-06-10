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

/// <summary>
/// A Round keeps track of its matches and whether all matches are complete (i.e., have a winner assigned).
/// </summary>
public class Round
{
    public List<MatchSlot> Matches { get; set; } = new();
    public bool IsComplete => Matches.TrueForAll(m => m.Winner != null);
}

/// <summary>
/// We keep a reference to the BattleInfo for each match slot, but the Winner is assigned at runtime after the battle concludes.
/// </summary>
public class MatchSlot
{
    public BattleInfo BattleInfo { get; set; }
    public TeamData Winner { get; set; } // null until manually assigned via keyboard
}
