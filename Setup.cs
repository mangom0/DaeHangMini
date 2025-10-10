using System;
using System.Collections.Generic;

namespace DaeHang
{
    public static class Setup
    {
        public static void SetupPortNames(World world)
        {
            world.TryRenamePortAt(13, 4, "바스라");
            world.TryRenamePortAt(27, 9, "호르무즈");
            world.TryRenamePortAt(30, 16, "무스카트");
            world.TryRenamePortAt(51, 17, "디부");
            world.TryRenamePortAt(56, 24, "고아");
            world.TryRenamePortAt(60, 35, "캘리컷");
            world.TryRenamePortAt(63, 27, "마드리스");
            world.TryRenamePortAt(73, 19, "마술리파트남");
        }
        public static void SeedGoods(World world)
        {
            world.Goods.Clear();
            world.Goods.Add(new Good(1, "쇠고기", 700));
            world.Goods.Add(new Good(2, "초석", 900));
            world.Goods.Add(new Good(3, "치즈", 900));
            world.Goods.Add(new Good(4, "융단", 1500));
            world.Goods.Add(new Good(5, "야자유", 400));
            world.Goods.Add(new Good(6, "사향", 2000));
            world.Goods.Add(new Good(7, "면화", 600));
            world.Goods.Add(new Good(8, "유황", 600));
            world.Goods.Add(new Good(9, "어육", 500));
            world.Goods.Add(new Good(10, "꿀", 900));
            world.Goods.Add(new Good(11, "마직물", 600));
            world.Goods.Add(new Good(12, "쌀", 500));
            world.Goods.Add(new Good(13, "설탕", 600));
            world.Goods.Add(new Good(14, "용연향", 1000));
            world.Goods.Add(new Good(15, "후추", 1500));
            world.Goods.Add(new Good(16, "청금석", 2000));
        }

        public static void SetupPortCatalog(World world)
        {
            var basra = FindPort(world, "바스라");
            var hormuz = FindPort(world, "호르무즈");
            var muscat = FindPort(world, "무스카트");
            var dibu = FindPort(world, "디부");
            var goa = FindPort(world, "고아");
            var calicut = FindPort(world, "캘리컷");
            var madras = FindPort(world, "마드리스");
            var masuli = FindPort(world, "마술리파트남");

            // 바스라: 초석/치즈/유황/쌀
            Set(basra, G(world, "초석"), 920, 20, 20);
            Set(basra, G(world, "치즈"), 940, 12, 12);
            Set(basra, G(world, "유황"), 620, 14, 14);
            Set(basra, G(world, "쌀"), 520, 18, 18);

            // 호르무즈: 융단/사향/후추/야자유
            Set(hormuz, G(world, "융단"), 1520, 8, 9);
            Set(hormuz, G(world, "사향"), 2050, 4, 5);
            Set(hormuz, G(world, "후추"), 1600, 7, 8);
            Set(hormuz, G(world, "야자유"), 420, 16, 16);

            // 무스카트: 쇠고기/초석/면화/유황
            Set(muscat, G(world, "쇠고기"), 720, 18, 18);
            Set(muscat, G(world, "초석"), 910, 12, 12);
            Set(muscat, G(world, "면화"), 610, 14, 14);
            Set(muscat, G(world, "유황"), 590, 10, 10);

            // 디부: 후추/유황/치즈/설탕
            Set(dibu, G(world, "후추"), 1550, 10, 10);
            Set(dibu, G(world, "유황"), 610, 14, 14);
            Set(dibu, G(world, "치즈"), 930, 9, 10);
            Set(dibu, G(world, "설탕"), 620, 15, 15);

            // 고아: 후추/설탕/치즈/야자유
            Set(goa, G(world, "후추"), 1500, 12, 12);
            Set(goa, G(world, "설탕"), 610, 14, 14);
            Set(goa, G(world, "치즈"), 920, 10, 10);
            Set(goa, G(world, "야자유"), 410, 18, 18);

            // 캘리컷: 후추/면화/쌀/융단
            Set(calicut, G(world, "후추"), 1480, 18, 18);
            Set(calicut, G(world, "면화"), 590, 12, 12);
            Set(calicut, G(world, "쌀"), 510, 18, 18);
            Set(calicut, G(world, "융단"), 1490, 8, 9);

            // 마드리스: 면화/마직물/설탕/쌀
            Set(madras, G(world, "면화"), 580, 18, 18);
            Set(madras, G(world, "마직물"), 610, 14, 14);
            Set(madras, G(world, "설탕"), 630, 12, 12);
            Set(madras, G(world, "쌀"), 520, 20, 20);

            // 마술리파트남: 면화/융단/후추/청금석(희귀)
            Set(masuli, G(world, "면화"), 600, 20, 20);
            Set(masuli, G(world, "융단"), 1510, 10, 10);
            Set(masuli, G(world, "후추"), 1520, 9, 10);
            Set(masuli, G(world, "청금석"), 2050, 3, 4);
        }

        public static bool FindStart(World world, out int x, out int y)
        {
            if (FindFirst(world, TileType.City, out x, out y))
            {
                return true;
            }
            return FindFirst(world, TileType.Sea, out x, out y);
        }


        private static Port FindPort(World world, string name)
        {
            for (int i = 0; i < world.Ports.Count; i++)
            {
                if (world.Ports[i].Name == name)
                {
                    return world.Ports[i];
                }
            }
            return null;
        }

        private static Good G(World world, string name)
        {
            for (int i = 0; i < world.Goods.Count; i++)
            {
                if (world.Goods[i].Name == name)
                {
                    return world.Goods[i];
                }
            }
            return null;
        }

        private static void Set(Port port, Good good, int unitPrice, int stock, int baseStock)
        {
            if (port == null || good == null)
            {
                return;
            }
            port.Market.SetPrice(good.Id, unitPrice);
            port.Market.SetStock(good.Id, stock, baseStock);
        }

        private static bool FindFirst(World world, TileType t, out int x, out int y)
        {
            for (int j = 0; j < world.Map.Height; j++)
            {
                for (int i = 0; i < world.Map.Width; i++)
                {
                    if (world.Map.Tiles[j, i] == t)
                    {
                        x = i;
                        y = j;
                        return true;
                    }
                }
            }
            x = y = 0;
            return false;
        }
    }
}
