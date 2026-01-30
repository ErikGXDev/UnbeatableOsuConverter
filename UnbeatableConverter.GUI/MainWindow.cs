using System.Linq;
using Gtk;
using UnbeatableConverter.Core;

namespace UnbeatableConverter.GUI
{
    class MainWindow : Window
    {
        public MainWindow() : base("Unbeatable Converter")
        {
            Build();
        }

        private OszExporter _exporter;

        private void Build()
        {
            // Window settings
            DeleteEvent += (o, args) => Application.Quit();
            SetDefaultSize(400, 100);
            SetPosition(WindowPosition.Center);

            // UI Components
            var filePicker = new FilePicker();

            var toggleList = new ToggleList();

            var convertButton = new Button("Convert");

            var tryBeatmaps = new WebsocketButton();
            tryBeatmaps.Sensitive = false;

            // Layout assembly
            var vbox = new Box(Orientation.Vertical, 6);
            vbox.Margin = 12;

            vbox.PackStart(filePicker, false, false, 0);
            vbox.PackStart(toggleList.TreeView, false, false, 0);

            var hbox = new Box(Orientation.Horizontal, 6);
            hbox.PackStart(convertButton, true, true, 0);
            hbox.PackStart(tryBeatmaps, false, false, 0);
            vbox.PackStart(hbox, false, false, 0);


            Add(vbox);

            ShowAll();

            // Event handlers
            filePicker.SelectionChanged += (o, args) =>
            {
                var inputPath = filePicker.Filename;
                if (!string.IsNullOrEmpty(inputPath))
                {
                    _exporter = new OszExporter(inputPath);

                    toggleList.SetItems(_exporter.BeatmapEntries.ToArray());
                }
            };

            convertButton.Clicked += (sender, e) =>
            {
                var inputPath = filePicker.Filename;
                if (string.IsNullOrEmpty(inputPath))
                {
                    ShowMessage("Please select a valid .osz file.");
                    return;
                }

                try
                {
                    var converter = new OszExporter(inputPath);
                    var outputPath = converter.ExportFull();
                    ShowMessage($"Conversion complete! File saved to:\n{outputPath}");
                }
                catch (System.Exception ex)
                {
                    ShowMessage($"An error occurred during conversion:\n{ex.Message}");
                }
            };

            var selection = toggleList.TreeView.Selection;
            selection.Changed += (o, args) =>
            {
                var inputPath = filePicker.Filename;

                if (string.IsNullOrEmpty(inputPath))
                {
                    tryBeatmaps.Sensitive = false;
                    return;
                }

                if (selection.GetSelected(out ITreeModel model, out TreeIter iter))
                {
                    _selectedEntry = (string)model.GetValue(iter, 1);

                    tryBeatmaps.Sensitive = true;
                }
                else
                {
                    tryBeatmaps.Sensitive = false;
                }
            };

            tryBeatmaps.Clicked += (o, args) =>
            {
                if (!string.IsNullOrEmpty(_selectedEntry))
                {
                    tryBeatmaps.ExportSingleEntry(_exporter, _selectedEntry);
                }
            };
        }

        private string _selectedEntry;

        private void ShowMessage(string message)
        {
            var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, message);
            dialog.Run();
            dialog.Destroy();
        }
    }
}