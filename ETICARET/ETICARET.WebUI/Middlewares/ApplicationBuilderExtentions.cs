using Microsoft.Extensions.FileProviders;  // PhysicalFileProvider için gerekli

namespace ETICARET.WebUI.Middlewares
{
    /// <summary>
    /// IApplicationBuilder için özel extension metodları içeren sınıf
    /// Middleware pipeline'ına özel konfigürasyonlar eklemek için kullanılır
    /// </summary>
    public static class ApplicationBuilderExtentions  // Typo: "Extensions" olmalı
    {
        /// <summary>
        /// Node.js modüllerini statik dosya olarak sunmak için özel middleware ekler
        /// Bu sayede frontend'de npm paketleri doğrudan kullanılabilir
        /// </summary>
        /// <param name="app">Genişletilecek IApplicationBuilder nesnesi</param>
        /// <returns>Yapılandırılmış IApplicationBuilder nesnesi (method chaining için)</returns>
        public static IApplicationBuilder CustomStaticFiles(this IApplicationBuilder app)
        {
            // Mevcut çalışma dizini ile node_modules klasörünün tam yolunu oluştur
            // Directory.GetCurrentDirectory(): Uygulamanın çalıştığı klasör
            // Path.Combine(): Platform bağımsız yol birleştirme
            var path = Path.Combine(Directory.GetCurrentDirectory(), "node_modules");

            // Statik dosya sunumu için seçenekleri yapılandır
            var options = new StaticFileOptions()
            {
                // Fiziksel dosya sağlayıcısını ayarla
                // PhysicalFileProvider: Disk üzerindeki gerçek dosyalara erişim sağlar
                FileProvider = new PhysicalFileProvider(path),

                // HTTP isteklerinde kullanılacak sanal yolu belirle
                // "/modules" URL'i "node_modules" klasörüne yönlendirilecek
                // Örnek: /modules/bootstrap/dist/css/bootstrap.css -> node_modules/bootstrap/dist/css/bootstrap.css
                RequestPath = "/modules"
            };

            // Yapılandırılmış seçeneklerle statik dosya middleware'ini pipeline'a ekle
            // Bu middleware HTTP isteklerini yakalayıp uygun dosyaları serve eder
            app.UseStaticFiles(options);

            // Method chaining için IApplicationBuilder'ı geri döndür
            // Bu sayede başka middleware'ler de zincirleme şekilde eklenebilir
            return app;
        }
    }
}
