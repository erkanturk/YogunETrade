// Yorum içeriğinin yükleneceği HTML elementi seçicisi
CommentBodyId = "#comment"

// Ürün ID (başlangıçta -1, sonrasında güncellenir)
var productId = -1

// Küçük resme tıklandığında büyük resim alanında gösteren fonksiyon
function imageBox(smallImg) {
    // "image-box" id'li element (büyük resim çerçevesi) seçilir
    var fullImg = document.getElementById("image-box")
    // Küçük resmin kaynağını büyük resme atıyoruz
    fullImg.src = smallImg.src
}

// Sayfa yüklendiğinde (document.ready) çalışacak kodlar
$(document).ready(function () {
    // "#comment" öğesinin "data-url" bilgisini alır
    var url = $("#comment").data("url")

    // "#comment" içine, data-url ile gelen kısmi görünüm veya içerik yüklenir
    $("#comment").load(url)

    // "#comment" öğesinin "data-product-id" bilgisini productId değişkenine atar
    productId = $("#comment").data("product-id")

    // CommentBodyId (yani "#comment") alanına, ilgili ürüne ait yorumları yükler
    $(CommentBodyId).load("/Comment/ShowProductComments?id=" + productId)
})

// Yorum ekleme/silme/düzenleme işlemlerini gerçekleştiren fonksiyon
// btn       : işlemi başlatan buton elemanı
// e         : olay tipi (new_clicked, delete_clicked, edit_clicked)
// commentId : hedef alınan yorumun ID'si (silinecek, düzenlenecek ya da yeni eklenecek yorum)
// spanId    : düzenleme anında metnin yer aldığı span/element seçicisi
function doComment(btn, e, commentId, spanId) {
    // tıklanan butonu jQuery ile sarar
    var button = $(btn)
    // buton üzerindeki data-edit-mode bilgisi (boolean) => düzenleme modunda mı?
    var mode = button.data("edit-mode")
    // düzenlenecek metin alanının id'sini üretir ve seçer (#comment_text_X gibi)
    var editableContent = $("#comment_text_" + commentId)

    // Yeni yorum ekleme
    if (e == 'new_clicked') {
        // Yeni yorum metnini input veya textarea'dan alır
        var txt = $("#new_comment_text").val()

        // Sunucuya AJAX ile POST isteği atarak yeni yorumu ekler
        $.ajax({
            method: "POST",
            url: '/Comment/Create',
            data: { 'text': txt, 'productId': productId }
        }).done(function (data) {
            // Sunucudan dönen sonuç "result" true ise yorum başarıyla eklenmiş demektir
            if (data.result) {
                // Yorumlar kısmını tekrar yükler (güncel liste)
                $(CommentBodyId).load("/Comment/ShowProductComments?id=" + productId)
            }
            else {
                alert("Yorum yapılırken bir hata oluştu!")
            }
        }).fail(function (error) {
            alert("Sunucuda bir hata oluştu!")
        })
    }
    // Yorum silme işlemi
    else if (e == 'delete_clicked') {
        // Kullanıcıya onay mesajı gösterir
        var dialog_res = confirm("Yorum Silinsin mi?")

        // Kullanıcı onay vermezse işlemi durdurur
        if (!dialog_res) return false

        // Sunucuya AJAX isteğiyle DELETE işlemini gönderir
        $.ajax({
            method: "POST",
            url: '/Comment/Delete?id=' + commentId,
        }).done(function (data) {
            if (data.result) {
                // Silme başarılıysa, yorum listesi güncellenir
                $(CommentBodyId).load("/Comment/ShowProductComments?id=" + productId)
            }
            else {
                alert("Yorum Silinemedi!")
            }
        }).fail(function (error) {
            alert("Sunucuda bir hata oluştu!")
        })
    }
    // Yorum düzenleme işlemi
    else if (e == 'edit_clicked') {
        // Eğer düzenleme modu aktif değilse (false), butona tıklama ile düzenleme modunu açar
        if (!mode) {
            // Butonun veri özelliğini düzenleme modunu aktif hale getir
            button.data("edit-mode", true)

            // Butonun görsel niteliklerini "düzenleme onaylama" haline getirir
            button.removeClass("btn-warning")
            button.addClass("btn-success")

            // Butondaki ikonun edit (kalem) yerine check (onay) ikonuna geçmesi
            var btnSpan = button.find("span")
            btnSpan.removeClass("fa-edit")
            btnSpan.addClass("fa-check")

            // Düzenlenecek metin alanını editable hale getirir (örneğin border, arkaplan vs.)
            editableContent.addClass("editable-content")
            editableContent.addClass("editableComment")
            // contenteditable özelliğini true yaparak kullanıcıya düzenleme imkanı verir
            $(spanId).attr("contenteditable", true)
        }
        // Eğer düzenleme modu zaten aktifse, bu tıklamayla değişiklikleri onaylama aşamasına geçer
        else {
            // Düzenleme modunu pasifleştir
            button.data("edit-mode", false)

            // Buton stilini geri eski haline getir
            button.removeClass("btn-success")
            button.addClass("btn-warning")

            // Butondaki ikonun check yerine tekrar edit olarak değiştirilmesi
            var btnSpan = button.find("span")
            btnSpan.removeClass("fa-check")
            btnSpan.addClass("fa-edit")

            // Metin alanına eklenen özel sınıfları kaldırarak görüntüyü düzenleme modundan çıkar
            editableContent.removeClass("editable-content")
            editableContent.removeClass("editableComment")

            // contenteditable yine aktif kalıyor ama istenirse false yapılabilir
            $(spanId).attr("contenteditable", true)

            // Kullanıcı tarafından düzenlenen metni alır
            var txt = $(spanId).text().trim(' ')

            // AJAX ile sunucuya düzenlenmiş metni gönderir
            $.ajax({
                method: "POST",
                url: '/Comment/Edit',
                data: { 'text': txt, 'id': commentId }
            }).done(function (data) {
                if (data.result) {
                    // Düzenleme başarılıysa yorumları tekrar yükler
                    $(CommentBodyId).load("/Comment/ShowProductComments?id=" + productId)
                }
                else {
                    alert("Yorum Güncellenemedi!")
                }
            }).fail(function (error) {
                alert("Sunucuda bir hata oluştu!")
            })
        }
    }
}
