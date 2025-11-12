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
            new Category() {Name="Telefon"},//0
            new Category() {Name="Bilgisayar"},//1
            new Category() {Name="Elektronik"},//2
            new Category() {Name="Ev Gereçleri"}//3

        };

        private static Product[] Products = {
             new Product(){ Name = "Samsung Note 8" , Price = 15000, Images = { new Image() {ImageUrl = "samsung.jpg" },  new Image() {ImageUrl = "samsung2.jpg" }, new Image() {ImageUrl = "samsung3.jpg" }, new Image() {ImageUrl = "samsung4.jpg" } },Description ="<p>Güzel telefon</p>" },
             new Product(){ Name = "Samsung Note 8" , Price = 25000, Images = { new Image() {ImageUrl = "samsung5.jpg" },  new Image() {ImageUrl = "samsung6.jpg" }, new Image() {ImageUrl = "samsung7.jpg" }, new Image() {ImageUrl = "samsung8.jpg" } },Description ="<p>Güzel telefon</p>" },
             new Product(){ Name = "Casper VIA X40" , Price = 9800, Images = { new Image() {ImageUrl = "casper1.jpg" },  new Image() {ImageUrl = "casper2.jpg" }, new Image() {ImageUrl = "casper3.jpg" }, new Image() {ImageUrl = "casper4.jpg" } },Description ="<p>256 GB - 8GB RAM - 6.67 inç</p>" },
             new Product(){ Name = "General Mobile Era 50" , Price = 11000, Images = { new Image() {ImageUrl = "gm1.jpg" }, new Image() {ImageUrl = "gm2.jpg" }, new Image() {ImageUrl = "gm3.jpg" }, new Image() {ImageUrl = "gm4.jpg" } },Description ="<p>256 GB - 12 GB RAM - 6.78 inç</p>" },
             new Product(){ Name = "Huawei Nova 13 Pro" , Price = 33000, Images = { new Image() {ImageUrl = "huawei1.jpg" }, new Image() {ImageUrl = "huawei2.jpg"}, new Image() {ImageUrl = "huawei3.jpg" }, new Image() {ImageUrl = "huawei4.jpg" } },Description ="<p>512 GB - 12 GB RAM - 6.76 inç</p>" },
             new Product(){ Name = "Oppo Reno 14", Price = 27000, Images = { new Image() {ImageUrl = "oppo1.jpg" }, new Image() {ImageUrl = "oppo2.jpg" }, new Image() {ImageUrl = "oppo3.jpg" }, new Image() {ImageUrl = "oppo4.jpg" } },Description ="<p>256 GB - 12 GB RAM - 6.57 inç</p>" },
             new Product(){ Name = "Panasonic TU 110", Price = 2500, Images = { new Image() {ImageUrl = "panasonic1.jpg"}, new Image() {ImageUrl = "panasonic2.jpg"}, new Image() {ImageUrl = "panasonic3.jpg"}, new Image() {ImageUrl = "panasonic4.jpg"} },Description ="<p>64 MB - 256 MB RAM - 2.40 inç</p>" },
             new Product(){ Name = "Poco X6 Pro", Price = 24000, Images = { new Image() {ImageUrl = "poco1.jpg" }, new Image() {ImageUrl = "poco2.jpg" }, new Image() {ImageUrl = "poco3.jpg" }, new Image() {ImageUrl = "poco4.jpg" } },Description ="<p>512 GB - 12 GB RAM - 6.67 inç</p>" },
             new Product(){ Name = "Redmi 10", Price = 9000, Images = { new Image() {ImageUrl = "redmi1.jpg" }, new Image() {ImageUrl = "redmi2.jpg" }, new Image() {ImageUrl = "redmi3.jpg" }, new Image() {ImageUrl = "redmi4.jpg" } },Description ="<p>64 GB - 4 GB RAM - 6.50 inç</p>" },
             new Product(){ Name = "Samsung Galaxy A16" , Price = 9100, Images = { new Image() {ImageUrl = "samsung_galaxy1.jpg" }, new Image() {ImageUrl = "samsung_galaxy2.jpg"  }, new Image() {ImageUrl = "samsung_galaxy3.jpg"}, new Image() {ImageUrl = "samsung_galaxy4.jpg" } },Description ="<p>128 GB - 6GB RAM - 6.71 inç</p>" },
             new Product(){ Name = "Samsung S21 FE" , Price = 19000, Images = { new Image() {ImageUrl = "samsung_s1.jpg" },  new Image() {ImageUrl = "samsung_s2.jpg"  }, new Image() {ImageUrl = "samsung_s3.jpg"  }, new Image() {ImageUrl = "samsung_s4.jpg"  } },Description ="<p>128 GB - 8GB RAM - 6.45 inç</p>" },
             new Product(){ Name = "Xiaomi Note 14 Pro", Price = 17900, Images = { new Image() {ImageUrl = "xiaomi_redmi1.jpg" }, new Image() {ImageUrl = "xiaomi_redmi2.jpg" }, new Image() {ImageUrl = "xiaomi_redmi3.jpg" }, new Image() {ImageUrl = "xiaomi_redmi4.jpg" } },Description ="<p>512 GB - 12 GB RAM - 6.67 inç</p>" },

             new Product(){ Name = "Acer Nitro ANV16", Price = 65000, Images = { new Image() {ImageUrl = "acer_pc1.jpg" }, new Image() {ImageUrl = "acer_pc2.jpg" }, new Image() {ImageUrl = "acer_pc3.jpg" }, new Image() {ImageUrl = "acer_4.jpg" } },Description ="<p>Nvidia GeForce RTX 4060 - 1TB SSD - 16 inç</p>" },
             new Product(){ Name = "Asus Vivobook 15", Price = 16300, Images = { new Image() {ImageUrl = "asus1.jpg" }, new Image() {ImageUrl = "asus2.jpg" }, new Image() {ImageUrl = "asus3.jpg" }, new Image() {ImageUrl = "asus4.jpg" } },Description ="<p>İntel Graphics - 512 GB SSD - 15.6 inç</p>" },
             new Product(){ Name = "Asus ROG G16", Price = 84000, Images = { new Image() {ImageUrl = "asus_rog1.jpg" }, new Image() {ImageUrl = "asus_rog2.jpg" }, new Image() {ImageUrl = "asus_rog3.jpg" }, new Image() {ImageUrl = "asus_rog4.jpg" } },Description ="<p>Nvidia GeForce RTX 4070 - 1TB SSD - 16 inç</p>" },
             new Product(){ Name = "Casper Excalibur", Price = 98000, Images = { new Image() {ImageUrl = "casper_pc1.jpg" }, new Image() {ImageUrl = "casper_pc2.jpg" }, new Image() {ImageUrl = "casper_pc3.jpg" }, new Image() {ImageUrl = "casper_pc4.jpg" } },Description ="<p>Nvidia GeForce RTX 5070Ti - 2TB SSD - 16 inç</p>" },
             new Product(){ Name = "Hometech Alfa", Price = 6600, Images = { new Image() {ImageUrl = "hometech1.jpg" }, new Image() {ImageUrl = "hometech2.jpg" }, new Image() {ImageUrl = "hometech3.jpg" }, new Image() {ImageUrl = "hometech4.jpg" } },Description ="<p>Dahili Ekran Kartı - 128 GB SSD - 14.1 inç</p>" },
             new Product(){ Name = "Lenovo Ideapad Slim", Price = 8400, Images = { new Image() {ImageUrl = "lenovo1.jpg" }, new Image() {ImageUrl = "lenovo2.jpg" }, new Image() {ImageUrl = "lenovo3.jpg" }, new Image() {ImageUrl = "lenovo4.jpg" } },Description ="<p>Onboard - 128 GB HD - 15.6 inç</p>" },
             new Product(){ Name = "MSI Thin 15", Price = 30000, Images = { new Image() {ImageUrl = "msi1.jpg" }, new Image() {ImageUrl = "msi2.jpg" }, new Image() {ImageUrl = "msi3.jpg" }, new Image() {ImageUrl = "msi4.jpg" } },Description ="<p>Nvidia GeForce RTX 3050 - 512 GB SSD - 15.6 inç</p>" },
             new Product(){ Name = "MSI Pulse 16", Price = 54000, Images = { new Image() {ImageUrl = "msi_pulse1.jpg" }, new Image() {ImageUrl = "msi_pulse2.jpg" }, new Image() {ImageUrl = "msi_pulse3.jpg" }, new Image() {ImageUrl = "msi_pulse4.jpg" } },Description ="<p>Nvidia GeForce RTX 4060 - 1TB SSD - 16 inç</p>" },
             new Product(){ Name = "Monster Tulpar", Price = 87000, Images = { new Image() {ImageUrl = "monster1.jpg" }, new Image() {ImageUrl = "monster2.jpg" }, new Image() {ImageUrl = "monster3.jpg" }, new Image() {ImageUrl = "monster4.jpg" } },Description ="<p>Nvidia GeForce RTX 5070 - 1TB SSD - 16 inç</p>" },
             new Product(){ Name = "Huawei Matebook D16", Price = 18800, Images = { new Image() {ImageUrl = "huawei_pc1.jpg" }, new Image() {ImageUrl = "huawei_pc2.jpg" }, new Image() {ImageUrl = "huawei_pc3.jpg" }, new Image() {ImageUrl = "huawei_pc4.jpg" } },Description ="<p>Paylaşımlı Ekran Kartı - 512 GB SSD - 16 inç</p>" },

             new Product(){ Name = "Playstation 5 Pro", Price = 44000, Images = { new Image() {ImageUrl = "ps1.jpg" }, new Image() {ImageUrl = "ps2.jpg" }, new Image() {ImageUrl = "ps3.jpg"}, new Image() {ImageUrl = "ps4.jpg"} },Description ="<p>1 TB SSD - 120 FPS - AMD RDNA 2 - ( DualSense Kol x2 )</p>"},
             new Product(){ Name = "Asus ROG Oyuncu Klavye", Price = 24000, Images = { new Image() {ImageUrl = "klavye1.jpg" }, new Image() {ImageUrl = "klavye2.jpg" }, new Image() {ImageUrl = "klavye3.jpg" }, new Image() {ImageUrl = "klavye4.jpg" } },Description ="<p>Kablolu - Mekanik - Q(İngilizce)</p>" },
             new Product(){ Name = "Divoom Pro Piksel Ekranlı Bluetooth Hoparlör", Price = 4800, Images = { new Image() {ImageUrl = "piksel1.jpg" }, new Image() {ImageUrl = "piksel2.jpg" }, new Image() {ImageUrl = "piksel3.jpg" }, new Image() {ImageUrl = "piksel4.jpg" } },Description ="<p>Bluetooth 5.0 - 5000mA - 48mm - 530 gr</p>" },
             new Product(){ Name = "Huawei Freebuds Se2", Price = 1100, Images = { new Image() {ImageUrl = "kulaklik1.jpg" }, new Image() {ImageUrl = "kulaklik2.jpg" }, new Image() {ImageUrl = "kulaklik3.jpg" }, new Image() {ImageUrl = "kulaklik4.jpg" } },Description ="<p>Bluetooth 5.3 - 40h - TWS</p>" },
             new Product(){ Name = "Hp LaserJet M111 Yazıcı", Price = 5800, Images = { new Image() {ImageUrl = "yazici1.jpg" }, new Image() {ImageUrl = "yazici2.jpg" }, new Image() {ImageUrl = "yazici3.jpg" }, new Image() {ImageUrl = "yazici4.jpg" } },Description ="<p>USB 2.0 - A4 - 600x600 DPI</p>" },
             new Product(){ Name = "Huawei Watch Fit 4", Price = 4500, Images = { new Image() {ImageUrl = "saat1.jpg" }, new Image() {ImageUrl = "saat2.jpg" }, new Image() {ImageUrl = "saat3.jpg" }, new Image() {ImageUrl = "saat4.jpg" } },Description ="<p>38 mm - 1.82 inç - Android - Huawei Health+</p>" },
             new Product(){ Name = "Lenovo Tab P12", Price = 27000, Images = { new Image() {ImageUrl = "tablet1.jpg" }, new Image() {ImageUrl = "tablet2.jpg" }, new Image() {ImageUrl = "tablet3.jpg" }, new Image() {ImageUrl = "tablet4.jpg" } },Description ="<p>(Klavye + Kalem) - 8 GB RAM - 256 GB - 12.7 inç</p>" },
             new Product(){ Name = "Meta Quset Sanal Gerçeklik Gözlüğü", Price = 33000, Images = { new Image() {ImageUrl = "vr1.jpg" }, new Image() {ImageUrl = "vr2.jpg" }, new Image() {ImageUrl = "vr3.jpg" }, new Image() {ImageUrl = "vr4.jpg" } },Description ="<p>4K+ - 512 GB - 3D Sound</p>" },
             new Product(){ Name = "Xiaomi Mi TV Stick Medya Oynatıcı", Price = 2000, Images = { new Image() {ImageUrl = "stick1.jpg" }, new Image() {ImageUrl = "stick2.jpg" }, new Image() {ImageUrl = "stick3.jpg" }, new Image() {ImageUrl = "stick4.jpg" } },Description ="<p>Full HD - HDMI - Dolby DTS - 1GB RAM</p>" },
             new Product(){ Name = "Thrustmaster Hyprid Yarış Direksiyon Seti", Price = 22000, Images = { new Image() {ImageUrl = "direksiyon1.jpg" }, new Image() {ImageUrl = "direksiyon2.jpg" }, new Image() {ImageUrl = "direksiyon3.jpg" }, new Image() {ImageUrl = "direksiyon4.jpg" } },Description ="<p>(Pc,Xbox One, X/s) - Dinamik Force Feedback</p>" },

             new Product(){ Name = "Arzum Steamline Buharlı Ütü", Price = 1600, Images = { new Image() {ImageUrl = "arzum1.jpg" }, new Image() {ImageUrl = "arzum2.jpg" }, new Image() {ImageUrl = "arzum3.jpg" }, new Image() {ImageUrl = "arzum4.jpg" } },Description ="<p>100W - 1 Lt - Akrilik Taban - Otomatik Kapanma</p>" },
             new Product(){ Name = "Bosch Climate Duvar Tipi Klima", Price = 27000, Images = { new Image() {ImageUrl = "klima1.jpg" }, new Image() {ImageUrl = "klima2.jpg" }, new Image() {ImageUrl = "klima3.jpg" }, new Image() {ImageUrl = "klima4.jpg" } },Description ="<p>A++ - 12000(BTU/s) - 214cm</p>" },
             new Product(){ Name = "Dyson Airwrap Saç Şekillendirici", Price = 14000, Images = { new Image() {ImageUrl = "dyson1.jpg" }, new Image() {ImageUrl = "dyson2.jpg" }, new Image() {ImageUrl = "dyson3.jpg" }, new Image() {ImageUrl = "dyson4.jpg" } },Description ="<p>Titanyum Yüzey- 3 Isı Kademesi - Otomatik Kapanma</p>" },
             new Product(){ Name = "Elektromarla Minibar Mini Buzdolabı", Price = 9800, Images = { new Image() {ImageUrl = "minibar1.jpg" }, new Image() {ImageUrl = "minibar2.jpg" }, new Image() {ImageUrl = "minibar3.jpg" }, new Image() {ImageUrl = "minibar4.jpg" } },Description ="<p>46Lt - 56.3cm - Mekanik Kontrol Paneli</p>" },
             new Product(){ Name = "Grundig Wk 5440 Cam Kettle", Price = 900, Images = { new Image() {ImageUrl = "su1.jpg" }, new Image() {ImageUrl = "su2.jpg" }, new Image() {ImageUrl = "su3.jpg" }, new Image() {ImageUrl = "su4.jpg" } },Description ="<p>1.7 Lt - Çelik - 2200W</p>" },
             new Product(){ Name = "Grundig Süt Köpürtücülü Espresso Makinesi", Price = 3800, Images = { new Image() {ImageUrl = "kahve1.jpg" }, new Image() {ImageUrl = "kahve2.jpg" }, new Image() {ImageUrl = "kahve3.jpg" }, new Image() {ImageUrl = "kahve4.jpg" } },Description ="<p>1500W - 15p - Led Ekran</p>" },
             new Product(){ Name = "Westinghouse Ekmek Kızartma Makinesi", Price = 3400, Images = { new Image() {ImageUrl = "ekmek1.jpg" }, new Image() {ImageUrl = "ekmek2.jpg" }, new Image() {ImageUrl = "ekmek3.jpg" }, new Image() {ImageUrl = "ekmek4.jpg" } },Description ="<p>Metal - 815W - Isı Ayarı</p>" },
             new Product(){ Name = "Xiaomi Mi Akıllı Robot Süpürge", Price = 10300, Images = { new Image() {ImageUrl = "robot1.jpg" }, new Image() {ImageUrl = "robot2.jpg" }, new Image() {ImageUrl = "robot3.jpg" }, new Image() {ImageUrl = "robot4.jpg" } },Description ="<p>5200mAh - 8000Pa - LDS Lazer Navigasyon</p>" },
             new Product(){ Name = "Vestel CMI 108242 Pro Çamaşır Makinesi", Price = 35000, Images = { new Image() {ImageUrl = "vestel1.jpg" }, new Image() {ImageUrl = "vestel2.jpg" }, new Image() {ImageUrl = "vestel3.jpg" }, new Image() {ImageUrl = "vestel4.jpg" } },Description ="<p>A - 1400 Devir - 10kg - WI-FI</p>" },
             new Product(){ Name = "Vestel Ankastre Fırın", Price = 14000, Images = { new Image() {ImageUrl = "firin1.jpg" }, new Image() {ImageUrl = "firin2.jpg" }, new Image() {ImageUrl = "firin3.jpg" }, new Image() {ImageUrl = "firin4.jpg" } },Description ="<p>A - 66Lt - Multifonksiyon - 60cm</p>" },

        };

        private static ProductCategory[] ProductCategories = {
            new ProductCategory(){Product=Products[0],Category=Categories[0]},//telefon
            new ProductCategory(){Product=Products[1],Category=Categories[3]},//Ev gereçleri
            new ProductCategory(){Product=Products[2],Category=Categories[0]},
            new ProductCategory(){Product=Products[2],Category=Categories[2]},
            new ProductCategory(){Product=Products[3],Category=Categories[0]},
            new ProductCategory(){Product=Products[3],Category=Categories[2]},
            new ProductCategory(){Product=Products[4],Category=Categories[0]},
            new ProductCategory(){Product=Products[4],Category=Categories[2]},
            new ProductCategory(){Product=Products[5],Category=Categories[0]},
            new ProductCategory(){Product=Products[5],Category=Categories[2]},
            new ProductCategory(){Product=Products[6],Category=Categories[0]},
            new ProductCategory(){Product=Products[6],Category=Categories[2]},
            new ProductCategory(){Product=Products[7],Category=Categories[0]},
            new ProductCategory(){Product=Products[7],Category=Categories[2]},
            new ProductCategory(){Product=Products[8],Category=Categories[0]},
            new ProductCategory(){Product=Products[8],Category=Categories[2]},
            new ProductCategory(){Product=Products[9],Category=Categories[0]},
            new ProductCategory(){Product=Products[9],Category=Categories[2]},
            new ProductCategory(){Product=Products[10],Category=Categories[0]},
            new ProductCategory(){Product=Products[10],Category=Categories[2]},
            new ProductCategory(){Product=Products[11],Category=Categories[0]},
            new ProductCategory(){Product=Products[11],Category=Categories[2]},

            new ProductCategory(){Product=Products[12],Category=Categories[1]},
            new ProductCategory(){Product=Products[12],Category=Categories[2]},
            new ProductCategory(){Product=Products[13],Category=Categories[1]},
            new ProductCategory(){Product=Products[13],Category=Categories[2]},
            new ProductCategory(){Product=Products[14],Category=Categories[1]},
            new ProductCategory(){Product=Products[14],Category=Categories[2]},
            new ProductCategory(){Product=Products[15],Category=Categories[1]},
            new ProductCategory(){Product=Products[15],Category=Categories[2]},
            new ProductCategory(){Product=Products[16],Category=Categories[1]},
            new ProductCategory(){Product=Products[16],Category=Categories[2]},
            new ProductCategory(){Product=Products[17],Category=Categories[1]},
            new ProductCategory(){Product=Products[17],Category=Categories[2]},
            new ProductCategory(){Product=Products[18],Category=Categories[1]},
            new ProductCategory(){Product=Products[18],Category=Categories[2]},
            new ProductCategory(){Product=Products[19],Category=Categories[1]},
            new ProductCategory(){Product=Products[19],Category=Categories[2]},
            new ProductCategory(){Product=Products[20],Category=Categories[1]},
            new ProductCategory(){Product=Products[20],Category=Categories[2]},
            new ProductCategory(){Product=Products[21],Category=Categories[1]},
            new ProductCategory(){Product=Products[21],Category=Categories[2]},

            new ProductCategory(){Product=Products[22],Category=Categories[2]},
            new ProductCategory(){Product=Products[23],Category=Categories[2]},
            new ProductCategory(){Product=Products[24],Category=Categories[2]},
            new ProductCategory(){Product=Products[25],Category=Categories[2]},
            new ProductCategory(){Product=Products[26],Category=Categories[2]},
            new ProductCategory(){Product=Products[27],Category=Categories[2]},
            new ProductCategory(){Product=Products[28],Category=Categories[2]},
            new ProductCategory(){Product=Products[29],Category=Categories[2]},
            new ProductCategory(){Product=Products[30],Category=Categories[2]},
            new ProductCategory(){Product=Products[31],Category=Categories[2]},

            new ProductCategory(){Product=Products[32],Category=Categories[2]},
            new ProductCategory(){Product=Products[32],Category=Categories[3]},
            new ProductCategory(){Product=Products[33],Category=Categories[2]},
            new ProductCategory(){Product=Products[33],Category=Categories[3]},
            new ProductCategory(){Product=Products[34],Category=Categories[2]},
            new ProductCategory(){Product=Products[34],Category=Categories[3]},
            new ProductCategory(){Product=Products[35],Category=Categories[2]},
            new ProductCategory(){Product=Products[35],Category=Categories[3]},
            new ProductCategory(){Product=Products[36],Category=Categories[2]},
            new ProductCategory(){Product=Products[36],Category=Categories[3]},
            new ProductCategory(){Product=Products[37],Category=Categories[2]},
            new ProductCategory(){Product=Products[37],Category=Categories[3]},
            new ProductCategory(){Product=Products[38],Category=Categories[2]},
            new ProductCategory(){Product=Products[38],Category=Categories[3]},
            new ProductCategory(){Product=Products[39],Category=Categories[2]},
            new ProductCategory(){Product=Products[39],Category=Categories[3]},
            new ProductCategory(){Product=Products[40],Category=Categories[2]},
            new ProductCategory(){Product=Products[40],Category=Categories[3]},
            new ProductCategory(){Product=Products[41],Category=Categories[2]},
            new ProductCategory(){Product=Products[41],Category=Categories[3]},

        };
    }
}
