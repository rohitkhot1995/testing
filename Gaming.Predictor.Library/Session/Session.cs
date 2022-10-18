using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Session;
using Gaming.Predictor.Library.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Library.Session
{
    public class Session
    {

        public static HTTPResponse ValidateUser(String ProfileUrl, Dictionary<String, String> vAuthHeaders, String vWafGUID)
        {

            HTTPResponse mHTTPResponse = new HTTPResponse();
            HTTPMeta mHTTPMeta = new HTTPMeta();
            try
            {

                string result = GenericFunctions.GetWebData(ProfileUrl, vAuthHeaders);

                String mMessage = " result:" + result;
                WAFResultDetails userData = GenericFunctions.Deserialize<WAFResultDetails>(result);

                if (!String.IsNullOrEmpty(userData.data.status) &&
                        userData.data.status == "1") 
                        // || userData.data.status == "2")
                {
                    Credentials credentials = new Credentials();

                    credentials.SocialId = userData.data.user_id; //AesCryptography.AesDecrypt(vWafGUID).Split('|')[0];
                                                                  //credentials.FullName = BareEncryption.BaseEncrypt(userData.data.user.name.ToString());
                    if (!String.IsNullOrEmpty(userData.data.user.last_name))
                        credentials.FullName = userData.data.user.first_name.Trim() + " " + userData.data.user.last_name.Trim();
                    else
                        credentials.FullName = userData.data.user.first_name.Trim();

                    credentials.EmailId = String.IsNullOrEmpty(userData.data.email_id) == false
                      ? BareEncryption.BaseEncrypt(userData.data.email_id.Trim().ToLower())
                      : "";
                    credentials.DOB = BareEncryption.BaseEncrypt(userData.data.user.dob);

                    if (!String.IsNullOrEmpty(userData.data.user.mobile_no))
                        credentials.PhoneNo = userData.data.user.mobile_no;
                    else
                        credentials.PhoneNo = "";

                    try
                    {
                        if (String.IsNullOrEmpty(userData.data.created_date) == false)
                        {
                            DateTime userCreateDateTime = new DateTime();
                            userCreateDateTime = DateTime.Parse(userData.data.created_date);

                            userCreateDateTime = DateTime.SpecifyKind(userCreateDateTime, DateTimeKind.Unspecified);

                            credentials.userCreatedDate = userCreateDateTime;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    mHTTPResponse.Data = credentials;
                    GenericFunctions.AssetMeta(1, ref mHTTPMeta, userData.data.user_guid + mMessage);
                }
                else
                {
                    if (!String.IsNullOrEmpty(userData.data.status) &&
                        userData.data.status == "2")
                    {
                        GenericFunctions.AssetMeta(2, ref mHTTPMeta, mMessage);
                    }
                    else
                    {
                        GenericFunctions.AssetMeta(-999, ref mHTTPMeta, mMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                GenericFunctions.AssetMeta(-40, ref mHTTPMeta, "Problem in Stumpped API." + ex.Message);
            }

            mHTTPResponse.Meta = mHTTPMeta;

            return mHTTPResponse;
        }
    }
}
