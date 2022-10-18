using System;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Common;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Gaming.Predictor.Contracts.Leaderboard;
using System.Linq;
using System.Collections.Generic;
namespace Gaming.Predictor.Blanket.Leaderboard
{
    class Utility : Common.BaseBlanket
    {
        public Utility(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
               : base(appSettings, aws, postgre, redis, cookies, asset)
        {
        }

        public ResponseObject FetchRecords(String data, Int32 from, Int32 to)
        {
            ResponseObject res = new ResponseObject();

            try
            {
                res = GenericFunctions.Deserialize<ResponseObject>(data);

                if (res != null)
                {
                    Top ranks = GenericFunctions.Deserialize<Top>(GenericFunctions.Serialize(res.Value));

                    if (ranks != null && ranks.Users != null && ranks.Users.Any())
                    {
                        List<Users> users = new List<Users>();

                        try
                        {
                            Int32 startIndex = from - 1;

                            if (ranks.Users.Count >= to)
                                users = ranks.Users.GetRange(startIndex, (to - startIndex));
                            else
                                users = ranks.Users.GetRange(startIndex, (ranks.Users.Count - startIndex));
                        }
                        catch
                        {
                            //Reaches here when list count is less than start index (starting point of record).
                            users = new List<Users>();
                        }

                        ranks.Users = users;
                    }

                    res.Value = ranks;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

    }
}
