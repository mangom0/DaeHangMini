using System;
using System.Collections.Generic;

namespace DaeHang
{
    using System.Collections.Generic;

    public class World
    {
        public Map Map { get; private set; }

        public GameClock Clock { get; } = new GameClock();
        public PortRegistry PortReg { get; } = new PortRegistry();
        public int Day => Clock.Day;
        public List<Port> Ports => PortReg.Ports;
        public List<Good> Goods { get; } = new List<Good>();

        public void SetMap(Map m) { Map = m; }
        public void AdvanceDays(int d) 
        { 
            Clock.Advance(d); 
        }

        public void AutoAssignPortsFromCities() 
        { 
            PortReg.AutoAssignFromMap(Map); 
        }
        public Port FindPortAt(int x, int y) 
        { 
            return PortReg.FindAt(x, y); 
        }
        public Port GetPortById(int id) 
        { 
            return PortReg.GetById(id); 
        }
        public bool TryRenamePortAt(int x, int y, string name) 
        {
            return PortReg.TryRenameAt(x, y, name);
        }
        public bool TryRenamePortById(int id, string name)
        { 
            return PortReg.TryRenameById(id, name); 
        }

        public static World FromAscii(string rawMap)
        {
            var w = new World();
            w.SetMap(Map.FromString(rawMap));
            return w;
        }
    }


}
