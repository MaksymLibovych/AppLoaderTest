using Autodesk.Revit.UI;

namespace AppLoaderTest.Command;

public class EchoService : IEchoService
{
    public void Echo()
    {
        TaskDialog.Show("Message", "Echo hello qsdfsd");
    }
}
