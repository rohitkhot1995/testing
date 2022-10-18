using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Session;
using System;

namespace Gaming.Predictor.Interfaces.Session
{
    public interface ICookies
    {
        bool _HasUserCookies { get; }
        UserCookie _GetUserCookies { get; }
        bool _HasGameCookies { get; }
        GameCookie _GetGameCookies { get; }


        WAFCookie _GetWAFURCCookies { get; }
        String _GetWAFUSCCookies { get; }
        bool _HasWAFURCCookies { get; }
        bool _HasWAFUSCCookies { get; }

        bool SetUserCookies(UserCookie uc);

        //bool UpdateUserCookies(UserCookie values);
        bool SetGameCookies(GameCookie gc);

        bool UpdateGameCookies(GameCookie values);
        HTTPLog PopulateLog(String FunctionName, String Message);

        void DeleteCookies();

        void DeleteMasterDemoCookies();
        bool SetGamingDemoCookie(MasterLoginCookie loginCookie);
        MasterLoginCookie _GetGamingDemoCookie { get; }
        bool _HasGamingDemoCookie { get; }
    }
}