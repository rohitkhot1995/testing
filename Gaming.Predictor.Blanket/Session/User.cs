using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Session;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gaming.Predictor.Blanket.Session
{
    public class User : Common.BaseBlanket
    {

        private readonly DataAccess.Session.User _DBContext;
        private readonly Int32 _TourId;

        private readonly Int32 _TnCVersion;
        private readonly Int32 _PrivacyPolicyVersion;

        public User(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Session.User(postgre);
            _TourId = appSettings.Value.Properties.TourId;

            _TnCVersion = appSettings.Value.Properties.TermsCondition;
            _PrivacyPolicyVersion = appSettings.Value.Properties.PrivacyPolicy;
        }


        public async Task<HTTPResponse> Login(Credentials credentials)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            ResponseObject res = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 mUserId = 0;

            bool success = false;

            GameCookie gameCookie = new GameCookie();
            UserCookie userCookie = new UserCookie();

            try
            {
                if (credentials.OptType == 2)
                {
                    if (_Cookies._HasUserCookies)
                    {
                        mUserId = _Cookies._GetUserCookies.UserId;
                    }
                }

                if (credentials.OptType == 1 && credentials.EmailId == null)
                {
                    credentials.EmailId = String.Empty;
                }

                UserLoginDBResp details = _DBContext.Login(credentials.OptType, credentials.PlatformId, _TourId, mUserId, credentials.SocialId, credentials.ClientId,
                    credentials.FullName, credentials.EmailId, credentials.PhoneNo, credentials.CountryCode, credentials.ProfilePicture, credentials.DOB,
                    credentials.userCreatedDate, _TnCVersion, _PrivacyPolicyVersion, ref httpMeta);

                if (httpMeta.RetVal == 1)
                {
                    gameCookie = new GameCookie()
                    {
                        GUID = details.Usrguid,
                        WAF_GUID = details.Usrguid,
                        ClientId = details.Usrclnid.ToString(),
                        CoinTotal = details.coinTotal == null ? 0 : details.coinTotal.Value,
                        SocialId = BareEncryption.BaseEncrypt(credentials.SocialId)
                };
                    userCookie = new UserCookie()
                    {
                        SocialId = credentials.SocialId,
                        UserId = details.Usrid == null ? 0 : details.Usrid.Value,
                        FullName = details.Usrname,
                        TeamId = details.Teamid == null ? 0 : details.Teamid.Value
                    };


                    success = _Cookies.SetGameCookies(gameCookie);
                    success = _Cookies.SetUserCookies(userCookie);
                }
                else if (httpMeta.RetVal == 3)
                {
                    GenericFunctions.AssetMeta(httpMeta.RetVal, ref httpMeta, "Email id already exists.");
                }
                else
                    GenericFunctions.AssetMeta(httpMeta.RetVal, ref httpMeta, "Error while fetching user details from database.");

                return OkResponse(gameCookie, httpMeta);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Session.User.Login", ex.Message);
                _AWS.Log(httpLog);

                return CatchResponse(ex.Message);
            }
        }

        public HTTPResponse UserPhoneUpdate(Int32 platformId, Int32 clientId, Int64 phoneNumber)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                if (_Cookies._HasUserCookies)
                {
                    Int32 UserId = _Cookies._GetUserCookies.UserId;
                    Int32 UserTourTeamId = _Cookies._GetUserCookies.TeamId;
                    Int32 OptType = 1;
                    httpResponse.Data = _DBContext.UserPhoneUpdate(OptType, platformId, _TourId, UserId, clientId, phoneNumber, ref httpMeta);

                }
                else
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Session.User.UserPhoneUpdate", ex.Message);
                _AWS.Log(httpLog);
            }
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

    }
}
