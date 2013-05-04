using System;
using System.Collections.Generic;
using System.Text;

namespace Introduce.Hide
{
    using T = Person;

    public static class Template
    {
        #region mixin

        public static unsafe void SetAge(this T item,  int* age)
        {
            item.Age = *age;
        }

        #endregion
    }
}
