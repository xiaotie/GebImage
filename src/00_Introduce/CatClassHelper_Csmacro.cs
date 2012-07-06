using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Introduce
{
    using T = Cat;

    public static partial class CatClassHelper
    {
        

        public static unsafe void SetAge(this T item,  int* age)
        {
            item.Age = *age;
        }

        
    }
}
