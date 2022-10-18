using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Session;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Gaming.Predictor.API.Controllers
{
    [Route("services/api/[controller]")]
    [ApiController]
    public class SessionController : BaseController
    {

        private readonly Blanket.Session.User _SessionContext;

        public SessionController(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset,
            Microsoft.AspNetCore.Http.IHttpContextAccessor httpContext)
            : base(appSettings, aws, postgre, redis, cookies, asset, httpContext)
        {
            _SessionContext = new Blanket.Session.User(appSettings, aws, postgre, redis, cookies, asset);
        }

        [Route("user/login")]
        [HttpPost]
        public async Task<ActionResult<HTTPResponse>> Login(String waf_guid)
        {

            HTTPResponse response;
            String mGUID = String.Empty;
            Credentials mCredentials = new Credentials();
            Credentials credentials = new Credentials();
            Dictionary<String, String> mAuthHeaders = new Dictionary<string, string>();

            String mUserYahooGuid = String.Empty; //credentials.SocialId;

            String mProfileUrl = String.Empty;

            #region " Validate WAF-USC Cookie "

            if (_Cookies._HasWAFUSCCookies)
            {

                //WAFCookie wAFCookie = _Cookies._GetWAFURCCookies;

                mUserYahooGuid = _Cookies._GetWAFUSCCookies;

                mGUID = String.IsNullOrEmpty(waf_guid) ? Guid.NewGuid().ToString() : waf_guid;
                mProfileUrl = _AppSettings.Value.Properties.WAFProfileUrl + "?token=" + mGUID;

            }
            else
                return Unauthorized();

            mAuthHeaders.Add("user_guid", mUserYahooGuid);
            mAuthHeaders.Add("auth", _AppSettings.Value.Properties.WAFAuthKey);
            mAuthHeaders.Add("User-Agent", _AppSettings.Value.Properties.WAFUserAgent);
            #endregion

            #region " User Authentication "


            response = Library.Session.Session.ValidateUser(mProfileUrl, mAuthHeaders, mUserYahooGuid);

            if (response.Meta.RetVal == -999)
            {
                return Ok(response);
            }
            else if (response.Meta.RetVal == 2)
            {
                //return Ok(response);
                return new RedirectResult(_AppSettings.Value.Redirect.ProfileIncomplete);
            }

            #endregion

            //mGUID = response.Meta.Message;

            mCredentials = (Credentials)response.Data;

            if (mCredentials == null)
                return Unauthorized();
            else
            {
                credentials.SocialId = mCredentials.SocialId;
                credentials.FullName = mCredentials.FullName;
                credentials.EmailId = mCredentials.EmailId;
                credentials.ClientId = 1;
                credentials.OptType = 1;
                credentials.PlatformId = 3;
                credentials.UserId = 0;
                credentials.DOB = mCredentials.DOB;
                credentials.PhoneNo = mCredentials.PhoneNo;
                credentials.userCreatedDate = mCredentials.userCreatedDate;

                #region " Validate SI Login "

                response = await _SessionContext.Login(credentials);
                ////response.Meta.Message = mUserYahooGuid;
                //if (response.Meta.RetVal == 1)
                //    return new RedirectResult(_AppSettings.Value.Redirect.PostLogin);
                //else
                //    return new RedirectResult(_AppSettings.Value.Redirect.PreLogin);
                ////return Ok(response);

                return Ok(response);

                #endregion
            }

        }

        ///// <summary>
        ///// Creates session for a user
        ///// </summary>
        ///// <param name="credentials">Payload</param>
        ///// <param name="backdoor"></param>
        ///// <returns></returns>
        //[Route("/user/login")]
        //[HttpPost]
        //public async Task<ActionResult<HTTPResponse>> Login(Credentials credentials, String backdoor = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (_Authentication.Validate(backdoor))
        //        {
        //            HTTPResponse response = await _SessionContext.Login(credentials, "");

        //            return Ok(response);
        //        }
        //        else
        //            return Unauthorized();
        //    }
        //    else
        //        return BadRequest();
        //}

        //[HttpPost("{userguid}/userphoneupdate")]
        //public ActionResult<HTTPResponse> UserPhoneUpdate(Int32 platformId, Int32 clientId, Int64 phoneNumber, string backdoor = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (_Authentication.Validate(backdoor))
        //        {
        //            HTTPResponse response = _SessionContext.UserPhoneUpdate(platformId, clientId, phoneNumber);
        //            return Ok(response);
        //        }
        //        else
        //            return Unauthorized();
        //    }
        //    else
        //        return BadRequest();
        //}



    }
}