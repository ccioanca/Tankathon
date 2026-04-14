using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankathon.API
{
    public interface ITankSetup
    {
        public string name { set; }
        public string primaryColor { set; }
        public string secondaryColor { set; }
        public ITankAttributes attributes { get; set; }
    }

    public interface ITankAttributes
    {
        public int moveSpeed { get; set; }
        public int rotationSpeed { get; set; }
        public int bulletSpeed { get; set; }
        public int reloadSpeed { get; set; }
    }
}
