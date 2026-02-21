using System;
using System.Threading.Tasks;
using Eto.Forms;
using UnbeatableConverter.Core;
using WebSocketSharp;

namespace UnbeatableConverter.GUI;

public class WebsocketButton : Button
{
    public WebsocketButton()
    {
        Text = "Try";
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
            try
            {
                using var ws = new WebSocket("ws://localhost:5080");
                ws.Connect();
                ws.Send("play " + filePath);
            }
            catch (Exception ex)
            {
                Dialog.Show("Error", "Couldn't connect to game: " + ex.Message, Dialog.MessageType.Error);
            }
        });
    }
}

