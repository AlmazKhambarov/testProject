using System;
using Microsoft.AspNetCore.Mvc;

namespace a.Controllers
{
	public class NotFoundPageController:Controller
	{
        public IActionResult Index() => View();
    }
}

