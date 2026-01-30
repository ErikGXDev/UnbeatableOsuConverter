using System;
using System.Threading.Tasks;
using Gtk;
using UnbeatableConverter.Core;
using WebSocketSharp;

namespace UnbeatableConverter.GUI;

public class WebsocketButton : Button
{
    public WebsocketButton() : base("Try")
    {
    }

    public void ExportSingleEntry(OszExporter exporter, string entry)
    {
        var tempPath = System.IO.Path.GetTempPath();
        var outputPath = exporter.ExportSingle(entry, tempPath);
        SendPlayMessage(outputPath);
    }

    public void SendPlayMessage(string filePath)
    {
        Task.Run(() =>
        {
            using (var ws = new WebSocket("ws://localhost:5080"))
            {
                ws.Connect();
                ws.Send("play " + filePath);
                ws.OnError += (sender, args) =>
                {
                    Dialog.Show("Error", "Couldn't connect to game: " + args.Message, MessageType.Error);
                };
            }
        });
    }
}