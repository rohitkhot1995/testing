using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gaming.Predictor.Blanket.Template
{
    public class Template : Common.BaseBlanket
    {
        private readonly Int32 _TourId;
        private readonly List<String> _Lang;
        private readonly String _TemplateUri;
        private readonly String _TemplateUriMobile;
        private readonly String _WvTemplateUri;
        private readonly String _UnavailableUri;

        public Template(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _TourId = appSettings.Value.Properties.TourId;
            _Lang = appSettings.Value.Properties.Languages;
            _TemplateUri = appSettings.Value.Admin.TemplateUri;
            _TemplateUriMobile = appSettings.Value.Admin.TemplateUriMobile;
            _WvTemplateUri = appSettings.Value.Admin.WvTemplateUri;
            _UnavailableUri = appSettings.Value.Admin.UnavailableUri;
        }


        #region " Get Template "

        public async Task<String> GetPreHeaderTemplate()
        {
            return await _Asset.GET(_Asset.PreHeaderTemplate());
        }

        public async Task<String> GetPostFooterTemplate()
        {
            return await _Asset.GET(_Asset.PostFooterTemplate());
        }

        public async Task<String> GetPageTemplate(String lang, Int32 IsWebView)
        {
            return await _Asset.GET(_Asset.PageTemplate(lang, IsWebView));
        }

        public async Task<String> GetPageTemplateMobile(String lang, Int32 IsWebView)
        {
            return await _Asset.GET(_Asset.PageTemplateMobile(lang, IsWebView));
        }

        public async Task<String> GetUnavailablePageTemplate(String lang)
        {
            return await _Asset.GET(_Asset.PageUnavailableTemplate(lang));
        }

        #endregion " Get Template "

        #region " Ingest Template "

        public async Task<bool> UpdatePreHeaderTemplate(string data)
        {
            return await _Asset.SET(_Asset.PreHeaderTemplate(), data, false);
        }

        public async Task<bool> updatePostFooterTemplate(string data)
        {
            return await _Asset.SET(_Asset.PostFooterTemplate(), data, false);
        }

        public async Task<bool> updatePageTemplate(string lang, bool IsWebView)
        {
            String template = String.Empty;

            if (!IsWebView)
            {
                //web
                template = scrapeTemplate(_TemplateUri);

                template = template.Replace("</head>", await GetPreHeaderTemplate() + "</head>");
                template = template.Replace("</body>", "</body>" + await GetPostFooterTemplate());
                template = template.Replace("</myapp>", "<div id=\"container\"></div></myapp>");
                return await _Asset.SET(_Asset.PageTemplate(lang, 0), template, false);
            }
            else
            {
                template = scrapeTemplate(_WvTemplateUri);
                template = template.Replace("</head>", await GetPreHeaderTemplate() + "</head>");
                template = template.Replace("</body>", "</body>" + await GetPostFooterTemplate());
                template = template.Replace("</myapp>", "<div id=\"container\"></div></myapp>");

                return await _Asset.SET(_Asset.PageTemplate(lang, 1), template, false);
            }
        }

        public async Task<bool> updatePageTemplateMobile(string lang, bool IsWebView)
        {
            String template = String.Empty;

            if (!IsWebView)
            {
                //web
                template = scrapeTemplate(_TemplateUriMobile);
                template = template.Replace("</head>", await GetPreHeaderTemplate() + "</head>");
                template = template.Replace("</footer>", "</footer>" + await GetPostFooterTemplate());
                template = template.Replace("</myapp>", "<div id=\"root\"></div></myapp>");
                template = template.Replace("\"swiper\":", "// \"swiper\":");
                template = template.Replace("\"nicescroll\":", "// \"nicescroll\":");

                return await _Asset.SET(_Asset.PageTemplateMobile(lang, 0), template, false);
            }
            else
            {
                template = scrapeTemplate(_WvTemplateUri);
                template = template.Replace("</head>", await GetPreHeaderTemplate() + "</head>");
                template = template.Replace("</footer>", "</footer>" + await GetPostFooterTemplate());
                template = template.Replace("</myapp>", "<div id=\"root\"></div></myapp>");

                return await _Asset.SET(_Asset.PageTemplateMobile(lang, 1), template, false);
            }
        }

        public async Task<bool> updateUnavilablePageTemplate(string lang)
        {
            String template = String.Empty;
            try
            {
                template = scrapeTemplate(_UnavailableUri);
                //template = template.Replace("</head>", await GetPreHeaderTemplate() + "</head>");
                //template = template.Replace("</footer>", "</footer>" + await GetPostFooterTemplate());
                //stemplate = template.Replace("</myapp>", "<div id=\"root\">" + scrapeTemplate(_UnavailableUri) + " </div></myapp>");
            }
            catch (Exception ex)
            {
                template = ex.Message;
            }

            return await _Asset.SET(_Asset.PageUnavailableTemplate(lang), template, false);
        }

        #endregion " Ingest Template "

        #region " Ingest SEO Meta "

        #region " Update "

        public async Task<bool> UpdateHomeMeta(string data)
        {
            return await _Asset.SET(_Asset.SEOHome(), data, false);
        }

        public async Task<bool> UpdateRulesMeta(string data)
        {
            return await _Asset.SET(_Asset.SEORules(), data, false);
        }

        public async Task<bool> UpdateFAQMeta(string data)
        {
            return await _Asset.SET(_Asset.SEOFAQ(), data, false);
        }        

        #endregion

        #region " Get "

        public async Task<String> GetHomeMeta()
        {
            return await _Asset.GET(_Asset.SEOHome());
        }

        public async Task<String> GetRulesMeta()
        {
            return await _Asset.GET(_Asset.SEORules());
        }
        
        public async Task<String> GetFAQMeta()
        {
            return await _Asset.GET(_Asset.SEOFAQ());
        }

        #endregion

        #endregion

        #region " Scrape Template  "

        private String scrapeTemplate(String templateURI)
        {
            String data = String.Empty;
            templateURI = templateURI + "?v=" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            data = Library.Utility.GenericFunctions.GetWebData(templateURI);

            return data;
        }

        #endregion " Scrape Template  "
    }
}
