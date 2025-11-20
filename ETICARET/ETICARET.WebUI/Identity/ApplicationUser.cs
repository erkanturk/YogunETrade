using Microsoft.AspNetCore.Identity;

namespace ETICARET.WebUI.Identity
{
    public class ApplicationUser : IdentityUser //Kullanıcı bilgileri için özel bir sınıf oluşturuldu.(isteğe bağlı)
    {
        public string FullName { get; set; }
    }
}
