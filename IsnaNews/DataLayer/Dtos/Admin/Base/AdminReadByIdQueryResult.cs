using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Base
{
    public class AdminReadByIdQueryResult<T> where T : class
    {
        /// <summary>
        /// Getinstance In No Errors
        /// </summary>
        /// <param name="result">Result Entity</param>
        public AdminReadByIdQueryResult(T result)
        {
            Result = result;
            Success = true;
        }
        /// <summary>
        /// Get Instance In List of Errors
        /// </summary>
        /// <param name="errors">List of Errors to show</param>

        public AdminReadByIdQueryResult(List<string> errors)
        {
            Result = null;
            Success = false;
            Error = errors;
        }

        /// <summary>
        /// Get Instance In Single Error
        /// </summary>
        /// <param name="errors">List of Errors to show</param>
        public AdminReadByIdQueryResult(string error)
        {
            Result = null;
            Success = false;
            Error.Add(error);
        }
        public bool Success { get; set; } = true;
        public List<string>? Error { get; set; } = new List<string>();
        public T? Result { get; set; }
    }
}
