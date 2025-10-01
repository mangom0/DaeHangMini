using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaeHang
{
    // ===== [Step 1] 기본 타입 & Map =====
    public enum TileType { Sea, Land, City }

    public struct Point
    {
        public int X, Y;
        public Point(int x, int y) { X = x; Y = y; }
    }
    public class World
    {
        //맵보관
        public Map Map { get; private set; }
        //날짜 관리
        public int Day { get; private set; } = 1;
        //맵 주입
        public void SetMap(Map m) { Map = m; }
        //시간 진행
        public void AdvanceDays(int d)
        {
            if (d > 0) Day += d;
            // (나중) 시장 재고 회귀/이벤트 훅을 여기에 넣을 거야
        }

        // 편의: MAP 문자열에서 World 만들기
        public static World FromAscii(string rawMap)
        {
            var w = new World();
            w.SetMap(Map.FromString(rawMap));
            return w;
        }
    }

    public class Map
    {
        //격자 크기
        public int Width { get; private set; }
        public int Height { get; private set; }
        //타일데이터
        public TileType[,] Tiles { get; private set; }
        // 문자열(MAP) → 타일 그리드로 바꾸기
        public static Map FromString(string raw)
        {
            // 줄 정리
            raw = raw.Replace("\r\n", "\n").Replace("\r", "\n");
            while (raw.StartsWith("\n")) raw = raw.Substring(1);
            while (raw.EndsWith("\n")) raw = raw.Substring(0, raw.Length - 1);

            // 공통 들여쓰기 제거(코드 안에서 들여써도 원형 유지)
            var lines = raw.Split('\n');
            int minIndent = int.MaxValue;
            for (int i = 0; i < lines.Length; i++)
            {
                var l = lines[i];
                if (l.Trim().Length == 0) continue;
                int c = 0; while (c < l.Length && l[c] == ' ') c++;
                if (c < minIndent) minIndent = c;
            }
            if (minIndent == int.MaxValue) minIndent = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                var l = lines[i];
                int trim = Math.Min(minIndent, l.Length);
                lines[i] = l.Substring(trim);
            }

            // 직사각형 패딩
            int h = lines.Length;
            int w = 0; for (int i = 0; i < h; i++) if (lines[i].Length > w) w = lines[i].Length;

            var tiles = new TileType[h, w];
            for (int y = 0; y < h; y++)
            {
                string line = lines[y].PadRight(w, '~'); // 짧은 부분은 바다로 채움
                for (int x = 0; x < w; x++)
                {
                    char c = line[x];
                    tiles[y, x] =
                        (c == '#') ? TileType.Land :
                        (c == '●') ? TileType.City :
                                     TileType.Sea; // 기본은 바다
                }
            }

            return new Map { Width = w, Height = h, Tiles = tiles };
        }

        // 나중 이동에 쓸 도우미들(오늘은 안 써도 됨)
        public bool IsInside(int x, int y) { return x >= 0 && y >= 0 && x < Width && y < Height; }
        public bool CanSail(int x, int y)
        {
            if (!IsInside(x, y)) return false;
            var t = Tiles[y, x];
            return t == TileType.Sea || t == TileType.City;
        }
    }

}
