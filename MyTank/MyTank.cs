using Godot;
using System;
using Tankathon.API;

public class MyTank : ITank 
{
    public string _name;
    public string Name { get => _name; set => _name = value; }

    public void Do()
    {
        
    }
}
