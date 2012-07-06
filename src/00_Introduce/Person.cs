using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Introduce
{
    public unsafe class Person
    {
        public int Age;

        public void SetAge(int age)
        {
            fixed (int* p = &Age)
            {
                *p = age;
            }
        }
    }
}
