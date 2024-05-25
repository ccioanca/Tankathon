using System;

namespace Tankathon.API;

public interface ITank
{
	string Name { get; set; }
	void Do();
}
