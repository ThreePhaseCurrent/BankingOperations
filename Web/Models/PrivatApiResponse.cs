namespace Web.Models
{
    public class PrivatApiResponse
    {
        public string  ccy      { get; set; }
        public string  base_ccy { get; set; }
        public decimal buy      { get; set; }
        public decimal sale     { get; set; }

        public override string ToString() =>
            $"{nameof(ccy)}: {ccy}, {nameof(base_ccy)}: {base_ccy}, {nameof(buy)}: {buy}, {nameof(sale)}: {sale}";
    }
}