using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServiceLayer.Utils
{
    public static class Extentions
    {
        /// <summary>
        /// Converts Normal Date to Persioan Date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToPersianDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            return persianCalendar.GetYear(date) + "/" + persianCalendar.GetMonth(date) + "/" + persianCalendar.GetDayOfMonth(date)
                + " " + persianCalendar.GetHour(date) + ":" + persianCalendar.GetMinute(date);
        }
        /// <summary>
        /// Formats ViewCount in 1000 base
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string FormatViewCount(long num)
        {
            if (num < 1000)
                return num.ToString();

            string result = ((double)num / 1000).ToString("0.0") + "K";
            return result;
        }
        /// <summary>
        /// Validates Models or Dtos by Attributes
        /// </summary>
        /// <param name="model"></param>
        /// <returns>a bool Validation Success and List of Validation Results</returns>
        public static (List<string> ErrorMessages, bool IsValid) ValidateModel(object? model)
        {
            var validationResults = new List<ValidationResult>();
            List<string> Errors = new List<string>();
            if (model == null)
            {
                Errors.Add("داده ای وجود ندارد");
                return (Errors, false);
            }
            try
            {
                var context = new ValidationContext(model, serviceProvider: null, items: null);
                bool isValid = Validator.TryValidateObject(model, context, validationResults, true);

                validationResults.ForEach(i =>
                {
                    Errors.Add(i.ErrorMessage);
                });
                return (Errors, isValid);
            }
            catch (Exception)
            {
                Errors = new List<string>() { "خطای نا شناخته.لطفا در پر کردن فیلد ها دقت کنید" };
                return (Errors, false);
            }
        }
        /// <summary>
        /// Hashes string to SHA512
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HashData(this string source)
        {
            SHA512 hasher = SHA512.Create();
            byte[] bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(source));
            var Result = "";
            foreach (byte b in bytes)
            {
                Result += b.ToString("x2");
            }
            return Result;
        }
        /// <summary>
        /// Decodes Jwt Token And Validates it's Claims
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static (ClaimsPrincipal claimsPrincipal, bool isAuthenticated) DecodeJwt(this string token, IConfiguration configuration)
        {
            var tokenValidateParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var validateResult = handler.ValidateToken(token, tokenValidateParameters, out _);
                var isAuthenticated = validateResult.Claims.Any(_ => _.Type == ClaimTypes.NameIdentifier);
                return (validateResult, isAuthenticated);
            }
            catch (Exception)
            {
                return (new ClaimsPrincipal(), false);
            }
        }
        /// <summary>
        /// Collapses too long Texts
        /// </summary>
        /// <param name="body"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToShortBody(this string body, int length)
        {
            string[] words = body.Split(' ');
            var Result = "";
            for (int i = 0; i < length; i++)
            {
                if (i == words.Length - 1)
                    break;
                Result += words[i] + " ";
            }
            Result += "...";
            return Result;
        }
        /// <summary>
        /// To demonstrate extraction of file extension from base64 string.
        /// </summary>
        /// <param name="base64String">base64 string.</param>
        /// <returns>Henceforth file extension from string.</returns>
        public static string GetFileExtension(this string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "R0lG":
                    return "gif";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
        public static bool IsBase64(this string base64String)
        {
            if (base64String.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
        public static bool IsAllowedFormat(this string base64String, string[] allowedFormats)
        {
            if (!base64String.IsBase64())
                return false;
            var FileFormat = base64String.GetFileExtension();
            if (FileFormat == string.Empty)
                return false;

            return allowedFormats.Any(_ => _ == FileFormat);
        }
        /// <summary>
        /// Builds DataTable With Given List and T type DisplayNameAttributes
        /// </summary>
        /// <typeparam name="T">To Get Class Properties DisplayeName</typeparam>
        /// <param name="data">Data List</param>
        /// <param name="TableName">The Table Name</param>
        /// <returns></returns>
        public static DataTable BuildDataTable<T>(IList<T> data,string TableName)
        {
            //Get properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //Hiding virtual properties
            .Where(p => !p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal && Attribute.IsDefined(p, typeof(DisplayNameAttribute))).ToArray();


            //Get column headers
            bool isDisplayNameAttributeDefined = false;
            string[] headers = new string[Props.Length];
            int colCount = 0;
            foreach (PropertyInfo prop in Props)
            {

                DisplayNameAttribute dna = (DisplayNameAttribute)Attribute.GetCustomAttribute(prop, typeof(DisplayNameAttribute));
                if (dna != null)
                    headers[colCount] = dna.DisplayName;

                colCount++;
                isDisplayNameAttributeDefined = false;
            }

            DataTable dataTable = new DataTable(typeof(T).Name);

            //Add column headers to datatable
            foreach (var header in headers)
                dataTable.Columns.Add(header);

            dataTable.Rows.Add(headers);

            //Add datalist to datatable
            foreach (T item in data)
            {
                object[] values = new object[Props.Length];
                for (int col = 0; col < Props.Length; col++)
                    values[col] = Props[col].GetValue(item, null);
                dataTable.Rows.Add(values);
            }

            dataTable.TableName = TableName;
            return dataTable;
        }
    }
}
