using Godot;
using System;
using Tankathon.API;

public partial class Obstacle : StaticBody2D, IEntity
{
    public EntityType eType => EntityType.Obstacle;
}
