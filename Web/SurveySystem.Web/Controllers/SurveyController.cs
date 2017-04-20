namespace SurveySystem.Web.Controllers
{
    using System.Web.Mvc;

    using SurveySystem.Web.Models.Survey;

    public class SurveyController : BaseController
    {
        [HttpGet]
        public ViewResult New()
        {
            return this.View();
        }

        [HttpPost]
        public ViewResult New(NewSurveyRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(request);
            }

            return this.View();
        }
    }
}