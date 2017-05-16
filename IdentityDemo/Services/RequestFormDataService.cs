using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityDemo.Services
{
    public class RequestFormDataService : IRequestFormDataService
    {
        public static readonly IDictionary<string, string> Provinces = new Dictionary<string, string>
        {
            {"Alberta", "AB"},
            {"British Columbia", "BC"},
            {"Manitoba", "MB"},
            {"New Brunswick", "NB"},
            {"Newfoundland and Labrador", "NL"},
            {"Northwest Territories", "NT"},
            {"Nova Scotia", "NS"},
            {"Nunavut", "NU"},
            {"Ontario", "ON"},
            {"Prince Edward Island", "PE"},
            {"Quebec", "QC"},
            {"Saskatchewan", "SK"},
            {"Yukon", "YT"}
        };

        public List<SelectListItem> GetProvinces()
        {
            return new SelectList(Provinces, "Value", "Key").ToList();
        }
    }
}
