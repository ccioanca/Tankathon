using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Tankathon.Scripts;
using Tankathon.Tools;

namespace Tankathon.API.Internal;

public partial class TheTank : CharacterBody2D, IEntity
{
	[Export]
	public string TankName = "TankName";

	[ExportGroup("Tank Visuals")]
	[Export]
	public Array<Sprite2D> Treads = null;
    [Export]
    public Array<Sprite2D> Thrusters = null;
    [Export]
    public Sprite2D BodyFill = null;
    [Export]
    public Array<Sprite2D> Turrets = null;
    [Export]
    public Array<Sprite2D> Hulls = null;

    public EntityType eType => EntityType.Tank; 

	internal bool col = false;
	internal Vector2 _velocity = Vector2.Zero;
	internal int health = 10;
	internal int points = 0;
	
	public ITank thisTank;
	private Actions _passedActions;
	private TankSetup _tankSetup;
	private Scoreboard _scoreboard;
	private BoxContainer _tankScoreContainer;
	private ProgressBar _healthBar;
	private Label _tankLabel;

	private Node scorePanel;

	private AudioStream shootSound;
	private AudioStream deathSound;



	//Shooty things
	CollisionShape2D _collisionShape;
	PackedScene bullet;
	Marker2D turret;
	private List<Bullet> bulletsFired = new List<Bullet>();


	//For the raycasting
	private PhysicsDirectSpaceState2D spaceState;
	private PhysicsRayQueryParameters2D query_m;
	private PhysicsRayQueryParameters2D query_l;
	private PhysicsRayQueryParameters2D query_r;
	private Dictionary rayQueryResult;
	private System.Collections.Generic.Dictionary<Side, Entity> hitResults = new System.Collections.Generic.Dictionary<Side, Entity>();


	//SFX
	AudioStreamPlayer shootPlayer = new AudioStreamPlayer();
	AudioStreamPlayer deathPlayer = new AudioStreamPlayer();

	//Treads
	private CpuParticles2D treadsL;
	private CpuParticles2D treadsR;

	public GameManager gm;

	//visuals
	private const int firstUpgradeVal = 2;
	private const int secondUpgradeVal = 4;

	public override void _Ready()
	{
		_passedActions = GetNode<Actions>("Actions");
		_healthBar = GetNode<ProgressBar>("HealthBar");
		_tankLabel = GetNode<Label>("NameLabel");
		_scoreboard = GetNode<Scoreboard>("%Scoreboard");
		_tankScoreContainer = GetNode<BoxContainer>("%TanksScoreContainer");
		_tankSetup = new TankSetup();
		_tankSetup.attributes = new TankAttributes();

		//get the turret object
		turret = GetNode<Marker2D>("Turret");

		//get the bullet preloaded
		bullet = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");

		//get sel references
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		//SFX
		AddChild(shootPlayer);
		AddChild(deathPlayer);

		//Treads
		treadsL = GetNodeOrNull<Node2D>("TreadsL")?.GetChild<CpuParticles2D>(0);
		treadsR = GetNodeOrNull<Node2D>("TreadsR")?.GetChild<CpuParticles2D>(0);

		//get ref to GM
		//gm = GetTree().Root.FindChild("GameScene") as GameManager;

        base._Ready();
	}

	void OnLabelResized(){
		if (_tankLabel != null)
		{
			_tankLabel.PivotOffset = new Vector2(_tankLabel.Size.X / 2, -_tankLabel.Position.Y);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
        if (gm.GAMESTART == false)
        {
            treadsL.Emitting = false;
            treadsR.Emitting = false;
            return;
        }

        thisTank.Do(_passedActions, _scoreboard);
		var k2d = MoveAndCollide(_velocity);
		if (k2d != null)
			col = true;
		else 
			col = false;

		_tankLabel.Rotation = -this.Rotation;

		base._PhysicsProcess(delta);
	}

	internal void Init(TeamData teamInfo = null)
	{
		_healthBar.Value = health;
		thisTank.Setup(_tankSetup);

		//Validate tank attributes
		//only tanke the first 10
		int attrMax = 10;

		if (_tankSetup.attributes.moveSpeed + _tankSetup.attributes.rotationSpeed + _tankSetup.attributes.bulletSpeed + _tankSetup.attributes.reloadSpeed > attrMax)
		{
            GetTree().Paused = true;
            throw new ArgumentOutOfRangeException("Tank attributes (move speed, rotation speed, bullet speed, and reload speed) exceed the cumulative max of "+ attrMax);
		}

		//set up actions
		_passedActions.tankSpeed = _passedActions.tankSpeed * _tankSetup.attributes.moveSpeed.Remap(0, 10, 1, 2);
		_passedActions.rotateSpeed = _passedActions.rotateSpeed * _tankSetup.attributes.rotationSpeed.Remap(0, 10, 1, 2);
		_passedActions.reloadCooldown = _passedActions.reloadCooldown / _tankSetup.attributes.reloadSpeed.Remap(0, 10, 1, 2);
		_passedActions.bulletSpeed = _passedActions.bulletSpeed * _tankSetup.attributes.bulletSpeed.Remap(0, 10, 1, 2);

        //setup Scoreboard object for this tank
        SetupScoreboard(_tankSetup);

		//setup name
		_tankLabel.Text = _tankSetup.name;

		//setup sounds
		shootSound = teamInfo.Get("shootSound").As<AudioStream>();
		deathSound = teamInfo.Get("deathSound").As<AudioStream>();
		shootPlayer.Stream = shootSound;
		deathPlayer.Stream = deathSound;

		//set up tank display sprites
		//hide all treads, then re-enable the right sprites
		if (Treads?.Count > 2)
		{
			Treads.All(e => e.Visible = false);
			if (_tankSetup.attributes.rotationSpeed >= secondUpgradeVal)
				Treads[2].Visible = true;
			else if (_tankSetup.attributes.rotationSpeed >= firstUpgradeVal)
				Treads[1].Visible = true;
			else
				Treads[0].Visible = true;
		}
		//hide all thrusters, then re-enable the right sprites
		if (Thrusters?.Count > 2)
		{
			Thrusters.All(e => e.Visible = false);
			if (_tankSetup.attributes.moveSpeed >= secondUpgradeVal)
			{
				Thrusters[2].Visible = true;
				LocalizeAndSetTeamColors(Thrusters[2].GetNode<Sprite2D>("Fill"));
			}
			else if (_tankSetup.attributes.moveSpeed >= firstUpgradeVal)
			{
				Thrusters[1].Visible = true;
                LocalizeAndSetTeamColors(Thrusters[1].GetNode<Sprite2D>("Fill"));
            }
            else
			{
				Thrusters[0].Visible = true;
                LocalizeAndSetTeamColors(Thrusters[0].GetNode<Sprite2D>("Fill"));
            }

        }

		//body doesn't need any logic, there's just the one BodyFill.
		if(BodyFill?.Texture != null)
            LocalizeAndSetTeamColors(BodyFill);

        //hide all turrets, then re-enable the right sprites
        if (Turrets?.Count > 2)
		{
			Turrets.All(e => e.Visible = false);
			if (_tankSetup.attributes.bulletSpeed >= secondUpgradeVal)
				Turrets[2].Visible = true;
			else if (_tankSetup.attributes.bulletSpeed >= firstUpgradeVal)
				Turrets[1].Visible = true;
			else
				Turrets[0].Visible = true;
		}

		//hide all hulls, then re-enable the right sprites
		if (Hulls?.Count > 2)
		{
			Hulls.All(e => e.Visible = false);
			if (_tankSetup.attributes.reloadSpeed >= secondUpgradeVal)
			{
                LocalizeAndSetTeamColors(Hulls[2].GetNode<Sprite2D>("Fill"));
                Hulls[2].Visible = true;
            }
            else if (_tankSetup.attributes.reloadSpeed >= firstUpgradeVal)
			{
                LocalizeAndSetTeamColors(Hulls[1].GetNode<Sprite2D>("Fill"));
				Hulls[1].Visible = true;
            }
			else
			{
                LocalizeAndSetTeamColors(Hulls[0].GetNode<Sprite2D>("Fill"));
                Hulls[0].Visible = true;
			}
		}

        //This is for our devs mostly.
        var baseTankSprite = GetNode<Sprite2D>("TankSprite");
		if (baseTankSprite.Texture != null) {
			LocalizeAndSetTeamColors(baseTankSprite);
        }
	}

	internal void LocalizeAndSetTeamColors(Sprite2D sprite)
	{
		if (sprite.Material != null)
		{
            //Need to duplicate the material or else sometimes it is treated as shared
            sprite.Material = (ShaderMaterial)sprite.Material.Duplicate();
			((ShaderMaterial)sprite.Material).SetShaderParameter("_newcolor1", Color.FromHtml(_tankSetup.primaryColor));
            ((ShaderMaterial)sprite.Material).SetShaderParameter("_newcolor2", Color.FromHtml(_tankSetup.secondaryColor));
        }
	}

	internal void Shoot()
	{
		Bullet bulletInstance = (Bullet)bullet.Instantiate();
		bulletInstance.bulletSpeedMultiplier = _passedActions.bulletSpeed;
		bulletInstance.Position = turret.GlobalPosition;
		bulletInstance.Rotation = this.Rotation;
		bulletInstance.initializer = this;
		GetParent().AddChild(bulletInstance);
		bulletsFired.Add(bulletInstance);
		shootPlayer?.Play();
	}

	internal void PopBullet(Bullet bullet)
	{
		bulletsFired.Remove(bullet);
	}

	internal System.Collections.Generic.Dictionary<Side, Entity> LookAt()
	{
		hitResults.Clear();

		spaceState = GetWorld2D().DirectSpaceState;
		//Middle Raycast
		// use global coordinates, not local to node
		query_m = PhysicsRayQueryParameters2D.Create(GlobalPosition, ToGlobal(new Vector2(0, -1500)));
		query_m.CollideWithAreas = true;
		query_m.Exclude = [GetRid(), .. bulletsFired.Select(b => b.GetRid()).ToArray()];
		rayQueryResult = spaceState.IntersectRay(query_m);

		if(rayQueryResult.Count > 0)
		{
			var entity = rayQueryResult["collider"].As<CollisionObject2D>();
			hitResults.Add(Side.Middle, GetEntityInPath(entity));
		}

		//Left Raycast
		query_l = PhysicsRayQueryParameters2D.Create(GlobalPosition, ToGlobal(new Vector2(-150, -1500)));
		query_l.CollideWithAreas = true;
		query_l.Exclude = [GetRid(), .. bulletsFired.Select(b => b.GetRid()).ToArray()];
		rayQueryResult = spaceState.IntersectRay(query_l);

		if (rayQueryResult.Count > 0)
		{
			var entity = rayQueryResult["collider"].As<CollisionObject2D>();
			hitResults.Add(Side.Left, GetEntityInPath(entity));
		}

		//Right Raycast
		query_r = PhysicsRayQueryParameters2D.Create(GlobalPosition, ToGlobal(new Vector2(150, -1500)));
		query_r.CollideWithAreas = true;
		query_r.Exclude = [GetRid(), .. bulletsFired.Select(b => b.GetRid()).ToArray()];
		rayQueryResult = spaceState.IntersectRay(query_r);

		if (rayQueryResult.Count > 0)
		{
			var entity = rayQueryResult["collider"].As<CollisionObject2D>();
			hitResults.Add(Side.Right, GetEntityInPath(entity));
		}

		return hitResults;
	}

	internal Entity GetEntityInPath(CollisionObject2D entity)
	{
		Entity entityInPath = new Entity(
			(entity as IEntity).eType,
			entity.GlobalPosition,
			entity.Rotation,
			((Vector2)rayQueryResult["position"]).DistanceTo(_collisionShape.GlobalPosition) - (_collisionShape.Shape.GetRect().Size.Y / 2)
			);

		return entityInPath;
	}
	
	internal void Tread(API.Rotation rotation)
	{
		if(treadsL == null || treadsR == null) return;

		treadsL.Emitting = false;
		treadsR.Emitting = false;
		if(rotation == API.Rotation.CW)
			treadsL.Emitting = true;
		if(rotation == API.Rotation.CCW)
			treadsR.Emitting = true;
	}

	public override void _Draw()
	{
		if (gm.DEBUG)
		{
			DrawLine(new Vector2(0, 0), new Vector2(0, -1500), Colors.Green, 2); //Middle
			DrawLine(new Vector2(0, 0), new Vector2(150, -1500), Colors.Red, 2); //Right
			DrawLine(new Vector2(0, 0), new Vector2(-150, -1500), Colors.Blue, 2); //Left
		}
	}

	internal void SetupScoreboard(TankSetup setup)
	{
		scorePanel = GD.Load<PackedScene>("res://Scenes/Score_Panel.tscn").Instantiate<Node>();
		scorePanel.Set("tank_health", health);
		scorePanel.Set("tank_name", setup.name.Length > 12 ? setup.name.Substring(0, 10) + "..." : setup.name);

		_tankScoreContainer.AddChild(scorePanel);
		scorePanel.Call("change_health", health);
		scorePanel.Call("change_panel_color", setup.primaryColor, setup.secondaryColor);

	}

	internal void Hurt()
	{
		if (gm.GAMESTART == false)
			return;

		health--;
		//_scoreboard.ScoreChanged(team);

		_healthBar.Value = health;
		scorePanel.Call("change_health", health);

		if (health <= 0)
		{
			deathPlayer.Reparent(GetTree().Root);
			deathPlayer.Play();

			this.QueueFree();
		}
	}

	internal void Score()
	{
        if (gm.GAMESTART == false)
            return;

        points++;
		scorePanel.Call("change_points", points);
	}
}
