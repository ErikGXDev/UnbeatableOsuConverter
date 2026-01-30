using Gtk;

namespace UnbeatableConverter.GUI;

public class Dialog
{
    public static void Show(string title, string message, MessageType type)
    {
        Application.Invoke((sender, e) =>
        {
            var md = new MessageDialog(null,
                DialogFlags.Modal,
                type,
                ButtonsType.Ok,
                message);
            md.Title = title;
            md.Run();
            md.Destroy();
        });
    }
}