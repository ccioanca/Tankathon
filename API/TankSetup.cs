using System;

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
        private int _moveSpeed = 0;
        private int _rotationSpeed = 0;
        private int _bulletSpeed = 0;
        private int _reloadSpeed = 0;
        public int moveSpeed { get => _moveSpeed; set => _moveSpeed = (value >= 0 && value <= 10) ? _moveSpeed = value : throw new ArgumentOutOfRangeException("Move Speed out of allowed bounds (0-10)"); }
        public int rotationSpeed { get => _rotationSpeed; set => _rotationSpeed = (value >= 0 && value <= 10) ? _rotationSpeed = value : throw new ArgumentOutOfRangeException("Rotation Speed out of allowed bounds (0-10)"); }
        public int bulletSpeed { get => _bulletSpeed; set => _bulletSpeed = (value >= 0 && value <= 10) ? _bulletSpeed = value : throw new ArgumentOutOfRangeException("Bullet Speed out of allowed bounds (0-10)"); }
        public int reloadSpeed { get => _reloadSpeed; set => _reloadSpeed = (value >= 0 && value <= 10) ? _reloadSpeed = value : throw new ArgumentOutOfRangeException("Reload Speed out of allowed bounds (0-10)"); }
    }
}
