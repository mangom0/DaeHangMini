using System;
using System.Collections.Generic;


namespace DaeHang
{
    public class Port
    {
        public int Id { get; }
        public string Name { get; private set; }
        public Point Position { get; }
        public Market Market { get; } = new Market();

        public Port(int id, string name, Point position)
        {
            Id = id;
            Name = name;
            Position = position;
        }

        public void Rename(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name.Trim();
            }
        }
    }

}
