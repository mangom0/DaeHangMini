using System;
using System.Collections.Generic;

namespace DaeHang
{
    public enum TileType { Sea, Land, City }

    public struct Point
    {
        public int X, Y; 
        public Point(int x, int y) { X = x; Y = y; }
    }

    public class Map
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TileType[,] Tiles { get; private set; }

        public static Map FromString(string raw)
        {
            raw = raw.Replace("\r\n", "\n").Replace("\r", "\n");
            while (raw.Length > 0 && raw[0] == '\n')
            {
                raw = raw.Substring(1);
            }
            while (raw.Length > 0 && raw[raw.Length - 1] == '\n')
            {
                raw = raw.Substring(0, raw.Length - 1);
            }

            var lines = raw.Split('\n');

            int h = lines.Length;
            int w = 0; 
            for (int i = 0; i < h; i++) 
            {
                if (lines[i].Length > w)
                {
                    w = lines[i].Length;
                }
            }

            var tiles = new TileType[h, w];
            for (int y = 0; y < h; y++)
            {
                string line = lines[y].PadRight(w, '~');
                for (int x = 0; x < w; x++)
                {
                    char c = line[x];
                    tiles[y, x] = (c == '#') ? TileType.Land : (c == '●') ? TileType.City : TileType.Sea;
                }
            }

            return new Map { Width = w, Height = h, Tiles = tiles };
        }

        public bool IsInside(int x, int y) 
        { 
            return x >= 0 && y >= 0 && x < Width && y < Height; 
        }
        public bool CanSail(int x, int y)
        {
            if (!IsInside(x, y)) return false;
            var t = Tiles[y, x];
            return t == TileType.Sea || t == TileType.City;
        }
    }

}
