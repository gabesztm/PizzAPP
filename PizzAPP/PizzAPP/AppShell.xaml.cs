namespace PizzAPP;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(EditPage), typeof(EditPage));
	}
}
