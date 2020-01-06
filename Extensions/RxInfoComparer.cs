using RxNS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Extensions
{
    public class RxInfoComparer : IEqualityComparer<RxInfo>
    {
        public bool Equals([AllowNull] RxInfo x, [AllowNull] RxInfo y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] RxInfo obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashProductId = obj.Id.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductId;

        }
    }
}
