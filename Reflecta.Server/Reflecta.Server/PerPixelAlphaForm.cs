#region Using

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

/// <summary>
///     Class that provides mappings from Win32 types to .NET types.
/// </summary>
internal class Win32
{
    public enum Bool
    {
        False = 0,
        True
    };

    public const Int32 ULW_COLORKEY = 0x00000001;
    public const Int32 ULW_ALPHA = 0x00000002;
    public const Int32 ULW_OPAQUE = 0x00000004;
    public const byte AC_SRC_OVER = 0x00;
    public const byte AC_SRC_ALPHA = 0x01;

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize,
        IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteObject(IntPtr hObject);

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public Int32 x;
        public Int32 y;

        public Point(Int32 x, Int32 y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public Int32 cx;
        public Int32 cy;

        public Size(Int32 cx, Int32 cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct ARGB
    {
        public readonly byte Blue;
        public readonly byte Green;
        public readonly byte Red;
        public readonly byte Alpha;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }
}

/// <summary>
///     A Form that supports per-pixel alpha rendering.
/// </summary>
public class PerPixelAlphaForm : Form
{
    /// <summary>
    ///     The per-pixel alpha form must have the WS_EX_LAYERED style.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x00080000; //WS_EX_LAYERED
            return cp;
        }
    }

    /// <summary>
    ///     Sets a Bitmap as background image for the current per-pixel alpha form.
    ///     An additional global opacity modifier is supported.
    ///     For best results, use .png file format images with 32 bits-per-pixel ARGB color data.
    /// </summary>
    /// <param name="bitmap">The background image for the current per-pixel alpha form.</param>
    /// <param name="opacity">
    ///     A global opacity modifier, range [0..255], with 0 being fully transparent and 255 being fully
    ///     opaque.
    /// </param>
    public void SetBitmap(Bitmap bitmap, byte opacity = byte.MaxValue)
    {
        if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

        //1. Create a compatible DC with screen;
        //2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
        //3. Call the UpdateLayeredWindow.

        var screenDc = Win32.GetDC(IntPtr.Zero);
        var memDc = Win32.CreateCompatibleDC(screenDc);
        var hBitmap = IntPtr.Zero;
        var oldBitmap = IntPtr.Zero;

        try
        {
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            oldBitmap = Win32.SelectObject(memDc, hBitmap);

            var size = new Win32.Size(bitmap.Width, bitmap.Height);
            var pointSource = new Win32.Point(0, 0);
            var topPos = new Win32.Point(Left, Top);
            var blend = new Win32.BLENDFUNCTION();
            blend.BlendOp = Win32.AC_SRC_OVER;
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = opacity;
            blend.AlphaFormat = Win32.AC_SRC_ALPHA;

            Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend,
                Win32.ULW_ALPHA);
        }
        finally
        {
            Win32.ReleaseDC(IntPtr.Zero, screenDc);
            if (hBitmap != IntPtr.Zero)
            {
                Win32.SelectObject(memDc, oldBitmap);
                Win32.DeleteObject(hBitmap);
            }
            Win32.DeleteDC(memDc);
        }
    }
}

/// <summary>
///     A draggable, non-resizeable per-pixel alpha form.
/// </summary>
public class CustomPerPixelAlphaForm : PerPixelAlphaForm
{
    /// <summary>
    ///     Called whenever the form receives a message.
    ///     If the form has Enabled set to true, it must respond to WM_NCHITTEST mouse drag events and it must ignore
    ///     WM_NCLBUTTONDBLCLK mouse double click events.
    /// </summary>
    /// <param name="m">The message sent to the form for processing.</param>
    protected override void WndProc(ref Message m)
    {
        if (Enabled)
        {
            switch (m.Msg)
            {
                //Let Windows drag this form...
                case 0x0084: //WM_NCHITTEST
                    m.Result = (IntPtr) 2; //HTCLIENT
                    break;

                //Prevent Windows from resizing this form...
                case 0x00A3: //WM_NCLBUTTONDBLCLK
                    m.Result = IntPtr.Zero;
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        else
            base.WndProc(ref m);
    }
}