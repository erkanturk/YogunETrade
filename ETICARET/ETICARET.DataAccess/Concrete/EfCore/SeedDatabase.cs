using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class SeedDatabase
    {
        public static void Seed()
        {
            var context = new DataContext();
            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context.Categories.Count()==0)
                {
                    context.AddRange(Categories);
                }
                if (context.Products.Count()==0)
                {
                    context.AddRange(Products);
                    context.AddRange(ProductCategories);
                }
                context.SaveChanges();
            }
        }
        private static Category[] Categories =
        {
            new Category(){Name="Telefon"},//0
            new Category(){Name="Bilgisayar"},//1
            new Category(){Name="Elektronik"},//2
            new Category(){Name="Ev Gereçleri"}//3
        };
        private static Product[] Products = 
        { 
            new Product(){Name="Samsung Note 8",Price=15000,Images={new Image() { ImageUrl="samsung2.jpg"},new Image() { ImageUrl="samsung.jpg"},new Image() {ImageUrl="samsung4.jpg"},new Image() { ImageUrl="samsung3.jpg"} },Description="<p>Güzel Telefon</p>"},
            new Product(){Name="Samsung Note 8",Price=15000,Images={new Image() { ImageUrl="samsung2.jpg"},new Image() { ImageUrl="samsung.jpg"},new Image() {ImageUrl="samsung4.jpg"},new Image() { ImageUrl="samsung3.jpg"} },Description="<p>Güzel Telefon</p>"},
            new Product(){Name="Samsung Note 8",Price=15000,Images={new Image() { ImageUrl="samsung2.jpg"},new Image() { ImageUrl="samsung.jpg"},new Image() {ImageUrl="samsung4.jpg"},new Image() { ImageUrl="samsung3.jpg"} },Description="<p>Güzel Telefon</p>"},
            new Product(){Name="Samsung Note 8",Price=15000,Images={new Image() { ImageUrl="samsung2.jpg"},new Image() { ImageUrl="samsung.jpg"},new Image() {ImageUrl="samsung4.jpg"},new Image() { ImageUrl="samsung3.jpg"} },Description="<p>Güzel Telefon</p>"}
        };
        private static ProductCategory[] ProductCategories =
        {
            new ProductCategory(){Product=Products[0],Category=Categories[0]},
            new ProductCategory(){Product=Products[1],Category=Categories[2]},
            new ProductCategory(){Product=Products[2],Category=Categories[3]},
            new ProductCategory(){Product=Products[3],Category=Categories[1]},
        };
    }
}
