using Microsoft.AspNetCore.Mvc.ViewFeatures;  // ITempDataDictionary için gerekli
using Newtonsoft.Json;                         // JSON serileştirme işlemleri için

namespace ETICARET.WebUI.Extensions
{
    /// <summary>
    /// TempData kullanımını kolaylaştıran extension metodları
    /// TempData normalde sadece string değerler saklayabilir, 
    /// bu extension'lar sayesinde karmaşık nesneler de saklanabilir
    /// </summary>
    public static class TempDataExtensions
    {
        /// <summary>
        /// TempData'ya karmaşık nesneleri JSON formatında saklayan extension metod
        /// </summary>
        /// <typeparam name="T">Saklanacak nesnenin tipi (class constraint ile sınırlandırılmış)</typeparam>
        /// <param name="tempData">Genişletilecek ITempDataDictionary nesnesi</param>
        /// <param name="key">TempData'da kullanılacak anahtar</param>
        /// <param name="value">Saklanacak nesne</param>
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            // Nesneyi JSON string'ine çevir ve TempData'ya kaydet
            // JsonConvert.SerializeObject() metodu nesneyi JSON formatına dönüştürür
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// TempData'dan karmaşık nesneleri JSON formatından deserialize ederek alan extension metod
        /// </summary>
        /// <typeparam name="T">Alınacak nesnenin tipi (class constraint ile sınırlandırılmış)</typeparam>
        /// <param name="tempData">Genişletilecek ITempDataDictionary nesnesi</param>
        /// <param name="key">TempData'dan alınacak verinin anahtarı</param>
        /// <returns>Deserialize edilmiş nesne veya null</returns>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o; // TempData'dan alınacak değeri tutacak değişken

            // TempData'dan belirtilen key ile değer almaya çalış
            // TryGetValue metodu: değer varsa true döner ve out parametresine değeri atar
            tempData.TryGetValue(key, out o);

            // Ternary operator kullanarak kontrol:
            // Eğer o null ise null döndür
            // Değilse string'e cast edip JSON'dan T tipine deserialize et
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
