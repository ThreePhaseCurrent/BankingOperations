namespace Web.ViewModels
{
    public class CurrencyViewModel
    {
        public int     Id       { get; set; }
        public string  Name     { get; set; }
        public decimal SaleRate { get; set; }
        public decimal BuyRate  { get; set; }
    }
}