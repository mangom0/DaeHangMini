using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaeHang
{
    public class Player
    {
        public string Name { get; }
        public long Gold { get; set; }
        public int MapX { get; set; }  // 현재 배 위치 X
        public int MapY { get; set; }  // 현재 배 위치 Y
        public int LocationPortId { get; set; } // 항구 안일 때 의미 있음(0이면 항해 중)

        public Player(string name, long gold, int startX, int startY)
        {
            Name = name; Gold = gold; MapX = startX; MapY = startY;
            LocationPortId = 0;
        }
    }

}
