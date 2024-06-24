using Godot;
using Godot.Collections;

namespace Tankathon.API.Internal;

public partial class TheTank : CharacterBody2D, IEntity
{
	[Export]
	public string TankName = "TankName";

	public EntityType eType => EntityType.Tank; 

	public bool col = false;
    public Vector2 _velocity = Vector2.Zero;
	
    public ITank thisTank;
	Actions actions;
	private IActions _passedActions;
	private Scoreboard _scoreboard;

	//collision shape
	CollisionShape2D _collisionShape;

	PackedScene bullet;
    Marker2D turret;

	//for the raycasting
	private PhysicsDirectSpaceState2D spaceState;
	private PhysicsRayQueryParameters2D query;
	private Dictionary result;
	private Entity entityInPath = new Entity();

	public TankTeam team { get; set; }

    public override void _Ready()
	{
		_passedActions = GetNode<Actions>("Actions");
		_scoreboard = GetParent().GetNode<Scoreboard>("Scoreboard");

		//get the turret object
		turret = GetNode<Marker2D>("Turret");

        //get the bullet preloaded
        bullet = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");

		//get sel references
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        base._Ready();
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		thisTank.Do(_passedActions, _scoreboard);
		var k2d = MoveAndCollide(_velocity);
		if (k2d != null)
			col = true;
		else 
			col = false;

		base._PhysicsProcess(delta);
	}

	internal void Init(TankTeam _team)
	{
		team = _team;
        thisTank.Setup(_passedActions.stats);
    }

	internal void Shoot()
	{
		Bullet bulletInstance = (Bullet)bullet.Instantiate();
		bulletInstance.Position = turret.GlobalPosition;
		bulletInstance.Rotation = this.Rotation;
		bulletInstance.initializer = this;
		GetParent().AddChild(bulletInstance);
    }

	internal Entity LookAt()
	{
        spaceState = GetWorld2D().DirectSpaceState;
        // use global coordinates, not local to node
        query = PhysicsRayQueryParameters2D.Create(GlobalPosition, ToGlobal(new Vector2(0, -1500)));
		query.CollideWithAreas = true;
        query.Exclude = new Array<Rid> { GetRid() };
        result = spaceState.IntersectRay(query);

		if(result.Count > 0)
		{
			var entity = result["collider"].As<CollisionObject2D>();

            entityInPath.eType = (entity as IEntity).eType;
			entityInPath.globalPosition = entity.GlobalPosition;
			entityInPath.rotation = entity.Rotation;
			entityInPath.distanceTo = ((Vector2)result["position"]).DistanceTo(_collisionShape.GlobalPosition) - (_collisionShape.Shape.GetRect().Size.Y / 2);

            return entityInPath;
		}
		return new Entity();
    }

    public override void _Draw()
    {
		DrawLine(new Vector2(0, 0), new Vector2(0, -1500), Colors.Green, 2);
    }

    internal void Hurt()
	{
		_scoreboard.ScoreChanged(team);
    }

}

