using BlazorRtsPlayer.BlazorApp;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorRtsPlayer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();
            serviceCollection.AddScoped(sp => new HttpClient { BaseAddress = new Uri("") });
            serviceCollection.AddSingleton<RtspInMemoryService>();

            mainBlazorWebView.HostPage = "wwwroot\\index.html";
            mainBlazorWebView.Services = serviceCollection.BuildServiceProvider();
            mainBlazorWebView.RootComponents.Add<App>("#app");
        }
    }
}