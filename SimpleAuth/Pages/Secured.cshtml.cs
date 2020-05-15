using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SimpleAuth.Pages
{
    public class SecuredModel : PageModel
    {
        private readonly ILogger<SecuredModel> _logger;

        public SecuredModel(ILogger<SecuredModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
