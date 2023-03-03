namespace Tapudb.Models
{
    public class TapuListViewModel
    {
        
        public IEnumerable<KullaniciViewModel>? Kullanicilar { get; set; }
        public TapuViewModel? Tapu { get; set; }
    }
}
