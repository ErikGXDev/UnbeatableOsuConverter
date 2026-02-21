using System;
using Eto.Forms;

namespace UnbeatableConverter.GUI;

/// <summary>
/// A label + button combo that lets the user pick a file, similar to GtkSharp's FileChooserButton.
/// </summary>
public class FilePicker : Panel
{
    private string? _filename;
    public event EventHandler? SelectionChanged;

    private readonly Label _label;

    public string? Filename => _filename;

    public FilePicker()
    {
        _label = new Label { Text = "No file selected", VerticalAlignment = Eto.Forms.VerticalAlignment.Center };

        var browseButton = new Button { Text = "Browse…" };
        browseButton.Click += OnBrowseClicked;

        Content = new StackLayout
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Items =
            {
                new StackLayoutItem(_label, true),
                browseButton
            }
        };
    }

    private void OnBrowseClicked(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Open .osz beatmap file"
        };
        dialog.Filters.Add(new FileFilter("Osu! Beatmaps (*.osz, *.zip)", "*.osz", "*.zip"));
        dialog.CurrentFilterIndex = 0;

        if (dialog.ShowDialog(ParentWindow) == DialogResult.Ok)
        {
            _filename = dialog.FileName;
            _label.Text = System.IO.Path.GetFileName(_filename);
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

