using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PizzAPP.Enums;
using PizzAPP.Model;
using PizzAPP.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PizzAPP
{
    [QueryProperty("EditedPizza", "EditedPizza")]
    internal partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private Pizza _editedPizza;

        [ObservableProperty]
        private List<LocalizedSorting> _availableSortings;

        [ObservableProperty]
        private ObservableCollection<Pizza> _pizzas;
        

        private LocalizedSorting _selectedSorting;
        public LocalizedSorting SelectedSorting
        {
            get { return _selectedSorting; }
            set { _selectedSorting = value; Sort(); }
        }

        public MainPageViewModel()
        {
            FillSortingCollection();
            LoadPizzas();
            SelectedSorting = AvailableSortings.First(_ => _.Method == OrderBy.UnitPriceLowToHigh);
            this.PropertyChanged += MainPageViewModel_PropertyChanged;
        }

        private void Sort()
        {
            switch (SelectedSorting.Method)
            {
                case OrderBy.UnitPriceLowToHigh:
                    Pizzas = new ObservableCollection<Pizza>(Pizzas.OrderBy(_ => _.UnitPrice));
                    break;
                case OrderBy.UnitPriceHighToLow:
                    Pizzas = new ObservableCollection<Pizza>(Pizzas.OrderByDescending(_ => _.UnitPrice));
                    break;
                case OrderBy.TotalPriceLowToHigh:
                    Pizzas = new ObservableCollection<Pizza>(Pizzas.OrderBy(_ => _.TotalPrice));
                    break;
                case OrderBy.TotalPriceHighToLow:
                    Pizzas = new ObservableCollection<Pizza>(Pizzas.OrderByDescending(_ => _.TotalPrice));
                    break;
                case OrderBy.RatingLowToHigh:
                    Pizzas = new ObservableCollection<Pizza>(Pizzas.OrderBy(_ => _.Rating));
                    break;
                case OrderBy.RatingHighToLow:
                    Pizzas = new ObservableCollection<Pizza>(Pizzas.OrderByDescending(_ => _.Rating));
                    break;
                default:
                    throw new InvalidEnumArgumentException($"{nameof(Sort)} was called with invalid argument: {SelectedSorting.Method}");
            }
        }

        private void MainPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "EditedPizza")
            {
                if(EditedPizza != null)
                {
                    Task t;
                    if (EditedPizza.ToBeDeleted)
                    {
                        var pizzaToBeRemoved = Pizzas.FirstOrDefault(_ => _.Guid == EditedPizza.Guid);
                        if (pizzaToBeRemoved != null)
                        {
                            Pizzas.Remove(pizzaToBeRemoved);
                        }
                        t = Task.Run(async () => {await DataPersistor.Save(Pizzas); });
                        t.Wait();
                        return;
                    }

                    var pizzaToBeUpdated = Pizzas.FirstOrDefault(_ => _.Guid == EditedPizza.Guid);
                    if(pizzaToBeUpdated != null)
                    {
                        var index = Pizzas.IndexOf(pizzaToBeUpdated);
                        Pizzas.Remove(pizzaToBeUpdated);
                        Pizzas.Insert(index, EditedPizza);
                        t = Task.Run(async () => { await DataPersistor.Save(Pizzas); });
                        t.Wait();
                        return;
                    }
                    
                    Pizzas.Add(EditedPizza);
                    t = Task.Run(async () => { await DataPersistor.Save(Pizzas); });
                    t.Wait();

                }
            }
        }

        private void FillSortingCollection()
        {
            var methods = Enum.GetValues(typeof(OrderBy)).Cast<OrderBy>().ToList();
            var availableSortings = new List<LocalizedSorting>();
            foreach (var method in methods)
            {
                var localizedMethod = new LocalizedSorting()
                {
                    Method = method,
                    LocalizedMethod = GetMethodLocalization(method)
                };
                availableSortings.Add(localizedMethod);
            }
            AvailableSortings = availableSortings;

        }

        private string GetMethodLocalization(OrderBy method)
        {
            string languageISOName = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (languageISOName == "hu")
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hu-HU");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }

            string methodString = method.ToString();
            string localizedMethod = AppRes.ResourceManager.GetString(methodString);
            return localizedMethod;
        }

        private void LoadPizzas()
        {
            Pizzas = new ObservableCollection<Pizza>(DataPersistor.Load());
        }

        [RelayCommand]
        public async Task Edit(object o)
        {
            var p = o as Pizza;
            if (p != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { nameof(Pizza), p.Copy() }
                };
                await Shell.Current.GoToAsync(nameof(EditPage), navigationParameter);
            }
        }

        [RelayCommand]
        public async Task Create()
        {
            var pizza = new Pizza();
            var navigationParameter = new Dictionary<string, object>
                {
                    { nameof(Pizza), pizza }
                };
            await Shell.Current.GoToAsync(nameof(EditPage), navigationParameter);
        }

        [RelayCommand]
        public async Task OpenLink(object o)
        {
            var p = o as Pizza;
            if (p != null)
            {
                try
                {
                    Uri uri = new Uri(p.URL);
                    await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                }
                catch(Exception ex)
                {
                    //TODO
                }
            }
        }
    }
}
