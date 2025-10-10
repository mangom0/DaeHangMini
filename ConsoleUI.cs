using System;

namespace DaeHang
{
    public static class ConsoleUI
    {
        //타이틀
        public static void ShowTitle()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            Console.WriteLine(MapData.TitleScreen);
            Console.ReadKey(true); // 아무 키 입력 대기
            Console.CursorVisible = true;
        }

        //콘솔 크기
        public static void TryResizeConsole(int targetW, int targetH)
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
            catch { }
        }

        //항해 화면 렌더
        public static void Render(World world, Player player, GameState state)
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < world.Map.Height; y++)
            {
                for (int x = 0; x < world.Map.Width; x++)
                {
                    if (x == player.MapX && y == player.MapY) 
                    { 
                        Console.Write('▼'); 
                        continue; 
                    }
                    var t = world.Map.Tiles[y, x];
                    char ch = (t == TileType.Sea) ? '~' : (t == TileType.Land ? '#' : '●');
                    Console.Write(ch);
                }
                // 오른쪽 잔상 제거
                int rest = Math.Max(0, Console.WindowWidth - world.Map.Width - 1);
                if (rest > 0)
                { 
                    Console.Write(new string(' ', rest)); 
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Mode: {state}   Day {world.Day}   Pos({player.MapX},{player.MapY})   ←↑→↓/WASD 이동, Q 종료");
        }

        //포트 메뉴(출항/교역소/종료)
        public static bool PortMenu(World world, Player player)
        {
            while (true)
            {
                var port = world.GetPortById(player.LocationPortId);
                Console.Clear();
                Console.WriteLine($"[{(port != null ? port.Name : "항구")}]  Day {world.Day}");
                Console.WriteLine();
                Console.WriteLine("[1] 출항");
                Console.WriteLine("[2] 교역소");
                Console.WriteLine("[0] 종료");

                var k = Console.ReadKey(true).Key;
                if (k == ConsoleKey.D1 || k == ConsoleKey.NumPad1)
                {
                    return true;  // 출항
                }
                if (k == ConsoleKey.D2 || k == ConsoleKey.NumPad2)
                {
                    TradePost(world, player);
                }
                else if (k == ConsoleKey.D0 || k == ConsoleKey.NumPad0 || k == ConsoleKey.Q)
                {
                    return false; // 종료
                }
            }
        }

        //교역소(구매/판매: 구매목록·내보유 분리 + 미취급 프리미엄)
        static void TradePost(World world, Player player)
        {
            var port = world.GetPortById(player.LocationPortId);
            if (port == null)
            {
                return;
            }

            while (true)
            {
                // 구매 가능(항구 카탈로그)
                var buyList = new System.Collections.Generic.List<(Good g, int price, StockInfo s)>();
                foreach (var g in world.Goods) 
                {
                    if (port.Market.HasGood(g.Id)) 
                    {
                        buyList.Add((g, BuyPrice(port, g), port.Market.GetStockInfo(g.Id)));
                    }
                }

                // 내 보유
                var sellList = new System.Collections.Generic.List<(Good g, int have, bool inCatalog, int portBuyPrice)>();
                foreach (var g in world.Goods)
                {
                    int have = player.Cargo.GetQty(g.Id);
                    if (have > 0) 
                    {
                        sellList.Add((g, have, port.Market.HasGood(g.Id), BuyPrice(port, g)));
                    }
                }

                Console.Clear();
                Console.WriteLine($"[교역소] {port.Name}   Day {world.Day}");
                Console.WriteLine($"보유골드: {player.Gold}   적재: {player.Cargo.TotalQty}/{player.Ship.CapacityUnits} (남은:{player.RemainingCapacity})");
                Console.WriteLine("================================================================================");

                // 구매
                Console.WriteLine("【구매 가능 품목】  (B <번호> <수량>)");
                if (buyList.Count == 0) Console.WriteLine("  - 이 항구에서는 현재 구매 가능한 품목이 없습니다.");
                else
                {
                    Console.WriteLine("  번호  상품명     매수가   재고/기준");
                    for (int i = 0; i < buyList.Count; i++)
                    {
                        var it = buyList[i];
                        Console.WriteLine($"{i + 1,5}  {it.g.Name.PadRight(8)} {it.price,6}   {it.s.Stock,3}/{it.s.Base,-3}");
                    }
                }

                Console.WriteLine("--------------------------------------------------------------------------------");

                // 판매
                Console.WriteLine("【내 보유 품목】  (S <번호> <수량>)");
                if (sellList.Count == 0) Console.WriteLine("  - 보유 중인 품목이 없습니다.");
                else
                {
                    Console.WriteLine("  번호  상품명     보유  (다음 1개 판매가)  비고");
                    for (int i = 0; i < sellList.Count; i++)
                    {
                        var it = sellList[i];
                        int next = SellUnitPrice(port, it.g, it.have, it.inCatalog); // 미취급=150% 프리미엄
                        string note = it.inCatalog ? "" : "미취급(프리미엄)";
                        Console.WriteLine($"{i + 1,5}  {it.g.Name.PadRight(8)} {it.have,3}    →{next,6}   {note}");
                    }
                }

                Console.WriteLine("================================================================================");
                Console.WriteLine("[B]구매  [S]판매  [0]뒤로");
                Console.Write("입력: ");
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (line.Trim() == "0") 
                { 
                    Console.Clear(); 
                    break; 
                }

                var tok = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tok.Length < 3) 
                { 
                    Pause("형식: B|S <번호> <수량>");
                    continue; 
                }

                char mode = char.ToUpperInvariant(tok[0][0]);
                if (!int.TryParse(tok[1], out int sel) || !int.TryParse(tok[2], out int qty) || sel < 1 || qty <= 0)
                { 
                    Pause("입력을 확인하세요.");
                    continue; 
                }

                if (mode == 'B')
                {
                    if (sel > buyList.Count) 
                    { 
                        Pause("구매 번호 범위를 벗어났습니다."); 
                        continue; 
                    }
                    var it = buyList[sel - 1];
                    long cost = (long)it.price * qty;

                    if (it.s.Stock < qty) 
                    {
                        Pause("재고 부족!"); 
                        continue; 
                    }
                    if (player.Gold < cost) 
                    { 
                        Pause("골드 부족!");
                        continue;
                    }
                    if (player.RemainingCapacity < qty) 
                    {
                        Pause("적재 공간 부족!"); 
                        continue;
                    }

                    if (!port.Market.TryDecreaseStock(it.g.Id, qty)) 
                    { 
                        Pause("잠시 후 다시 시도"); 
                        continue; 
                    }
                    player.Gold -= cost;
                    player.Cargo.Add(it.g.Id, qty);
                    Pause($"{it.g.Name} {qty}개 구매 (-{cost}G)");
                }
                else if (mode == 'S')
                {
                    if (sel > sellList.Count) 
                    {
                        Pause("판매 번호 범위를 벗어났습니다."); 
                        continue; 
                    }
                    var it = sellList[sel - 1];
                    if (it.have < qty) 
                    { 
                        Pause("보유 수량 부족!");
                        continue;
                    }

                    long revenue = ComputeSellRevenue(port, it.g, it.have, qty, it.inCatalog);
                    if (!player.Cargo.Remove(it.g.Id, qty)) 
                    { 
                        Pause("알 수 없는 오류");
                        continue; 
                    }
                    port.Market.IncreaseStock(it.g.Id, qty);
                    player.Gold += revenue;

                    Pause($"{it.g.Name} {qty}개 판매 (+{revenue}G)");
                }
                else
                {
                    Pause("B(구매) 또는 S(판매)만 지원합니다.");
                }
            }
        }

        //가격/정산 보조
        static int BuyPrice(Port port, Good g)
        {
            return port.Market.GetUnitPrice(g.Id, g);
        }

        //(미취급)면 150% 프리미엄, 취급이면 (중복 → 50%)
        static int SellUnitPrice(Port port, Good g, int playerQtyBefore, bool inCatalog)
        {
            int buy = BuyPrice(port, g);
            if (!inCatalog) 
            {
                return (int)Math.Round(buy * 1.5, MidpointRounding.AwayFromZero);
            }
            return (int)Math.Floor(buy / 2.0);
        }

        static long ComputeSellRevenue(Port port, Good g, int playerQtyBefore, int sellQty, bool inCatalog)
        {
            int buy = BuyPrice(port, g);
            if (!inCatalog)
            {
                int unit = (int)Math.Round(buy * 1.5, MidpointRounding.AwayFromZero);
                return (long)unit * sellQty;
            }
            else
            {
                int unit = (int)Math.Floor(buy / 2.0);
                return (long)unit * sellQty;
            }
        }

        static void Pause(string msg)
        {
            Console.WriteLine(msg);
            Console.Write("계속하려면 Enter...");
            Console.ReadLine();
        }
    }
}

