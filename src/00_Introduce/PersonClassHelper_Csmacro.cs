using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Introduce
{
    using T = Person;

    public static partial class PersonClassHelper
    {
        

        public static unsafe void SetAge(this T item,  int* age)
        {
            item.Age = *age;
        }

         
    }
}
