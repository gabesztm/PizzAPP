namespace PizzAPP.Model
{
    internal static class DataPersistor
    {
        private const string _fileName = "Pizzas.dat";
        private const string _separator = ";";
        public static async Task Save(IEnumerable<Pizza> pizzas)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(GetFullPath(), append: false))
                {
                    foreach (var pizza in pizzas)
                    {
                        string line = GetDataString(pizza);
                        await file.WriteLineAsync(line);
                    }
                }
            }
            catch(Exception ex)
            { 

            }
        }

        private static string GetFullPath()
        {
            return Path.Combine(FileSystem.Current.AppDataDirectory, _fileName);
        }

        private static string GetDataString(Pizza pizza)
        {
            return string.Join(_separator, pizza.Title, pizza.Diameter, pizza.Price, pizza.Shipping, pizza.URL, pizza.Rating, pizza.Guid);
        }

        private static Pizza ParseDataString(string data)
        {
            string[] splits = data.Split(_separator);
            if(splits.Length != 7)
            {
                //TODO missing values
                return null;
            }

            string title = splits[0];
            bool success = true;
            success &= double.TryParse(splits[1], out double diameter);
            success &= double.TryParse(splits[2], out double price);
            success &= double.TryParse(splits[3], out double shipping);
            string url = splits[4];
            success &= int.TryParse(splits[5], out int rating);
            success &= Guid.TryParse(splits[6], out Guid guid);

            if (!success)
            {
                //TODO invalid values
                return null;
            }

            Pizza pizza = new Pizza()
            {
                Title = title,
                Diameter = diameter,
                Price = price,
                Shipping = shipping,
                URL = url,
                Rating = rating,
                Guid = guid
            };

            return pizza;
        }

        public static IEnumerable<Pizza> Load()
        {
            List<Pizza> pizzas = new List<Pizza>();
            if (File.Exists(GetFullPath()))
            {
                foreach (string line in File.ReadLines(GetFullPath()))
                {
                    var pizza = ParseDataString(line);
                    if (pizza != null)
                    {
                        pizzas.Add(pizza);
                    }
                }
            }

            return pizzas;
        }
    }
}
