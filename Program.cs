using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaeHang
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8; // ● 같은 유니코드 표시

            // 1) MAP → World
            World world = World.FromAscii(MapData.World1);

            // 2) 콘솔 크기(가능 범위 내) 맞추기
            try
            {
                int w = Math.Min(world.Map.Width + 2, Console.LargestWindowWidth);
                int h = Math.Min(world.Map.Height + 2, Console.LargestWindowHeight);
                if (Console.BufferWidth < w) Console.BufferWidth = w;
                if (Console.BufferHeight < h) Console.BufferHeight = h;
                if (Console.WindowWidth < w) Console.WindowWidth = w;
                if (Console.WindowHeight < h) Console.WindowHeight = h;
            }
            catch { }

            // 3) 맵 한 번 그리기
            Console.Clear();
            for (int y = 0; y < world.Map.Height; y++)
            {
                for (int x = 0; x < world.Map.Width; x++)
                {
                    var t = world.Map.Tiles[y, x];
                    char ch = (t == TileType.Sea) ? '~' : (t == TileType.Land ? '#' : '●');
                    Console.Write(ch);
                }
                Console.WriteLine();
            }

            Console.WriteLine($"\nDay {world.Day}  (맵 크기: {world.Map.Width}×{world.Map.Height})  아무 키나 누르면 계속");
            Console.ReadKey(true);
        }


    }
}
