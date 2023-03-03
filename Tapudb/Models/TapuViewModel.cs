namespace Tapudb.Models
{
    public class TapuViewModel
    {
       
       

        public int Id { get; set; }
        public int sayi { get; set; }
        public string Adres { get; set; }
        public DateTime KayitTarihi { get; set; }
        public KullaniciViewModel Kullanici { get; set; }
        public int sahipId{ get; set; }
    }
}
