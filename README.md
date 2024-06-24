# Tankathon Challenge

Technical Documentation

## Project Setup 

To set up, simply download the git repository from the latest Release on the sidebar to the right. 

The git repository will include a .NET solution which you can open in any modern Visual Studio version, or your IDE of choice. 

The git repository will also include a folder containing the Godot Game Engine. 

Please note you do not need to know anything about how the Godot Game Engine works to get this up and running, but you will need to know how to open a project and how to run a project.

## Running the Godot Project

Included with the Git repository, you will have a `Godot.zip` file as well. This folder contains the entire Godot game engine that you’ll need to run the project. Extract this to a local folder on your machine. 

To open a Godot project, either double click the `project.godot` in the repository and, if no default app is set up, navigate to the `Godot.exe` executable and set it up as the default app for opening the `.godot` extension; or open up the `Godot.exe` app, and import a new project to the project list, navigate to the `project.godot` file in the Tankathon repository, and select it. 

Once the project is open, wait for the engine UI to load, and then simply click the play button at the top right of the Godot GUI (or press F5) to run the application.

A new window should pop up and immediately start the project application. 

While this method doesn’t have any preset way to hit breakpoints, it does have access to the Console Debugger at the bottom of the UI where all debug print messages, and errors will be displayed.

## Navigating the .NET Project

The .NET solution may have a lot of files, and you’re free to look throughout them all as you wish, though most will be of little impact or relevance to what you will be creating.

The important folder to identify is the `MyTank` folder. This is where the whole of your work will happen. Inside this folder you will see a `MyTank.cs` Class. This is your tank.  

## Debugging the Project via Visual Studio

To set up Visual Studio Debugging, you will need to add a new executable profile to your Visual Studio instance. 

In your Launch Profile dropdown, you should see two options: `Tankathon` and `Godot Executable`. 

The profile you’ll be working with is “Godot Executable” for the entirety of this challenge. To set this up, click on the dropdown button and select `Tankathon Debug Properties`

Which will open up your Launch Profile config window. Select “Godot Executable” and in the `Executable` input box, replace the “YOUR_GODOT_EXE_PATH_HERE” text with the path to your `Godot.exe` that we’ve extracted earlier. 

You should now be able to click the play button, which will load the debug symbols, as well as any breakpoints, and you will see a Godot game window pop up. You can now step through and debug your C# code. 

## Creating A Bot

Inside your `MyTank.cs` Class, you have access to two methods. 

1. The `Setup(...)` method is called the first time the project is loaded, and only called once. You can use this function to set up any initial values you may need for your bot brain. 

2. The `Do(...)` method is called on every frame after the project starts. This is where your brain logic will live. This method runs 60 times per second on average (this also runs on the physics update of the game, meaning that state is deterministic. I.e. it is not susceptible to inherent randomness). 

   Inside this method, you have access to two objects that you can use to your advantage: The `actions` object and the `scoreboard` object. 

### Actions Object

Inside the Actions object you will have access to the available commands you can use to influence your bot’s actions:

#### `Scan()`
Looks at anything in the tanks forward path and tells you what the bot can see

Returns an Entity object detailing what the next thing in the path is, along with some useful information such as distance from the bot.

You can scan for:

- Obstacles
- Bullets
- Tanks

Please note, out of the box, the tanks will have green lines emitted from their “front”. This is a debug representation of the raycast that’s being used internally. This is simply for visual help and is not functional. Also note that this is a crude representation, as the drawn green line can pass through objects, whereas the raycast would not. 

#### `MoveForward()`
Moves the tank forward by the base tank speed.

Returns a bool indicating whether or not the movement was successful

#### `Aim(Rotation)`
Rotates the tank clockwise or counter-clockwise depending on direction input

#### `Fire()`
Fires the cannon! The bullet will fire from the tank body, and travel in a straight line until it hits something, exploding on impact. After a successful shot, a reload countdown begins internally. 

Returns a float detailing how much longer until the cannon is ready in seconds. If the shot was successful, the return value will be the maximum cooldown timer. 

#### `FireCooldown()`
Allows you to check whether or not you can fire the cannon, without actually attempting to using the `Fire()` method. 

Returns a float representing the time in seconds that the cannon has left on its cooldown

#### `stats`
An object representing your tank’s current stats such as position, rotation, and team.

### Scoreboard Object

Inside your scoreboard object, you will have access to view the game’s internal state properties

#### `timeLeft`
An integer representing the match remaining time, in seconds. It might be beneficial for your tank to know how much longer is left in the match.

#### `GetScoreForTeam(TankTeam)`
A method that allows you to see the current score for the game, when given a `TankTeam` enum. You can check your own team’s score by using the `actions.stats.team` prop. 

## Fighting Bots

The project comes with two very basic bots: `DumTank` and `EvilTank`. Both can be found in the `EvilTanks` directory. By default, the project is set up to use the `DumTank` bot. Though, as the name might imply, it’s not the smartest bot ever conceived. 

You can, however, opt for a slightly (read: _slightly_) harder challenge, by changing the opponent to be `EvilTank` instead. 

To do this, open the `GameManager.cs` file, and make the following change: 

From: `redTank.thisTank = new Tankathon.EvilTank.DumTank();`

To: &nbsp;&nbsp;&nbsp;&nbsp; `redTank.thisTank = new Tankathon.EvilTank.EvilTank();`

You can also make further changes by instead instantiating your own tank for both the red and blue sides once it’s got some logic going. This will allow for better iteration of your tank logic and you can be sure to test starting on both the top or bottom sides of the arena! (Note: `EvilTank` is only designed to start on the red side)

### **Happy tanking!**