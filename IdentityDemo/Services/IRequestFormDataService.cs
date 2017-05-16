using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Services
{
    public interface IRequestFormDataService
    {
        List<SelectListItem> GetProvinces();


        
    }
}
