using System;
using System.Linq;
using Godot;
using Godot.Collections;
using Tankathon.API;
using TestTankathon.API;

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
	private IScoreboard _scoreboard;

	//collision shape
	CollisionShape2D _collisionShape;

	PackedScene bullet;
    Marker2D turret;

	//for the raycasting
	private PhysicsDirectSpaceState2D spaceState;
	private PhysicsRayQueryParameters2D query;
	private Dictionary result;
	private Entity entityInPath = new Entity();


    public override void _Ready()
	{
		_passedActions = GetNode<Actions>("Actions");
		_scoreboard = GetParent().GetNode<Scoreboard>("Scoreboard");

		//get the turret object
		turret = GetNode<Marker2D>("Turret");
		GD.Print("is turret: " + turret.Name);
		GD.Print("TurretPath: " + turret.GetPath());

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

	internal void Shoot()
	{
		Node2D bulletInstance = (Node2D)bullet.Instantiate();
		bulletInstance.Position = turret.GlobalPosition;
		bulletInstance.Rotation = this.Rotation;
		GetParent().AddChild(bulletInstance);
    }

	internal Entity LookAt()
	{
        spaceState = GetWorld2D().DirectSpaceState;
        // use global coordinates, not local to node
        query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + new Vector2(0, -1500));
        query.Exclude = new Godot.Collections.Array<Rid> { GetRid() };
        result = spaceState.IntersectRay(query);

		if(result.Count > 0)
		{
			var entity = result["collider"].As<CollisionObject2D>();

            entityInPath.eType = (entity as IEntity).eType;
			entityInPath.globalPosition = entity.GlobalPosition;
			entityInPath.rotation = entity.Rotation;
			entityInPath.distanceTo = entity.GlobalPosition.DistanceTo(_collisionShape.GlobalPosition) - (_collisionShape.Shape.GetRect().Size.Y/2) - (entity.GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size.Y / 2);

            GD.Print((result["collider"].As<CollisionObject2D>() as IEntity).eType);

			return entityInPath;
		}
		return new Entity();
    }

    public override void _Draw()
    {
        DrawLine(new Vector2(0, 0), new Vector2(0, -1500), Colors.White, 2);
    }

    internal void Hurt()
	{
        GD.Print("Tank Hurt: " + Name);
    }

}

