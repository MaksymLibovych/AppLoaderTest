using Autodesk.Revit.UI;
using System.Windows.Input;

namespace AppLoaderTest.Command;

public class CommandHandler : ICommand
{
    private readonly IEchoService _echoService;

    public CommandHandler(IEchoService echoService)
    {
        _echoService = echoService;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        _echoService.Echo();
    }

    public bool CanExecute(object parameter) => true;
}
