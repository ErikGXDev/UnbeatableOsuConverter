using Eto.Forms;

namespace UnbeatableConverter.GUI;

public static class Dialog
{
    public enum MessageType { Info, Error, Warning }

    public static void Show(string title, string message, MessageType type = MessageType.Info)
    {
        Application.Instance.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK,
                type == MessageType.Error ? MessageBoxType.Error :
                type == MessageType.Warning ? MessageBoxType.Warning :
                MessageBoxType.Information);
        });
    }
}

