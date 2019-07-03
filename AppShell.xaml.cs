using ShiftCheck.Views;
using Xamarin.Forms;

namespace ShiftCheck
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PendingSamplesPage), typeof(PendingSamplesPage));
            Routing.RegisterRoute(nameof(CreateHandoverPage), typeof(CreateHandoverPage));
        }
    }
}
