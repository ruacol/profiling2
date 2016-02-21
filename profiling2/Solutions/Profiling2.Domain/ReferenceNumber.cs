using System;

namespace Profiling2.Domain
{
    /// <summary>
    /// Sortable object representing the ReferenceNumber value of screening requests, in order to incorporate proper numerical sorting.
    /// </summary>
    public class ReferenceNumber : IComparable
    {
        protected string _referenceNumber { get; set; }
        protected int _referenceNumberInt { get; set; }

        public ReferenceNumber() { }

        public ReferenceNumber(string referenceNumber)
        {
            this._referenceNumber = referenceNumber;

            if (!string.IsNullOrEmpty(this._referenceNumber))
            {
                string[] parts = this._referenceNumber.Split('-');

                // Zero-pad the last part of the reference number.  Example: 2014-05-27-1 will become 2014052701
                if (parts.Length == 4 && parts[3].Length == 1)
                {
                    parts[3] = "0" + parts[3];
                }

                int i = 0;
                if (int.TryParse(string.Join("", parts), out i))
                {
                    this._referenceNumberInt = i;
                }
            }
        }

        public override string ToString()
        {
            return this._referenceNumber;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ReferenceNumber otherReferenceNumber = obj as ReferenceNumber;
            if (otherReferenceNumber != null)
                return this._referenceNumberInt.CompareTo(otherReferenceNumber._referenceNumberInt);
            else
                throw new ArgumentException("Object is not a ReferenceNumber");
        }
    }
}
