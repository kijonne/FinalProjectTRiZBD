using System.Configuration;
using System.Data;
using System.Security.Claims;
using System.Windows;

namespace OnlineShoeStoreWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ClaimsPrincipal CurrentUser { get; set; } = new ClaimsPrincipal(new ClaimsIdentity());
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            new LoginWindow().Show();
        }
    }

}
