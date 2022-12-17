using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PizzAPP.Model;
using PizzAPP.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzAPP
{
    [QueryProperty("Pizza","Pizza")]
    public partial class EditPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private Pizza _pizza;

        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task Delete()
        {
            bool confirmationAnswer = await Shell.Current.DisplayAlert(AppRes.Warning, AppRes.ConfirmationQuestion, AppRes.Yes, AppRes.No);
            if (!confirmationAnswer)
            {
                return;
            }
            var pizzaCopy = _pizza.Copy();
            pizzaCopy.ToBeDeleted = true;
            var navigationParameter = new Dictionary<string, object>
                {
                    { "EditedPizza", pizzaCopy }
                };
            await Shell.Current.GoToAsync("..", navigationParameter);
        }

        [RelayCommand]
        public async Task Save()
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "EditedPizza", _pizza.Copy() }
                };
            await Shell.Current.GoToAsync("..", navigationParameter);
        }


    }
}
