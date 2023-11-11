using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;

namespace MyExcel;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			// Initialize the .NET MAUI Community Toolkit by adding the below line of code
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMopups();

        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
		builder.Services.AddSingleton<IPopupNavigation>(MopupService.Instance);
		builder.Services.AddTransient<MainPage>();

        // Continue initializing your .NET MAUI App here
		#if DEBUG
				builder.Logging.AddDebug();
		#endif
        return builder.Build();
	}
}
