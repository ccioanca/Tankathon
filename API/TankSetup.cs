namespace Tankathon.API
{
    public class TankSetup : ITankSetup
    {
        public string name { get; set; } = "MyTank";
        public string primaryColor { get; set; } = "#000000";
        public string secondaryColor { get; set; } = "#ffffff";
        public ITankAttributes attributes { get;  set; }
    }

    public class TankAttributes : ITankAttributes
    {
        public int moveSpeed { get; set; }
        public int rotationSpeed { get; set; }
        public int bulletSpeed { get; set; }
        public int reloadSpeed { get; set; }
    }
}
