
namespace PizzAPP.Model
{
    public class Pizza
    {
        public string Title{ get; set; }
        public double Diameter{ get; set; }
        public double Area => Math.Pow(Diameter / 2, 2) * Math.PI;
        public double Price { get; set; }
        public double Shipping { get; set; }
        public double TotalPrice => Price + Shipping;
        public string URL { get; set; }
        public int Rating { get; set; }
        public double UnitPrice => (Price + Shipping) / Area;
        public Guid Guid { get; set; } //TODO should not be public settable, although json parse does not work in MAUI now
        public bool ToBeDeleted { get; set; }

        public Pizza(bool isNewInstance = true)
        {
            if(isNewInstance)
            {
                Guid= Guid.NewGuid();
            }
        }

        public Pizza Copy()
        {
            return new Pizza(false)
            {
                Title = Title,
                Diameter = Diameter,
                Price = Price,
                Shipping = Shipping,
                URL = URL,
                Rating = Rating,
                Guid = Guid
            };
        }
    }
}
