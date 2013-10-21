using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;

namespace AOExTra.Web
{
    public class WebConsoleWriter : TextWriter
    {
        HttpResponseBase Response { get; set; }

        public bool FlushAfterEveryWrite { get; set; }
        public bool AutoScrollToBottom { get; set; }

        Color BackgroundColor { get; set; }
        Color ForegroundColor { get; set; }

        public WebConsoleWriter(HttpResponseBase response, bool flushAfterEveryWrite, bool autoScrollToBottom)
        {
            Response = response;
            FlushAfterEveryWrite = flushAfterEveryWrite;
            AutoScrollToBottom = autoScrollToBottom;
        }

        public WebConsoleWriter(HttpResponseBase response, bool flushAfterEveryWrite,  bool autoScrollToBottom, Color backgroundColor, Color foregroundColor)
        {
            Response = response;
            FlushAfterEveryWrite = flushAfterEveryWrite;
            AutoScrollToBottom = autoScrollToBottom;
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
        }

        public virtual void WritePageBeforeStreamingText()
        {
            string headerFormat =
@"<!DOCTYPE html>
<html>
    <head>
        <title>Web Console</title>
        <style>
            html {{
                background-color: {0};
                color: {1};
            }}
        </style>        
    </head>
    <body><pre>";
            string backgroundColorHex = ColorTranslator.ToHtml(BackgroundColor);
            string foregroundColorHex = ColorTranslator.ToHtml(ForegroundColor);
            Response.Write(string.Format(headerFormat, backgroundColorHex, foregroundColorHex));

            Response.Write("<!--");
            Response.Write(new string('*', 256));
            Response.Write("-->");
            Response.Flush();
        }

        public virtual void WritePageAfterStreamingText()
        {
            Response.Write("</pre></body></html>");
        }

        public override void Write(string value)
        {
            string encoded = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            Response.Write(encoded);            
            if (FlushAfterEveryWrite)
                Response.Flush();
        }

        public override void WriteLine(string value)
        {
            Response.Write(value + "\n");
            if (AutoScrollToBottom)
                ScrollToBottom();
            if (FlushAfterEveryWrite)
                Response.Flush();
        }

        public override void WriteLine()
        {
            Response.Write('\n');
            if (AutoScrollToBottom)
                ScrollToBottom();
            if (FlushAfterEveryWrite)
                Response.Flush();            
        }

        public override void Flush()
        {
            Response.Flush();
        }

        public void ScrollToBottom()
        {
            Response.Write("<script>window.scrollTo(0, document.body.scrollHeight);</script>");
        }

        public override System.Text.Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }
    }
}