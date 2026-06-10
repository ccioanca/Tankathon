using Godot;

namespace Tankathon.Scripts;

[GlobalClass]
public partial class TournamentBracket : Resource
{
    /// <summary>
    /// Round 1 battles, manually configured in the Godot editor.
    /// Each BattleInfo already contains its teams, time, and name.
    /// </summary>
    [Export]
    public Godot.Collections.Array<BattleInfo> seedBattles = new();

    /// <summary>
    /// Template BattleInfo copied for auto-generated subsequent rounds.
    /// Teams are populated at runtime from previous round winners.
    /// </summary>
    [Export]
    public BattleInfo templateBattle;

    /// <summary>
    /// Template BattleInfo copied for tiebreaker battles.
    /// Typically configured as 1v1, SuddenDeath (60s).
    /// </summary>
    [Export]
    public BattleInfo tiebreakerTemplate;

    /// <summary>
    /// Teams per battle for auto-generated rounds (2–4). Defaults to 4.
    /// </summary>
    [Export(PropertyHint.Range, "2,4")]
    public int teamsPerBattle = 4;
}
