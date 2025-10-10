using System;

namespace DaeHang
{
    public class GameClock
    {
        public int Day { get; private set; } = 1;
        public void Advance(int days)
        {
            if (days > 0)
            {
                Day += days;
            }
        }
    }
}
