using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Process.NET.Native.Types;

namespace Process.NET.Windows
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    [SuppressMessage("Microsoft.Security", "CA2108:ReviewDeclarativeSecurityOnValueTypes")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ArrangeTypeMemberModifiers")]
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
    public struct Message
    {
#if DEBUG
        static TraceSwitch AllWinMessages = new TraceSwitch("AllWinMessages", "Output every received message");
#endif

        IntPtr hWnd;
        int msg;
        IntPtr wparam;
        IntPtr lparam;
        IntPtr result;

        public IntPtr HWnd
        {
            get { return hWnd; }
            set { hWnd = value; }
        }
        public int Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        public IntPtr WParam
        {
            get { return wparam; }
            set { wparam = value; }
        }

        public IntPtr LParam
        {
            get { return lparam; }
            set { lparam = value; }
        }

  
        public IntPtr Result
        {
            get { return result; }
            set { result = value; }
        }

        public object GetLParam(Type cls)
        {
            return Marshal.PtrToStructure(lparam, cls);
        }

        public static Message Create(IntPtr hWnd, WindowsMessages msg, IntPtr wparam, IntPtr lparam)
        {
            return Create(hWnd, (int) msg, wparam, lparam);
        }
    
    public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var m = new Message
            {
                hWnd = hWnd,
                msg = msg,
                wparam = wparam,
                lparam = lparam,
                result = IntPtr.Zero
            };

#if DEBUG
            if (AllWinMessages.TraceVerbose)
            {
                Debug.WriteLine(m.ToString());
            }
#endif
            return m;
        }

        public override bool Equals(object o)
        {
            if (!(o is Message))
            {
                return false;
            }

            var m = (Message) o;
            return hWnd == m.hWnd &&
                   msg == m.msg &&
                   wparam == m.wparam &&
                   lparam == m.lparam &&
                   result == m.result;
        }

        public static bool operator !=(Message a, Message b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Message a, Message b)
        {
            return a.Equals(b);
        }


        public override int GetHashCode()
        {
            return (int) hWnd << 4 | msg;
        }

        private static readonly CodeAccessPermission UnmanagedCode =
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        public override string ToString()
        {
            // ASSURT : 151574. Link Demand on System.Windows.Forms.Message 
            // fails to protect overriden methods.
            bool unrestricted = false;
            try
            {
                UnmanagedCode.Demand();
                unrestricted = true;
            }
            catch (SecurityException)
            {
                // eat the exception.
            }

            if (unrestricted)
            {

                return GetProperName(((WindowsMessages) Msg).ToString());
            }
            return base.ToString();
        }

        private static string GetProperName(string name)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (i > 0 && char.IsUpper(c))
                {
                    sb.Append(' ');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}