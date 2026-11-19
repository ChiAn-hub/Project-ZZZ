using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Project_ZZZ.mysql
{
    [SugarTable("shop_product")]
    public class ShopProduct
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public int  UnitCount { get; set; }
        public long Price { get; set; }
        public bool IsActive { get; set; }
    }
}
