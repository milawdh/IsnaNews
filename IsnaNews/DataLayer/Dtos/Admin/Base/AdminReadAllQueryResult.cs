using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Base
{
    public class AdminReadAllQueryResult<T> where T : class
    {
        ///<summary>
        /// Getinstance In No Errors
        ///</summary>
        ///<param name="result">Only List of Entities</param>
        public AdminReadAllQueryResult(List<T> Result)
        {
            Success = true;
            this.Result = Result;
        }

        public bool Success { get; set; } = true;
        public List<T> Result { get; set; } = new List<T>();

    }
}
