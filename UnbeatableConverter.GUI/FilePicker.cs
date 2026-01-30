using Gtk;

namespace UnbeatableConverter.GUI;

public class FilePicker : FileChooserButton
{
    public FilePicker() : base("Open .osz beatmap file", FileChooserAction.Open)
    {
        var filter = new FileFilter();
        filter.AddPattern("*.osz;*.zip");
        filter.Name = "Osu! Beatmaps (*.osz, *.zip)";
        AddFilter(filter);
    }
}