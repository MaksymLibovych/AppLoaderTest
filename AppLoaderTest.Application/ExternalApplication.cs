using AppLoaderTest.Command;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ricaun.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace AppLoaderTest.Application;

[AppLoader]
public class ExternalApplication : IExternalApplication
{
    private Autodesk.Revit.UI.RibbonPanel? _ribbonPanel;
    private Autodesk.Windows.RibbonPanel? _ribbonPanelWindows;
    private Autodesk.Windows.RibbonTab? _ribbonTabWindows;
    private RibbonTabCollection? _ribbonTabCollection;

    private IHost? _host;

    public Result OnStartup(UIControlledApplication uiControlledApplication)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddTransient<IEchoService, EchoService>();
            }).Build();
        //_host = Host.CreateDefaultBuilder()
        //    .ConfigureServices((hostBuilderContext, services) =>
        //    {
        //        services.AddTransient<IEchoService, EchoService>();
        //    }).Build();
        
        Assembly executingAssembly = Assembly.GetAssembly(typeof(IHost));

        _host?.Start();

        #region Autodesk.Windows Approach
        var ribbonButton = new Autodesk.Windows.RibbonButton
        {
            Name = "RibbonButton",
            Image = new BitmapImage(new Uri("pack://application:,,,/AppLoaderTest.Application;component/Resources/Icons/RevitCommandExternalCommand16.png")),
            LargeImage = new BitmapImage(new Uri("pack://application:,,,/AppLoaderTest.Application;component/Resources/Icons/RevitCommandExternalCommand32.png")),
            CommandHandler = new CommandHandler(_host?.Services.GetRequiredService<IEchoService>()),
        };
        _ribbonPanelWindows = new Autodesk.Windows.RibbonPanel
        {
            Source = new RibbonPanelSource
            {
                Title = "RibbonPanel"
            }
        };
        _ribbonTabWindows = new RibbonTab
        {
            Title = "RibbonTab"
        };
        _ribbonPanelWindows.Source.Items.Add(ribbonButton);
        _ribbonTabWindows.Panels.Add(_ribbonPanelWindows);
        _ribbonTabCollection = ComponentManager.Ribbon.Tabs;
        _ribbonTabCollection.Add(_ribbonTabWindows);
        #endregion

        #region Autodesk.Revit.UI Approach
        //_ribbonPanel = uiControlledApplication
        //    .CreateRibbonPanel(Tab.AddIns, "RibbonPanel");

        //var externalCommandType = typeof(ExternalCommand);
        //var externalCommandPushButton = new PushButtonData(externalCommandType.FullName,
        //                                                      "Revit Command",
        //                                                      Assembly.GetAssembly(externalCommandType).Location,
        //                                                      externalCommandType.FullName)
        //{
        //    Image = new BitmapImage(new Uri($"pack://application:,,,/AppLoaderTest.Application;component/Resources/Icons/RevitCommandExternalCommand16.png")),
        //    LargeImage = new BitmapImage(new Uri($"pack://application:,,,/AppLoaderTest.Application;component/Resources/Icons/RevitCommandExternalCommand32.png")),
        //    ToolTip = "Revit Command Tooltip"
        //};

        //Autodesk.Revit.UI.RibbonItem externalCommandRibbonItem = _ribbonPanel.AddItem(externalCommandPushButton);

        //Autodesk.Windows.RibbonButton? externalCommandWindowsRibbonItem = GetRibbonItem(externalCommandRibbonItem)
        //    as Autodesk.Windows.RibbonButton;

        //if (externalCommandWindowsRibbonItem is not null)
        //{
        //    externalCommandWindowsRibbonItem.CommandHandler = new CommandHandler();
        //}
        #endregion

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        _ribbonTabCollection?.Remove(_ribbonTabWindows);
        _host?.Dispose();
        return Result.Succeeded;
    }

    private Autodesk.Windows.RibbonItem? GetRibbonItem(Autodesk.Revit.UI.RibbonItem item)
    {
        Type itemType = item.GetType();

        MethodInfo methodInfo = itemType.GetMethod(
            "getRibbonItem", BindingFlags.NonPublic | BindingFlags.Instance);

        object windowRibbonItem = methodInfo.Invoke(item, null);

        return windowRibbonItem as Autodesk.Windows.RibbonItem;
    }
}
