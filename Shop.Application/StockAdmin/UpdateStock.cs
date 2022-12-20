﻿using Shop.Domain.Infrastructure;
using Shop.Domain.Models;

namespace Shop.Application.StockAdmin
{
    public class UpdateStock
    {
        private IStockManager _stockManager;

        public UpdateStock(IStockManager stockManager)
        {
            _stockManager = stockManager;
        }

        public async Task<Response> Do(Request request)
        {
            var stockList = new List<Stock>();

            foreach(var stock in request.Stock)
            {
                stockList.Add(new Stock{
                    Id = stock.Id,
                    Description = stock.Description,
                    Qty = stock.Qty,
                    ProductId = stock.ProductId,
                });
            }

            _stockManager.UpdateStockRange(stockList);

            return new Response
            {
               Stock = request.Stock
            };
        }

        public class StockViewModel
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string Description { get; set; }
            public int Qty { get; set; }
        }

        public class Request
        {
            public IEnumerable<StockViewModel> Stock { get; set; }  
        }

        public class Response
        {
            public IEnumerable<StockViewModel> Stock { get; set; }
        }
    }
}
