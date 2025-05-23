﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Entity
{
    public class BasketItem
    {
        public int BasketItemId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int  Quantity { get; set; }
        public string PictureUrl { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
    }
}
