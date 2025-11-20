using ETICARET.WebUI.Models;           // PageInfo modeli için
using Microsoft.AspNetCore.Razor.TagHelpers;  // TagHelper base class için
using System.Text;                     // StringBuilder için

namespace ETICARET.WebUI.TagHelpers
{
    /// <summary>
    /// Sayfalama (pagination) bileşeni oluşturan özel Tag Helper
    /// HTML'de <div page-model="@Model.PageInfo"></div> şeklinde kullanılır
    /// </summary>
    [HtmlTargetElement("div", Attributes = "page-model")]  // div elementine page-model attribute'u eklendiğinde çalışır
    public class PageLinkTagHelper : TagHelper
    {
        /// <summary>
        /// Sayfalama bilgilerini içeren model
        /// Razor view'dan page-model attribute'u ile set edilir
        /// </summary>
        public PageInfo PageModel { get; set; }

        /// <summary>
        /// Tag Helper'ın ana işlem metodu
        /// HTML çıktısını oluşturur ve sayfalama linklerini render eder
        /// </summary>
        /// <param name="context">Tag Helper'ın çalıştığı context bilgisi</param>
        /// <param name="output">Oluşturulacak HTML çıktısı</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Çıktı tag'ini div olarak ayarla
            output.TagName = "div";

            // HTML içeriğini oluşturmak için StringBuilder kullan (performans için)
            StringBuilder stringBuilder = new StringBuilder();

            // Bootstrap pagination component'inin başlangıç HTML'i
            stringBuilder.Append("<ul class='pagination'>");

            // Tüm sayfalar için döngü (1'den toplam sayfa sayısına kadar)
            for (int i = 1; i <= PageModel.TotalPages(); i++)
            {
                // Her sayfa için list item oluştur
                // Mevcut sayfa ise 'active' class'ı ekle (Bootstrap styling için)
                stringBuilder.AppendFormat("<li class='page-item {0}'>",
                    i == PageModel.CurrentPage ? "active" : "");

                // Kategori kontrolü yaparak uygun URL formatını belirle
                if (string.IsNullOrEmpty(PageModel.CurrentCategory))
                {
                    // Kategori yok ise: /products?page=X formatında URL
                    stringBuilder.AppendFormat("<a class='page-link' href='/products?page={0}'>{0}</a>", i);
                }
                else
                {
                    // Kategori var ise: /products/kategori?page=X formatında URL
                    stringBuilder.AppendFormat("<a class='page-link' href='/products/{0}?page={1}'>{1}</a>",
                        PageModel.CurrentCategory, i);
                }

                // List item'ı kapat
                stringBuilder.AppendFormat("</li>");
            }

            // Pagination component'ini kapat
            stringBuilder.AppendFormat("</ul>");

            // Oluşturulan HTML içeriğini output'a set et
            // SetHtmlContent: HTML olarak render edilmesini sağlar (encode edilmez)
            output.Content.SetHtmlContent(stringBuilder.ToString());

            // Base class'ın Process metodunu çağır (opsiyonel)
            base.Process(context, output);
        }
    }
}
