namespace PizzAPP;

public partial class EditPage : ContentPage
{
	public EditPage(EditPageViewModel vm)
	{
		InitializeComponent();
		BindingContext= vm;
	}
}