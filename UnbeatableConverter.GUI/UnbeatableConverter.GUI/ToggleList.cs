using System;
using System.Collections.Generic;
using System.ComponentModel;
using Eto.Forms;

namespace UnbeatableConverter.GUI;

/// <summary>
/// A grid view with checkboxes (Convert) and a name column (Beatmap), replacing the GtkSharp TreeView/ListStore.
/// </summary>
public class ToggleList
{
    public class BeatmapEntry : INotifyPropertyChanged
    {
        private bool _convert = true;
        private string _name = string.Empty;

        public bool Convert
        {
            get => _convert;
            set { _convert = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Convert))); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private readonly GridView _gridView;
    private readonly List<BeatmapEntry> _entries = new();

    public GridView GridView => _gridView;

    public event EventHandler<string?>? SelectionChanged;

    public ToggleList()
    {
        _gridView = new GridView
        {
            AllowMultipleSelection = false,
            ShowHeader = true,
            DataStore = _entries
        };

        _gridView.Columns.Add(new GridColumn
        {
            HeaderText = "Convert",
            Editable = true,
            Width = 70,
            DataCell = new CheckBoxCell(nameof(BeatmapEntry.Convert))
        });

        _gridView.Columns.Add(new GridColumn
        {
            HeaderText = "Beatmap",
            Expand = true,
            AutoSize = false,
            DataCell = new TextBoxCell(nameof(BeatmapEntry.Name))
        });

        _gridView.SelectionChanged += (s, e) =>
        {
            var sel = _gridView.SelectedItem as BeatmapEntry;
            SelectionChanged?.Invoke(this, sel?.Name);
        };
    }

    public void SetItems(string[] items)
    {
        _entries.Clear();
        foreach (var item in items)
            _entries.Add(new BeatmapEntry { Name = item, Convert = true });

        _gridView.DataStore = new List<BeatmapEntry>(_entries);
    }

    public string[] GetSelectedItems()
    {
        var result = new List<string>();
        foreach (var entry in _entries)
            if (entry.Convert)
                result.Add(entry.Name);
        return result.ToArray();
    }
}

