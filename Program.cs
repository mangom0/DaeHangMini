using System;
using System.Text;


namespace DaeHang
{
    public enum GameState { Sailing, InPort }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            ConsoleUI.ShowTitle(); 

            var world = World.FromAscii(MapData.World1);
            world.AutoAssignPortsFromCities();
            Setup.SetupPortNames(world);
            Setup.SeedGoods(world);
            Setup.SetupPortCatalog(world);

            Setup.FindStart(world, out int sx, out int sy);
            var player = new Player("주인공", 5000, sx, sy, shipCapacity: 5);

            //ConsoleUI.TryResizeConsole(world.Map.Width + 3, world.Map.Height + 3);
            var state = GameState.Sailing;
            bool running = true;

            ConsoleUI.Render(world, player, state);

            while (running)
            {
                var key = Console.ReadKey(true).Key;
                int dx = 0; 
                int dy = 0;
                switch (key)
                {
                    case ConsoleKey.LeftArrow: 
                    case ConsoleKey.A: 
                        dx = -1; 
                        break;
                    case ConsoleKey.RightArrow: 
                    case ConsoleKey.D: 
                        dx = 1; 
                        break;
                    case ConsoleKey.UpArrow: 
                    case ConsoleKey.W: 
                        dy = -1; 
                        break;
                    case ConsoleKey.DownArrow: 
                    case ConsoleKey.S: 
                        dy = 1; 
                        break;
                    case ConsoleKey.Q: 
                        running = false;
                        continue;
                    default: 
                        continue;
                }

                int nx = player.MapX + dx;
                int ny = player.MapY + dy;
                if (!world.Map.CanSail(nx, ny))
                {
                    continue;
                }

                player.MapX = nx;
                player.MapY = ny;
                world.AdvanceDays(1);
                ConsoleUI.Render(world, player, state);

                // "항해 상태"일 때만 항구 진입
                if (state == GameState.Sailing && world.Map.Tiles[player.MapY, player.MapX] == TileType.City)
                {
                    var port = world.FindPortAt(player.MapX, player.MapY);
                    player.LocationPortId = (port != null) ? port.Id : 0;

                    Console.Clear();
                    state = GameState.InPort;

                    bool backToSea = ConsoleUI.PortMenu(world, player); // 여기서 '1' 누르면 true
                    if (!backToSea) 
                    { 
                        running = false;
                        break; 
                    }
                    // 출항
                    player.LocationPortId = 0;
                    state = GameState.Sailing;  //다시 항해 상태로
                    Console.Clear();
                    ConsoleUI.Render(world, player, state);
                }
            }
            Console.CursorVisible = true;
        }
    }
}
