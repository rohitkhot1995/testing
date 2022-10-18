using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gaming.Predictor.Admin.Models
{
    public class TemplateModel
    {

        [Display(Name = "PreHeaderTemplate")]
        public string PreHeaderTemplate { get; set; }

        [Display(Name = "PostFooterTemplate")]
        public string PostFooterTemplate { get; set; }

        [Display(Name = "PostUnavailableTemplate")]
        public string PostUnavailableTemplate { get; set; }


        public string LangCode { get; set; }

        public List<LangList> LangList { get; set; }


        #region " SEO "

        [Display(Name = "GamePlayPageMetaTags")]
        public string GamePlayPageMetaTags { get; set; }
        [Display(Name = "RulesPageMetaTags")]
        public string RulesPageMetaTags { get; set; }
        [Display(Name = "FAQPageMetaTags")]
        public string FAQPageMetaTags { get; set; }

        #endregion
    }

    public class LangList
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    #region " Worker "

    public class TemplateWorker
    {
        public async Task<TemplateModel> GetModel(Blanket.Template.Template templateContext,
                                         Gaming.Predictor.Contracts.Configuration.Application applicationContext, System.String LangCode)
        {
            TemplateModel model = new TemplateModel();


            model.PreHeaderTemplate = await templateContext.GetPreHeaderTemplate();
            model.PostFooterTemplate = await templateContext.GetPostFooterTemplate();

            model.LangList = applicationContext.Properties.Languages.Select(o => new Models.LangList
            {
                Id = o.ToString(),
                Name = o.ToString()
            }).ToList();

            #region " SEO "

            model.GamePlayPageMetaTags = await templateContext.GetHomeMeta();
            model.RulesPageMetaTags = await templateContext.GetRulesMeta();
            model.FAQPageMetaTags = await templateContext.GetFAQMeta();

            #endregion

            return model;
        }        
    }

    #endregion   

}
