using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using UnbeatableConverter.Core;

namespace UnbeatableConverter.GUI
{
    public partial class MainForm : Form
    {
        private OszExporter? _exporter;
        private string? _selectedEntry;

        public MainForm()
        {
            Title = "Unbeatable Converter";
            MinimumSize = new Size(480, 300);
            Resizable = true;

            var filePicker = new FilePicker();

            var toggleList = new ToggleList();

            var convertButton = new Button { Text = "Convert" };

            var tryBeatmaps = new WebsocketButton();
            tryBeatmaps.Enabled = false;

            var buttonRow = new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = 6,
                Items =
                {
                    new StackLayoutItem(convertButton, true),
                    tryBeatmaps
                }
            };

            Content = new StackLayout
            {
                Padding = new Padding(12),
                Spacing = 6,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Items =
                {
                    new StackLayoutItem(filePicker, HorizontalAlignment.Stretch),
                    new StackLayoutItem(toggleList.GridView, true),
                    new StackLayoutItem(buttonRow, HorizontalAlignment.Stretch)
                }
            };

            // File picker
            filePicker.SelectionChanged += (sender, e) =>
            {
                var inputPath = filePicker.Filename;
                if (!string.IsNullOrEmpty(inputPath))
                {
                    _exporter = new OszExporter(inputPath);
                    toggleList.SetItems(_exporter.BeatmapEntries.ToArray());
                }
            };

            // Convert button
            convertButton.Click += (sender, e) =>
            {
                var inputPath = filePicker.Filename;
                if (string.IsNullOrEmpty(inputPath))
                {
                    MessageBox.Show(this, "Please select a valid .osz file.", "Warning",
                        MessageBoxButtons.OK, MessageBoxType.Warning);
                    return;
                }

                try
                {
                    var converter = new OszExporter(inputPath);
                    var outputPath = converter.ExportFull();
                    MessageBox.Show(this, $"Conversion complete! File saved to:\n{outputPath}", "Done",
                        MessageBoxButtons.OK, MessageBoxType.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"An error occurred during conversion:\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxType.Error);
                }
            };

            // Toggle list selection
            toggleList.SelectionChanged += (sender, name) =>
            {
                var inputPath = filePicker.Filename;
                if (string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(name))
                {
                    tryBeatmaps.Enabled = false;
                    return;
                }

                _selectedEntry = name;
                tryBeatmaps.Enabled = true;
            };

            // Try button
            tryBeatmaps.Click += (sender, e) =>
            {
                if (_exporter != null && !string.IsNullOrEmpty(_selectedEntry))
                {
                    tryBeatmaps.ExportSingleEntry(_exporter, _selectedEntry);
                }
            };
        }
    }
}