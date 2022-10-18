using System;
using System.Collections.Generic;

namespace Gaming.Predictor.Contracts.Configuration
{
    public class Admin
    {
        public List<Authorization> Authorization { get; set; }
        public Feed Feed { get; set; }
        public String TemplateUri { get; set; }
        public String TemplateUriMobile { get; set; }
        public String WvTemplateUri { get; set; }
        public String UnavailableUri { get; set; }
        public String BasePath { get; set; }
    }

    public class Authorization
    {
        public String User { get; set; }
        public String Password { get; set; }
        public List<String> Pages { get; set; }
    }

    public class Feed
    {
        public String API { get; set; }
        public String Client { get; set; }
    }
}