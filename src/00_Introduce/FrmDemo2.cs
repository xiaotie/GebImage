using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Introduce
{
    public partial class FrmDemo2 : Form
    {
        public unsafe class UnmanagedMemory : IDisposable
        {
            public int Count { get; private set; }

            public byte* Handle { get; private set; }

            private bool _disposed = false;

            public UnmanagedMemory(int bytes)
            {
                Handle = (byte*) System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes);
                Count = bytes;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(true);
            }

            protected virtual void Dispose( bool isDisposing )
            {
                if (_disposed) return;
                if (isDisposing)
                {
                    if (Handle != null)
                    {
                        System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)Handle);
                    }
                }
                _disposed = true;
            }

            ~UnmanagedMemory()
 　         {
 　　           Dispose( false );
 　         }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Argb32
        {
            [FieldOffset(0)]
            public Byte Blue;
            [FieldOffset(1)]
            public Byte Green;
            [FieldOffset(2)]
            public Byte Red;
            [FieldOffset(3)]
            public Byte Alpha;

            [FieldOffset(0)]
            public Int32 IntVal;
        }
		

        public FrmDemo2()
        {
            InitializeComponent();
        }

        private void FrmDemo2_Load(object sender, EventArgs e)
        {
        }

        private void btnTest0_Click(object sender, EventArgs e)
        {
            Person p = new Person();
            p.SetAge(20);
            MessageBox.Show(p.Age.ToString());
        }

        private unsafe void btnTest1_Click(object sender, EventArgs e)
        {
            int age = 0;
            int* p = &age;
            *p = 20;
            MessageBox.Show(p->ToString());
        }

        private unsafe void btnTest2_Click(object sender, EventArgs e)
        {
            IntPtr handle = System.Runtime.InteropServices.Marshal.AllocHGlobal(4);
            Int32* p = (Int32*)handle;
            *p = 20;
            MessageBox.Show(p->ToString());
            System.Runtime.InteropServices.Marshal.FreeHGlobal(handle);
        }

        private unsafe void btnTest3_Click(object sender, EventArgs e)
        {
            using (UnmanagedMemory memory = new UnmanagedMemory(10))
            {
                int* p = (int*)memory.Handle;
                *p = 20;
                MessageBox.Show(p->ToString());
            }
        }

        private unsafe void btnTest4_Click(object sender, EventArgs e)
        {
            int* p = stackalloc int[10];
            for (int i = 0; i < 10; i++)
            {
                p[i] = 2 * i + 2;
            }
            MessageBox.Show(p[9].ToString());
        }

        private void btnTest5_Click(object sender, EventArgs e)
        {
            Argb32 p = new Argb32();
            p.Red = 255;
            MessageBox.Show(p.IntVal.ToString());
        }

    }
}
