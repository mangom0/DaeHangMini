using System;
using System.Collections.Generic;

namespace DaeHang
{
    public class PortRegistry
    {
        private readonly List<Port> _ports = new List<Port>();
        private readonly Dictionary<int, Port> _byId = new Dictionary<int, Port>();
        private readonly Dictionary<long, int> _posToId = new Dictionary<long, int>();

        public List<Port> Ports { get { return _ports; } }

        public void Clear()
        {
            _ports.Clear();
            _byId.Clear();
            _posToId.Clear();
        }

        public void Add(Port p)
        {
            _ports.Add(p);
            _byId[p.Id] = p;
            _posToId[Key(p.Position.X, p.Position.Y)] = p.Id;
        }

        public void AutoAssignFromMap(Map map)
        {
            Clear();
            int id = 1;
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (map.Tiles[y, x] == TileType.City)
                    {
                        Add(new Port(id++, "Port" + (id - 1), new Point(x, y)));
                    }
                }
            }
        }

        public Port GetById(int id)
        {
            Port p;
            return _byId.TryGetValue(id, out p) ? p : null;
        }

        public Port FindAt(int x, int y)
        {
            int id;
            if (_posToId.TryGetValue(Key(x, y), out id))
            {
                return GetById(id);
            }
            return null;
        }

        public bool TryRenameAt(int x, int y, string name)
        {
            var p = FindAt(x, y);
            if (p == null)
            {
                return false;
            }
            p.Rename(name); 
            return true;
        }

        public bool TryRenameById(int id, string name)
        {
            var p = GetById(id);
            if (p == null)
            {
                return false;
            }
            p.Rename(name);
            return true;
        }

        private static long Key(int x, int y) 
        { 
            unchecked 
            { 
                return ((long)y << 32) | (uint)x; 
            } 
        }
    }
}
