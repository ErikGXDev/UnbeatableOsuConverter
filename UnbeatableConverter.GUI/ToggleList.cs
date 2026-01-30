using System;
using System.Collections.Generic;
using Gtk;

namespace UnbeatableConverter.GUI;

public class ToggleList
{
    private TreeView _treeView;
    private ListStore _toggleStore;

    public TreeView TreeView => _treeView;

    public ToggleList()
    {
        _toggleStore = new ListStore(typeof(bool), typeof(string));

        _treeView = new TreeView();
        _treeView.Model = _toggleStore;

        var toggleRenderer = new CellRendererToggle();
        toggleRenderer.Toggled += OnToggle;

        var toggleColumn = new TreeViewColumn("Convert", toggleRenderer, "active", 0);
        _treeView.AppendColumn(toggleColumn);

        var textColumn = new TreeViewColumn("Beatmap", new CellRendererText(), "text", 1);
        _treeView.AppendColumn(textColumn);
    }

    private void OnToggle(object sender, ToggledArgs args)
    {
        TreePath path = new TreePath(args.Path);

        if (_toggleStore.GetIter(out TreeIter iter, path))
        {
            bool currentValue = (bool)_toggleStore.GetValue(iter, 0);
            _toggleStore.SetValue(iter, 0, !currentValue);
        }
    }

    public void SetItems(string[] items)
    {
        _toggleStore.Clear();

        foreach (var item in items)
        {
            _toggleStore.AppendValues(true, item);
        }
    }
}