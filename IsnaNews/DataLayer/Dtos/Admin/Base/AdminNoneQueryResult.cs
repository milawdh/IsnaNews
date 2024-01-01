using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Base
{
    public class AdminNoneQueryResult
    {
        /// <summary>
        /// Get Instance In No Errors
        /// </summary>
        public AdminNoneQueryResult()
        {
            
        }
        /// <summary>
        /// Get Instance In List of Errors
        /// </summary>
        /// <param name="errors">List of Errors to show</param>
        public AdminNoneQueryResult(List<string> Errors)
        {
            this.Error = Errors;
            Success = false;
        }
        /// <summary>
        /// Get Instance In Single Error
        /// </summary>
        /// <param name="error">String that will show in Errors</param>

        public AdminNoneQueryResult(string Error)
        {
            this.Error.Add(Error);
            Success = false;
        }
        public bool Success { get; set; } = true;
        public List<string>? Error { get; set; } = new List<string>();
    }
}
