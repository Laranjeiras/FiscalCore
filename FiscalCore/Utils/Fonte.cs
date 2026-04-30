using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace FiscalCore.Utils
{
    [SupportedOSPlatform("windows")]
    public static class Fonte
    {
        public static FontFamily CarregarDeByteArray(byte[] fonte, out PrivateFontCollection colecaoDeFonte)
        {
            var handle = GCHandle.Alloc(fonte, GCHandleType.Pinned);
            try
            {
                var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(fonte, 0);
                colecaoDeFonte = new PrivateFontCollection();
                colecaoDeFonte.AddMemoryFont(ptr, fonte.Length);
                return colecaoDeFonte.Families[0];
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
