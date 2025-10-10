using System;
using System.Collections.Generic;


namespace DaeHang
{
    public class Ship
    {
        public int CapacityUnits { get; }   // 총 적재 한도(유닛)
        public Ship(int capacityUnits) { CapacityUnits = capacityUnits; }
    }

    public class CargoHold
    {
        private readonly Dictionary<int, int> _items = new Dictionary<int, int>();
        public int TotalQty 
        {
            get 
            { 
                int s = 0;
                foreach (var v in _items.Values)
                {
                    s += v;
                }
                return s;
            } 
        }

        public int GetQty(int goodId)
        {
            int q;
            return _items.TryGetValue(goodId, out q) ? q : 0;
        }
        public void Add(int goodId, int qty)
        {
            int cur = GetQty(goodId);
            _items[goodId] = cur + qty;
        }
        public bool Remove(int goodId, int qty)
        {
            int cur = GetQty(goodId);
            if (cur < qty)
            {
                return false;
            }
            cur -= qty;
            if (cur == 0)
            {
                _items.Remove(goodId);
            }
            else
            {
                _items[goodId] = cur;
            }
            return true;
        }
    }

    public class Player
    {
        public string Name { get; }
        public long Gold { get; set; }

        public int MapX { get; set; }
        public int MapY { get; set; }
        public int LocationPortId { get; set; }

        public Ship Ship { get; }             
        public CargoHold Cargo { get; } = new CargoHold();
        public int RemainingCapacity { get { return Ship.CapacityUnits - Cargo.TotalQty; } }

        public Player(string name, long gold, int startX, int startY, int shipCapacity = 5)
        {
            Name = name; 
            Gold = gold; 
            MapX = startX; 
            MapY = startY;
            LocationPortId = 0;
            Ship = new Ship(shipCapacity);   
        }
    }
}
