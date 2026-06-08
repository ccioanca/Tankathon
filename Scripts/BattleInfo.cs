using Godot;

namespace Tankathon.Scripts;

[GlobalClass]
public partial class BattleInfo : Resource
{
    [Export]
    public string battleName;

    [Export(PropertyHint.Enum)] public BattleLength battleTime;

    [Export]
    public Godot.Collections.Array<TeamData> teams;

    public bool injected = false;

    public BattleInfo()
    {
        teams = new Godot.Collections.Array<TeamData>();
    }

    public enum BattleLength
    {
        None = 0,
        Standard = 3*60,
        SuddenDeath = 60
    }
}
