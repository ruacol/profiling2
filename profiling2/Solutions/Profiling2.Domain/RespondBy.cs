using System;

namespace Profiling2.Domain
{
    /// <summary>
    /// Sortable object representing the RespondBy value of screening requests, in order to incorporate proper sorting of
    /// 'Immediately' RespondBy values.
    /// 
    /// Useful when used in conjunction with Mvc.DataTables' built-in DataTablesResult.Create().
    /// </summary>
    public class RespondBy : IComparable
    {
        protected string _respondBy { get; set; }
        protected int _respondByInt { get; set; }

        public RespondBy() { }

        public RespondBy(string respondBy)
        {
            this._respondBy = respondBy;

            if (!string.IsNullOrEmpty(respondBy))
            {
                if (string.Equals(respondBy, "Immediately"))
                {
                    this._respondByInt = 0;
                }
                else
                {
                    string x = respondBy.Replace("-", "").Replace(" ", "").Replace(":", "");
                    int i = 0;
                    if (int.TryParse(x, out i))
                        this._respondByInt = i;
                }
            }
        }

        public override string ToString()
        {
            return this._respondBy;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            RespondBy otherRespondBy = obj as RespondBy;
            if (otherRespondBy != null)
                return this._respondByInt.CompareTo(otherRespondBy._respondByInt);
            else
                throw new ArgumentException("Object is not a RespondBy");
        }
    }
}
