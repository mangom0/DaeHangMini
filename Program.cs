using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaeHang
{
    enum GameState { Sailing, InPort }
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            // 1) 문자열 맵 → World
            World world = World.FromAscii(MapData.World1);
            world.AutoAssignPortsFromCities();
            world.TryRenamePortAt(13, 4, "바스라");
            world.TryRenamePortAt(27, 9, "호르무즈");
            world.TryRenamePortAt(30, 16, "무스카트");
            world.TryRenamePortAt(51, 17, "디부");
            world.TryRenamePortAt(56, 24, "고아");
            world.TryRenamePortAt(60, 35, "캘리컷");
            world.TryRenamePortAt(63, 27, "마드리스");
            world.TryRenamePortAt(73, 19, "마술리파트남");
            DumpPorts(world);

            // 기존: px/py 사용 → 변경: Player 생성
            int startX, startY;
            if (!FindFirst(world, TileType.City, out startX, out startY))
                FindFirst(world, TileType.Sea, out startX, out startY);

            var player = new Player("주인공", 5000, startX, startY);


            // 2) 콘솔 크기(가능 범위 내) 맞추기
            TryResizeConsole(world.Map.Width + 2, world.Map.Height + 3);

            GameState state = GameState.Sailing;
            bool running = true;

            // 첫 렌더
            Render(world, player, state);

            // 4) 메인 루프
            while (running)
            {
                switch (state)
                {
                    case GameState.Sailing:
                        {
                            var key = Console.ReadKey(true).Key;
                            int dx = 0, dy = 0;
                            switch (key)
                            {
                                case ConsoleKey.LeftArrow: case ConsoleKey.A: dx = -1; break;
                                case ConsoleKey.RightArrow: case ConsoleKey.D: dx = 1; break;
                                case ConsoleKey.UpArrow: case ConsoleKey.W: dy = -1; break;
                                case ConsoleKey.DownArrow: case ConsoleKey.S: dy = 1; break;
                                case ConsoleKey.Q: running = false; continue;
                                default: continue;
                            }

                            int nx = player.MapX + dx, ny = player.MapY + dy;
                            if (!world.Map.CanSail(nx, ny)) continue;

                            player.MapX = nx; player.MapY = ny;
                            world.AdvanceDays(1);

                            // 도시면 InPort 전환 + 플레이어 LocationPortId 설정
                            if (world.Map.Tiles[player.MapY, player.MapX] == TileType.City)
                            {
                                var port = world.FindPortAt(player.MapX, player.MapY);
                                player.LocationPortId = port != null ? port.Id : 0;
                                state = GameState.InPort;
                            }

                            Render(world, player, state);

                            break;
                        }

                    case GameState.InPort:
                        {
                            // 간단한 항구 메뉴
                            InPortMenu(world, player, ref state, ref running);
                            Render(world, player, state);
                            break;
                        }
                }
            }

            Console.CursorVisible = true;
        }

        // ====== 항구 메뉴 ======
        static void InPortMenu(World world, Player player, ref GameState state, ref bool running)
        {
            int menuTop = world.Map.Height + 1;
            var port = world.GetPortById(player.LocationPortId);
            string portName = port != null ? port.Name : "Unknown Port";

            while (state == GameState.InPort && running)
            {
                Console.SetCursorPosition(0, menuTop);
                ClearLine();
                Console.Write($"입항: {portName}  좌표({player.MapX},{player.MapY})  Day {world.Day}   [1] 출항   [0] 종료  ");

                var k = Console.ReadKey(true).Key;
                if (k == ConsoleKey.D1 || k == ConsoleKey.NumPad1)
                {
                    state = GameState.Sailing;
                    player.LocationPortId = 0; // 항해로 복귀
                    break;
                }
                else if (k == ConsoleKey.D0 || k == ConsoleKey.NumPad0 || k == ConsoleKey.Q)
                {
                    running = false;
                    break;
                }
            }

            Console.SetCursorPosition(0, menuTop);
            ClearLine();
        }

        // ====== 렌더링: 맵 + 배 + 상태 ======
        static void Render(World world, Player player, GameState state)
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < world.Map.Height; y++)
            {
                for (int x = 0; x < world.Map.Width; x++)
                {
                    if (x == player.MapX && y == player.MapY) { Console.Write('▼'); continue; }
                    var t = world.Map.Tiles[y, x];
                    char ch = (t == TileType.Sea) ? '~' : (t == TileType.Land ? '#' : '●');
                    Console.Write(ch);
                }
                int rest = Math.Max(0, Console.WindowWidth - world.Map.Width - 1);
                if (rest > 0) Console.Write(new string(' ', rest));
                Console.WriteLine();
            }

            // 상태줄: InPort면 항구 이름 표시
            string portText = "";
            if (state == GameState.InPort && player.LocationPortId > 0)
            {
                var p = world.GetPortById(player.LocationPortId);
                if (p != null) portText = $"  Port: {p.Name}";
            }
            Console.WriteLine($"Mode: {state}   Day {world.Day}   Pos({player.MapX},{player.MapY}){portText}   ←↑→↓/WASD 이동, Q 종료");
        }


        // ====== 유틸 ======
        static bool FindFirst(World world, TileType t, out int x, out int y)
        {
            for (int j = 0; j < world.Map.Height; j++)
                for (int i = 0; i < world.Map.Width; i++)
                    if (world.Map.Tiles[j, i] == t) { x = i; y = j; return true; }
            x = y = 0; return false;
        }

        static void TryResizeConsole(int targetW, int targetH)
        {
            try
            {
                int w = Math.Min(targetW, Console.LargestWindowWidth);
                int h = Math.Min(targetH, Console.LargestWindowHeight);
                if (Console.BufferWidth < w) Console.BufferWidth = w;
                if (Console.BufferHeight < h) Console.BufferHeight = h;
                if (Console.WindowWidth < w) Console.WindowWidth = w;
                if (Console.WindowHeight < h) Console.WindowHeight = h;
            }
            catch { /* 플랫폼/권한 따라 실패 가능 → 무시 */ }
        }

        static void ClearLine()
        {
            int width = Console.WindowWidth;
            Console.Write(new string(' ', Math.Max(0, width - 1)));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
        static void DumpPorts(World world)
        {
            Console.WriteLine("== Ports (X,Y / Id:Name) ==");
            foreach (var p in world.Ports)
                Console.WriteLine($"({p.Position.X},{p.Position.Y})  Id={p.Id}  {p.Name}");
            Console.WriteLine("================================");
        }
    }
}
