using System;
using System.Collections.Generic;

namespace DaeHang
{
    public class Good
    {
        public int Id { get; }
        public string Name { get; }
        public int BasePrice { get; }

        public Good(int id, string name, int basePrice)
        {
            Id = id; 
            Name = name; 
            BasePrice = basePrice;
        }
    }

    public struct StockInfo
    {
        public int Stock;
        public int Base;
    }

    public class Market
    {
        private readonly Dictionary<int, StockInfo> _stocks = new Dictionary<int, StockInfo>();

        // 항구별 가격 카탈로그(판매 메뉴)
        private readonly Dictionary<int, int> _prices = new Dictionary<int, int>(); // goodId -> unit price

        public void SetStock(int goodId, int stock, int @base)
        {
            _stocks[goodId] = new StockInfo { Stock = stock, Base = @base };
        }

        public StockInfo GetStockInfo(int goodId)
        {
            StockInfo s;
            if (_stocks.TryGetValue(goodId, out s))
            {
                return s;
            }
            return new StockInfo { Stock = 0, Base = 0 };
        }

        // 항구별 가격/메뉴 설정 & 조회
        public void SetPrice(int goodId, int unitPrice)
        {
            _prices[goodId] = unitPrice;
        }

        public bool HasGood(int goodId) => _prices.ContainsKey(goodId);

        public int GetUnitPrice(int goodId, Good fallback)
        {
            int p;
            if (_prices.TryGetValue(goodId, out p))
            {
                return p;
            }
            return fallback.BasePrice; // 메뉴에 없으면 기본가(보통 구매는 막지만, 참고용)
        }

        // 재고 증감 (구매/판매에 사용)
        public bool TryDecreaseStock(int goodId, int qty)
        {
            var s = GetStockInfo(goodId);
            if (s.Stock < qty)
            {
                return false;
            }
            s.Stock -= qty;
            _stocks[goodId] = s; 
            return true;
        }

        public void IncreaseStock(int goodId, int qty)
        {
            var s = GetStockInfo(goodId);
            s.Stock += qty; 
            _stocks[goodId] = s;
        }
    }
}
