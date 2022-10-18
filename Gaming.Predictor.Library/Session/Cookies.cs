using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Gaming.Predictor.Library.Session
{
    public class Cookies : Logs, Interfaces.Session.ICookies
    {
        #region " PROPERTIES "

        public readonly string _UserCookey; 
        public readonly string _GameCookey; 
        private readonly int _ExpiryDays;
        private readonly string _Domain;
        public readonly string _WAFURCCookey;
        public readonly string _WAFUSCCookey;

        public readonly string _GamingDemoCookey = "master-login";

        public Cookies(IHttpContextAccessor httpContextAccessor, IOptions<Application> appSettings) : base(httpContextAccessor)
        {
            _UserCookey= appSettings.Value.Properties.ClientName+ "_007";
            _GameCookey= appSettings.Value.Properties.ClientName + "_RAW";

            _WAFURCCookey = appSettings.Value.Cookies.WAFURCCookie;
            _WAFUSCCookey = appSettings.Value.Cookies.WAFUSCCookie;

            _ExpiryDays = appSettings.Value.Cookies.ExpiryDays;
            _Domain = appSettings.Value.Cookies.Domain;
        }

        public bool _HasUserCookies
        {
            get
            {
                return (_HttpContextAccessor.HttpContext.Request.Cookies[_UserCookey] != null);
            }
        }

        public UserCookie _GetUserCookies
        {
            get
            {
                return GetUserCookies();
            }
        }

        public bool _HasGameCookies
        {
            get
            {
                return (_HttpContextAccessor.HttpContext.Request.Cookies[_GameCookey] != null);
            }
        }

        public GameCookie _GetGameCookies
        {
            get
            {
                return GetGameCookies();
            }
        }

        #endregion " PROPERTIES "

        #region " WAF Cookie "

        public WAFCookie _GetWAFURCCookies
        {
            get
            {
                return GetWAFURCCookies();
            }
        }
        public String _GetWAFUSCCookies
        {
            get
            {
                return GetWAFUSCCookies();
            }
        }

        public bool _HasWAFURCCookies
        {
            get
            {
                return (_HttpContextAccessor.HttpContext.Request.Cookies[_WAFURCCookey] != null);
            }
        }

        public bool _HasWAFUSCCookies
        {
            get
            {
                return (_HttpContextAccessor.HttpContext.Request.Cookies[_WAFUSCCookey] != null);
            }
        }

        #endregion

        #region " FUNCTIONS "

        #region " User Cookie "

        private UserCookie GetUserCookies()
        {
            String cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_UserCookey];

            if (!String.IsNullOrEmpty(cookie))
            {
                UserCookie uc = new UserCookie();

                try
                {
                    String DecryptedCookie = Encryption.BaseDecrypt(cookie);
                    uc = GenericFunctions.Deserialize<UserCookie>(DecryptedCookie);
                }
                catch { }

                return uc;
            }
            else
                return null;
        }

        public bool SetUserCookies(UserCookie uc)
        {
            bool set = false;

            try
            {
                String serializedCookie = GenericFunctions.Serialize(uc);
                String value = Encryption.BaseEncrypt(serializedCookie);

                SET(_UserCookey, value);

                set = true;
            }
            catch { }

            return set;
        }

        /*public bool UpdateUserCookies(UserCookie values)
        {
            bool set = false;
            try
            {
                UserCookie uc = new UserCookie();
                uc = GetUserCookies();

                if (uc != null)
                {
                    if (values != null && !String.IsNullOrEmpty(values.UserId))
                        uc.UserId = values.UserId;

                    set = SetUserCookies(uc);
                }
            }
            catch { }

            return set;
        }*/

        #endregion " User Cookie "

        #region " Game Cookie "

        private GameCookie GetGameCookies()
        {
            String cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_GameCookey];

            if (!String.IsNullOrEmpty(cookie))
            {
                GameCookie gc = new GameCookie();

                try
                {
                    gc = GenericFunctions.Deserialize<GameCookie>(cookie);
                }
                catch { }

                return gc;
            }
            else
                return null;
        }

        public bool SetGameCookies(GameCookie gc)
        {
            bool set = false;

            try
            {
                string value = GenericFunctions.Serialize(gc);

                SET(_GameCookey, value);

                set = true;
            }
            catch { }

            return set;
        }

        public bool UpdateGameCookies(GameCookie values)
        {
            bool set = false;
            try
            {
                GameCookie gc = new GameCookie();
                gc = GetGameCookies();

                if (gc != null)
                {
                    if (values != null && !String.IsNullOrEmpty(values.GUID))
                        gc.GUID = values.GUID;

                    if (values != null && !String.IsNullOrEmpty(values.WAF_GUID))
                        gc.WAF_GUID = values.WAF_GUID;

                    if (values != null && !String.IsNullOrEmpty(values.ClientId))
                        gc.ClientId = values.ClientId;

                    if (values != null && values.CoinTotal >=0)
                        gc.CoinTotal = values.CoinTotal;

                    set = SetGameCookies(gc);
                }
            }
            catch { }

            return set;
        }

        #endregion " Game Cookie "

        #region " WAF Cookie "

        private WAFCookie GetWAFURCCookies()
        {
            String cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_WAFURCCookey];

            if (!String.IsNullOrEmpty(cookie))
            {
                WAFCookie gc = null;

                try
                {
                    gc = GenericFunctions.Deserialize<WAFCookie>(cookie);
                }
                catch { }

                return gc;
            }
            else
                return null;
        }

        private String GetWAFUSCCookies()
        {
            String cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_WAFUSCCookey];

            if (!String.IsNullOrEmpty(cookie))
            {
                return cookie;
            }
            else
                return null;
        }

        #endregion

        private void SET(String key, String value, bool isSecure = false)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(_ExpiryDays);
            if (isSecure)
                option.Secure = true;
            //option.Domain = _Domain;

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }

        private void DELETE()
        {
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_UserCookey);
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_GameCookey);
        }

        public void DeleteCookies()
        {
            DELETE();
        }
        #endregion " FUNCTIONS "

        #region " Master Demo Login "

        public bool SetGamingDemoCookie(MasterLoginCookie loginCookie)
        {
            bool set = false;

            try
            {
                String serializedCookie = GenericFunctions.Serialize(loginCookie);
                String value = Encryption.BaseEncrypt(serializedCookie);

                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(_ExpiryDays);
                option.Secure = false;
                option.Domain = ".sportz.io";

                _HttpContextAccessor.HttpContext.Response.Cookies.Append(_GamingDemoCookey, value, option);

                set = true;
            }
            catch { }

            return set;
        }

        public void DeleteMasterDemoCookies()
        {
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_GamingDemoCookey);
        }

        public bool _HasGamingDemoCookie
        {
            get
            {
                return (_HttpContextAccessor.HttpContext.Request.Cookies[_UserCookey] != null);
            }
        }

        public MasterLoginCookie _GetGamingDemoCookie
        {
            get
            {
                return GetGamingDemoCookie();
            }
        }

        private MasterLoginCookie GetGamingDemoCookie()
        {
            String cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_GamingDemoCookey];

            if (!String.IsNullOrEmpty(cookie))
            {
                MasterLoginCookie uc = new MasterLoginCookie();

                try
                {
                    String DecryptedCookie = Encryption.BaseDecrypt(cookie);
                    uc = GenericFunctions.Deserialize<MasterLoginCookie>(DecryptedCookie);
                }
                catch { }

                return uc;
            }
            else
                return null;
        }


        #endregion
    }
}