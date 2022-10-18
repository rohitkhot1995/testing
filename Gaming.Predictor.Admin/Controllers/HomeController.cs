using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gaming.Predictor.Admin.Models;
using Gaming.Predictor.Interfaces.Admin;
using Gaming.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Contracts.Notification;
using System.Data;
using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Library.Utility;
using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Contracts.Enums;
using Gaming.Predictor.Blanket.Scoring;

namespace Gaming.Predictor.Admin.Controllers
{

    [Route("games/predictor/admin/")]
    public class HomeController : BaseController
    {
        private readonly Blanket.DataPopulation.Populate _PopulateDBContext;
        private readonly Blanket.Feeds.Ingestion _IngestionDBContext;
        private readonly Blanket.Management.Tour _TourContext;
        private readonly Blanket.Management.Series _SeriesContext;
        private readonly Blanket.Simulation.Simulation _SimulationContext;
        private readonly Blanket.BackgroundServices.GameLocking _GameLocking;
        //private readonly Blanket.Feeds.Ingestion _Ingestion;
        private readonly Blanket.Notification.Publish _NotificationPublishContext;
        private readonly Blanket.Scoring.PlayerStatistics _PlayerStatistics;
        private readonly Blanket.Leaderboard.Leaderbaord _LeaderBoard;
        private readonly Blanket.Analytics.Analytics _AnalyticsDBContext;
        private readonly Blanket.Feeds.Gameplay _GameplayBlanketContext;
        private readonly Blanket.AdminQuestions.AdminQuestions _QuestionsContext;
        private readonly Blanket.Scoring.Process _ScoringContext;
        private readonly Blanket.Template.Template _TemplateContext;
        private readonly Blanket.BackgroundServices.PointsCalculation  _PointCalculationContext;
        //private readonly DataAccess.Scoring.Answers _AnswersDB;
        private readonly Answers _Answers;
        private readonly Int32 _OptionsCount;
        private readonly Int32 _ShowPointProcessScreen;
        private readonly Int32 _ShowMatchAbandonScreen;
        private readonly Int32 _QuestionMultiplierEnabled;
        private readonly Int32 _IsLivePredictor;
        private readonly Int32 _TourId;

        public HomeController(IOptions<Application> appSettings, ISession session, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, session, aws, postgre, redis, cookies, asset)
        {
            _PopulateDBContext = new Blanket.DataPopulation.Populate(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _IngestionDBContext = new Blanket.Feeds.Ingestion(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _TourContext = new Blanket.Management.Tour(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _SeriesContext = new Blanket.Management.Series(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _SimulationContext = new Blanket.Simulation.Simulation(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _GameLocking = new Blanket.BackgroundServices.GameLocking(_AppSettings, null, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            // _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublishContext = new Blanket.Notification.Publish(appSettings, aws, postgre, redis, cookies, asset);
            //_TemplateContext = new Blanket.Template.Markup(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _PlayerStatistics = new Blanket.Scoring.PlayerStatistics(appSettings, aws, postgre, redis, cookies, asset);
            _LeaderBoard = new Blanket.Leaderboard.Leaderbaord(appSettings, aws, postgre, redis, cookies, asset);
            _AnalyticsDBContext = new Blanket.Analytics.Analytics(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _GameplayBlanketContext = new Blanket.Feeds.Gameplay(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _QuestionsContext = new Blanket.AdminQuestions.AdminQuestions(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _ScoringContext = new Blanket.Scoring.Process(appSettings, aws, postgre, redis, cookies, asset);

            _TemplateContext = new Blanket.Template.Template(_AppSettings, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _PointCalculationContext = new Blanket.BackgroundServices.PointsCalculation(_AppSettings, null, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _Answers = new Answers(appSettings, aws, postgre, redis, cookies, asset);
            //_AnswersDB = new DataAccess.Scoring.Answers(postgre);

            _OptionsCount = appSettings.Value.Properties.OptionCount;
            _ShowPointProcessScreen = appSettings.Value.Properties.ShowPointProcessScreen;
            _ShowMatchAbandonScreen = appSettings.Value.Properties.ShowMatchAbandonScreen;
            _QuestionMultiplierEnabled = appSettings.Value.Properties.QuestionMultiplierEnabled;
            _IsLivePredictor = appSettings.Value.Properties.IsLivePredictor;
            _TourId = appSettings.Value.Properties.TourId;
        }

        #region " LOGIN "

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string enc)
        {
            #region " Decryption "

            ViewBag.Enc = Library.Utility.GenericFunctions.DecryptedValue(enc);
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;
            #endregion

            if (_Session._HasAdminCookie)
                Response.Redirect(_AppSettings.Value.Admin.BasePath + _Session.Pages().FirstOrDefault().Replace(" ", ""));

            return View();
        }

        [HttpPost]
        [Route("login")]
        //[ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            if (ModelState.IsValid)
            {
                foreach (Authorization authority in _Admin.Authorization)
                {
                    if (model.Username.ToLower().Trim() == authority.User.ToLower().Trim() && model.Password == authority.Password)
                    {
                        bool status = _Session.SetAdminCookie(model.Username);

                        if (status)
                            Response.Redirect(_AppSettings.Value.Admin.BasePath + _Session.Pages(model.Username).FirstOrDefault().Replace(" ", ""));
                        else
                        {
                            ViewBag.MessageType = "_Error";
                            ViewBag.MessageText = "Session is Invalid.";
                        }
                    }
                    else
                    {
                        ViewBag.MessageType = "_Error";
                        ViewBag.MessageText = "Incorrect login credentials.";
                    }
                }
            }

            return View();
        }

        #endregion

        #region " DATA POPULATION "

        [HttpGet]
        [Route("datapopulation")]
        public IActionResult DataPopulation(Int32 tournament, Int32 series)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            DataPopulationModel model = new DataPopulationWorker().GetModel(_TourContext, _SeriesContext, tournament, series);
            return View(model);
        }

        [HttpPost]
        [Route("datapopulation")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DataPopulation(DataPopulationModel model, String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 tournament = model.TournamentId;
            Int32 series = model.SeriesId;
            String league = model.League ?? "";
            Int32 retVal = -40;

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "tournament":
                        retVal = _PopulateDBContext.SaveTournament(model.League);
                        break;
                    case "series":
                        retVal = _PopulateDBContext.SaveSeries(model.League);
                        break;
                    case "fixtures":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.SaveFixtures(tournament, series, league);
                        else
                            retVal = -30;
                        break;
                    case "players":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.SavePlayers(tournament, series, league);
                        else
                            retVal = -30;
                        break;
                    case "teams":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.SaveTeams(tournament, series, league);
                        else
                            retVal = -30;
                        break;
                    case "fixturesmapping":
                        if (tournament != 0 && series != 0)
                            retVal = await _PopulateDBContext.FixturesMapping(tournament, series);
                        else
                            retVal = -30;
                        break;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please select both tournament and series.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} populated successfully."
                            : $"Error while populating {process}. RetVal = {retVal}";
                    }
                }
            }

            //Doesn't need to pass tournament and series as it it already initialized in model object.
            DataPopulationModel dataModel = new DataPopulationWorker().GetModel(_TourContext, _SeriesContext, tournament, series);
            return View(dataModel);
        }

        #endregion

        #region " FEED INGESTION "

        [HttpGet]
        [Route("feedingestion")]
        public async Task<IActionResult> FeedIngestion()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            FeedIngestionModel mModel = new FeedIngestionModel();

            String data = await _Asset.GET(_Asset.EOTFlag(_TourId));
            mModel.EOTFlag = string.IsNullOrEmpty(data) ? 0 : Convert.ToInt32(data);

            Int32 DaemonServiceCombineLBStatus = 0;
            Int32 DaemonServiceStatus = 0;
            Int32 DaemonServiceMatchAnswerStatus = 0;
            try
            {
                string _daemonStatus = await _Asset.GET(_Asset.DaemonServiceCombineLB());

                DaemonServiceCombineLBStatus = string.IsNullOrEmpty(_daemonStatus) ? 0 : Convert.ToInt32(_daemonStatus);

                string _daemonCombineLBStatus = await _Asset.GET(_Asset.DaemonService());
                DaemonServiceStatus = string.IsNullOrEmpty(_daemonCombineLBStatus) ? 0 : Convert.ToInt32(_daemonCombineLBStatus);

                string _daemonMatchAnswerStatus = await _Asset.GET(_Asset.DaemonServiceMatchAnswer());
                DaemonServiceMatchAnswerStatus = string.IsNullOrEmpty(_daemonMatchAnswerStatus) ? 0 : Convert.ToInt32(_daemonMatchAnswerStatus);
            }
            catch (Exception)
            { }

            mModel.DaemonServiceStatus = DaemonServiceStatus;
            mModel.DaemonServiceCombineLbStatus = DaemonServiceCombineLBStatus;
            mModel.DaemonServiceMatchAnswerStatus = DaemonServiceMatchAnswerStatus;

            return View(mModel);
        }

        [HttpPost]
        [Route("feedingestion")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> FeedIngestion(FeedIngestionModel model, String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -40;

            Tuple<int, string> tuple = App_Code.Extension.DefaultInput();
            DateTime dt = DateTime.Now;


            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "languages":
                        retVal = await _IngestionDBContext.Languages();
                        break;
                    case "ingestmixapi":
                        retVal = await _IngestionDBContext.UpdateMixApiAdmin(_AppSettings.Value.Properties.TourId,
                            _ShowPointProcessScreen == 1 ? model.MixApiIsPointPrc.Value : 0, _ShowMatchAbandonScreen == 1 ? model.MixApiIsAbandon.Value : 0, model.MixApiIsMaintance.Value);
                        break;
                    case "fixtures":
                        {
                            retVal = await _IngestionDBContext.Fixtures();                            
                        }
                        break;
                    case "skills":
                        retVal = await _IngestionDBContext.Skills();
                        break;
                    case "questions":
                        if (model.QuestionsMatchID == null)
                            break;
                        retVal = await _IngestionDBContext.Questions(Convert.ToInt32(model.QuestionsMatchID));
                        break;
                    case "allmatchquestions":
                        retVal = await _IngestionDBContext.AllMatchQuestions();
                        break;
                    case "teamsrecentresults":
                        retVal = await _IngestionDBContext.GetRecentResults();
                        break;
                    case "matchstatus":
                        if (model.MatchId == null)
                            break;
                        retVal = await _IngestionDBContext.MatchInningStatus(Convert.ToInt32(model.MatchId));
                        break;
                    case "leaderboard":
                        if (model.LeaderBoardGamedayId == null && model.LeaderBoardPhaseId == null)
                            break;
                        retVal = await _IngestionDBContext.LeaderBoard(Convert.ToInt32(model.LeaderBoardGamedayId), Convert.ToInt32(model.LeaderBoardPhaseId));
                        break;
                    case "currentgamedaymatches":
                        retVal = await _IngestionDBContext.CurrentGamedayMatches();
                        break;
                    case "updateeotflag":
                        if (model.EOTFlag != null && model.EOTFlag.HasValue)
                            retVal = await _IngestionDBContext.UpdateEOTFlag(model.EOTFlag.Value);
                        else
                            retVal = -40;
                        break;
                    case "week_mapping_file":
                        retVal = await _IngestionDBContext.WeekMappingCombine();
                        break;
                    case "combineleaderboardweek":
                        if (model.CombineLeaderboardWeekId != null && model.CombineLeaderboardWeekId.HasValue)
                            retVal = await _IngestionDBContext.LeaderBoardCombineWeek(model.CombineLeaderboardWeekId.Value);
                        break;
                    case "combineleaderboardoverall":
                        retVal = await _IngestionDBContext.LeaderBoardCombineOverall();
                        break;
                    case "daemonservicecombinelbstatus":
                        {
                            if (model.DaemonServiceCombineLbStatus == null)
                                break;
                            await _Asset.SET(_Asset.DaemonServiceCombineLB(), model.DaemonServiceCombineLbStatus.Value.ToString(), serialize: false);
                            retVal = 1;
                        }
                        break;
                    case "daemonservicestatus":
                        {
                            if (model.DaemonServiceStatus == null)
                                break;
                            await _Asset.SET(_Asset.DaemonService(), model.DaemonServiceStatus.Value.ToString(), serialize: false);
                            retVal = 1;
                        }
                        break;
                    case "daemonservicematchanswerstatus":
                        {
                            if (model.DaemonServiceMatchAnswerStatus == null)
                                break;
                            await _Asset.SET(_Asset.DaemonServiceMatchAnswer(), model.DaemonServiceMatchAnswerStatus.Value.ToString(), serialize: false);
                            retVal = 1;
                        }
                        break;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
                else
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide the input values.";
                }
            }

            FeedIngestionModel mModel = new FeedIngestionModel();

            String data = await _Asset.GET(_Asset.EOTFlag(_TourId));
            mModel.EOTFlag = string.IsNullOrEmpty(data) ? 0 : Convert.ToInt32(data);

            Int32 DaemonServiceCombineLBStatus = 0;
            Int32 DaemonServiceStatus = 0;
            Int32 DaemonServiceMatchAnswerStatus = 0;
            try
            {
                string _daemonStatus = await _Asset.GET(_Asset.DaemonServiceCombineLB());

                DaemonServiceCombineLBStatus = string.IsNullOrEmpty(_daemonStatus) ? 0 : Convert.ToInt32(_daemonStatus);

                string _daemonCombineLBStatus = await _Asset.GET(_Asset.DaemonService());
                DaemonServiceStatus = string.IsNullOrEmpty(_daemonCombineLBStatus) ? 0 : Convert.ToInt32(_daemonCombineLBStatus);

                string _daemonMatchAnswerStatus = await _Asset.GET(_Asset.DaemonServiceMatchAnswer());
                DaemonServiceMatchAnswerStatus = string.IsNullOrEmpty(_daemonMatchAnswerStatus) ? 0 : Convert.ToInt32(_daemonMatchAnswerStatus);
            }
            catch (Exception)
            { }

            mModel.DaemonServiceStatus = DaemonServiceStatus;
            mModel.DaemonServiceCombineLbStatus = DaemonServiceCombineLBStatus;
            mModel.DaemonServiceMatchAnswerStatus = DaemonServiceMatchAnswerStatus;

            return View(mModel);
        }

        #endregion

        #region " SIMULATION "

        [HttpGet]
        [Route("simulation")]
        public async Task<IActionResult> Simulation()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            SimulationModel model = new SimulationWorker().GetModel(_SimulationContext, null);

            Int32 DaemonServiceStatus = 0;
            string _daemonStatus = "";

            try
            {
                _daemonStatus = await _Asset.GET(_Asset.DaemonServiceCombineLB());

                DaemonServiceStatus = string.IsNullOrEmpty(_daemonStatus) ? 0 : Convert.ToInt32(_daemonStatus);
            }
            catch (Exception)
            {}

            model.DaemonServiceCombineLbStatus = DaemonServiceStatus;

            return View(model);
        }

        [HttpPost]
        [Route("simulation")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Simulation(SimulationModel model, String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -40;

            if (ModelState.IsValid)
            {
                try
                {
                    switch (process)
                    {
                        case "matchprocess":
                            retVal = _SimulationContext.SubmitMatchForProcess(Convert.ToInt32(model.MatchId));
                            break;
                        case "generateuser":
                            retVal = _SimulationContext.GenerarateUser(Convert.ToInt32(model.UserCount));
                            break;
                        case "generateuserpredictions":
                            retVal = _SimulationContext.GenerarateUserPredictioons(Convert.ToInt32(model.GenenrateUserPredictionMatchId), Convert.ToInt32(model.OptionId));
                            break;
                        //case "userpointprocess":
                        //    if (model.GamedayId == null)
                        //        break;
                        //    retVal = _SimulationContext.GenerarateUserPredictioons(Convert.ToInt32(model.GamedayId), Convert.ToInt32(model.matchdayId));
                        //    break;
                        case "masterdatarollback":
                            retVal = _SimulationContext.MasterdataRollback();
                            break;
                        case "userdatarollback":
                            retVal = _SimulationContext.UserdataRollback();
                            break;
                        case "matchlocking":
                            retVal = _GameLocking.Lock(1, Convert.ToInt32(model.LockMatchId), 0);
                            await IngestThis(Convert.ToInt32(model.LockMatchId));
                            await _IngestionDBContext.UpdateMixApiLeaderboard(1, Convert.ToInt32(model.LockMatchId));
                            break;
                        case "inninglocking":
                            if (model.InningId == null)
                                break;
                            retVal = _GameLocking.Lock(1, Convert.ToInt32(model.LockInningMatchId), Convert.ToInt32(model.InningId));
                            await IngestThis(Convert.ToInt32(model.LockInningMatchId));
                            break;
                        case "inningunlocking":
                            if (model.UnlockInningId != null)
                            {
                                retVal = _GameLocking.UnLock(1, Convert.ToInt32(model.UnlockInningMatchId), Convert.ToInt32(model.UnlockInningId));
                                await IngestThis(Convert.ToInt32(model.UnlockInningMatchId));
                            }
                            break;
                        case "submitMatchLineups":
                            retVal = _SimulationContext.SubmitMatchLineups(model.MatchFile);
                            break;
                        case "submitMatchToss":
                            retVal = _SimulationContext.SubmitMatchToss(model.TossMatchFile);
                            break;
                        case "matchanswers":
                            retVal = _SimulationContext.SubmitMatchAnswers(Convert.ToInt32(model.SubmitAnswerMatchId));
                            break;
                        case "pointcalculation":
                            retVal = _SimulationContext.RunPointCalculation();
                            break;
                        case "updatematchdatetime":
                            retVal = _SimulationContext.UpdateMatchDateTime(Convert.ToInt32(model.UpdateMatchId), model.UpdateMatchDateTime);
                            break;
                        case "abandonmatch":
                            if (model.AbandonMatchId == null)
                                break;
                            retVal = _QuestionsContext.AbandonMatch(Convert.ToInt32(model.AbandonMatchId));
                            await IngestThis(Convert.ToInt32(model.AbandonMatchId));
                            break;
                        case "daemonservicecombinelbstatus":
                            {
                                if (model.DaemonServiceCombineLbStatus == null)
                                    break;
                                await _Asset.SET(_Asset.DaemonServiceCombineLB(), model.DaemonServiceCombineLbStatus.Value.ToString(), serialize: false);
                                retVal = 1;
                            }
                            break;   
                        case "emptyweekleaderboard":
                            {
                                if (model.DeleteWeekLeaderboard == null)
                                    break;
                                await _Asset.SET(_Asset.LeaderBoardCombine(2, 0, model.DeleteWeekLeaderboard.Value), "");                                
                                retVal = 1;
                            }
                            break;
                        case "emptyoverallleaderboard":
                            {
                                await _Asset.SET(_Asset.LeaderBoardCombine(1, 0, 0), "");
                                retVal = 1;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Contracts.Common.HTTPLog httpLog = _Cookies.PopulateLog(" ADMIN SIMULATION ERROR: ", ex.Message);
                    _AWS.Log(httpLog);
                }

                ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                ViewBag.MessageText = (retVal == 1) ? $"{process} submited successfully."
                    : $"Error while ingesting {process}. RetVal = {retVal}";
            }
            model = new SimulationWorker().GetModel(_SimulationContext, null);

            Int32 DaemonServiceStatus = 0;
            string _daemonStatus = "";

            try
            {
                _daemonStatus = await _Asset.GET(_Asset.DaemonServiceCombineLB());

                DaemonServiceStatus = string.IsNullOrEmpty(_daemonStatus) ? 0 : Convert.ToInt32(_daemonStatus);
            }
            catch (Exception)
            { }

            model.DaemonServiceCombineLbStatus = DaemonServiceStatus;

            return View(model);
        }

        #endregion

        #region " Notification "

        [HttpGet]
        [Route("notification")]
        public IActionResult Notification()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            NotificationModel model = new NotificationWorker().GetModel();
            return View(model);
        }

        [HttpPost]
        [Route("notification")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Notification(NotificationModel model, String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -40;
            Boolean success = false;

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "ingesttopics":
                        retVal = await _IngestionDBContext.InsertTopics();
                        break;
                    case "sendnotification":
                        if (model.NotificationPlatformId == "0" || String.IsNullOrEmpty(model.NotificationText))
                            break;
                        List<NotificationPlatforms> listPlatform = new List<NotificationPlatforms>();
                        if (model.NotificationPlatformId == "1")
                            listPlatform.Add(NotificationPlatforms.Android);
                        else if (model.NotificationPlatformId == "2")
                            listPlatform.Add(NotificationPlatforms.IOS);
                        else if (model.NotificationPlatformId == "3")
                        {
                            listPlatform.Add(NotificationPlatforms.Android);
                            listPlatform.Add(NotificationPlatforms.IOS);
                        }
                        foreach (NotificationPlatforms p in listPlatform)
                        {
                            List<NotificationMessages> notificationList = new List<NotificationMessages>();
                            NotificationMessages n = new NotificationMessages();

                            n.EventId = (Int32)Contracts.Notification.NotificationEvents.Generic;
                            n.Language = "en";
                            n.Message = model.NotificationText;
                            n.Subject = "Generic";
                            notificationList.Add(n);
                            success = await _NotificationPublishContext.SendPushNotification(p, notificationList, false, model.NotificationMatch, model.NotificationMatch == null ? "overall" : "");
                            if (success)
                                retVal = 1;
                        }
                        break;
                    case "ingestnotificationmessages":
                        if (!String.IsNullOrEmpty(model.NotifcationTextJson))
                            retVal = await _IngestionDBContext.IngestNotificationMessages(model.NotifcationTextJson);
                        break; ;
                    case "notificationStatus":
                        retVal = await _IngestionDBContext.IngestNotificationStatus();
                        break;
                }

                ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                ViewBag.MessageText = (retVal == 1) ? $"{process} submited successfully."
                    : $"Error while ingesting {process}. RetVal = {retVal}";
            }
            model = new NotificationWorker().GetModel();
            return View(model);
        }

        #endregion

        #region " MatchAnswers "
        [HttpGet]
        [Route("matchanswers")]
        public IActionResult MatchAnswers(Int32 matchId, String matchFile)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            //NotificationModel model = new NotificationWorker().GetModel();
            //  MatchAnswersModel model = new MatchAnswersWorker().GetModel(_GameLocking, _PlayerStatistics, matchId, matchFile);
            MatchAnswersModel model = new MatchAnswersModel();
            return View(model);
        }

        #endregion

        #region " LOG ME OUT "

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            _Session.DeleteAdminCookie();
            Response.Redirect(_AppSettings.Value.Admin.BasePath + "Login");
            return Content("");
        }

        #endregion

        #region " LeaderBoard "
        [HttpGet]
        [Route("adminleaderboard")]
        public IActionResult AdminLeaderBoard()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            LeaderBoardModel model = new LeaderBoardWorker().GetModel(_LeaderBoard);
            return View(model);
        }

        [HttpPost]
        [Route("adminleaderboard")]
        //[ValidateAntiForgeryToken]
        public IActionResult AdminLeaderBoard(LeaderBoardModel model, String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -40;
            Reports Leaderboard = new Reports();
            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "getusertoprank":

                        if (model.GamedayId == null && model.PhaseId == null && model.TopUser == null)
                            retVal = -40;
                        else
                            retVal = _LeaderBoard.AdminLeaderBoard(Convert.ToInt32(model.TopUser), Convert.ToInt32(model.LeaderBoardTypeId), Convert.ToInt32(model.GamedayId), Convert.ToInt32(model.PhaseId), out Leaderboard);

                        break;
                }
                model = new LeaderBoardWorker().GetModel(_LeaderBoard);
                model.LeaderBoardList = Leaderboard.LeaderBoardList;

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
                else
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide the input values.";
                }
            }

            return View(model);
        }

        #endregion " LeaderBoard "

        #region " ANALYTICS "
        [HttpGet]
        [Route("analytics")]
        public IActionResult Analytics()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            String mError = String.Empty;
            String mAnalytics = _AnalyticsDBContext.GetAnalytics(ref mError);//.GetModel(_TourContext, _SeriesContext, tournament, series);

            ViewBag.HTML = mAnalytics;
            ViewBag.Error = mError;
            return View();
        }
        #endregion " ANALYTICS "

        #region " QUESTIONS "

        [HttpGet]
        [Route("matchschedule")]
        public IActionResult MatchSchedule(Int32 retVal = 1)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            if (retVal != 1)
            {
                ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                ViewBag.MessageText = (retVal == 1) ? $"Question saved successfully."
                    : $"Something went wrong, please select date again.";
            }

            Schedule schedule = new Schedule();
            schedule.Fixtures = new List<Contracts.Feeds.Fixtures>();
            schedule.MatchDate = DateTime.Now;
            return View(schedule);
        }

        [HttpPost]
        [Route("matchschedule")]
        public IActionResult MatchSchedule(Schedule schedule)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            if (ModelState.IsValid)
            {
                schedule.ShortMatchDate = schedule.MatchDate.ToString("MM/dd/yyyy");
                HTTPResponse response = _GameplayBlanketContext.GetFixtures("en").Result;
                List<Fixtures> fixtures = fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)response.Data).Value)); //GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Deserialize<ResponseObject>(GenericFunctions.Serialize(response.Data)).Value.ToString());
                schedule.Fixtures = fixtures.Where(a => GenericFunctions.ToUSCulture(a.Date).Date == schedule.MatchDate.Date).ToList();

                foreach (Fixtures fixture in schedule.Fixtures)
                {
                    fixture.Date = (fixture.Date.USFormatDate()).ToString("dd/MM/yyyy hh:mm:ss tt");
                }

                return View(schedule);
            }
            else
            {
                schedule.Fixtures = new List<Contracts.Feeds.Fixtures>();
                return View(schedule);
            }
        }

        [HttpGet]
        [Route("questions")]
        public IActionResult Questions(Int32 matchId, String header, String questionStatus = "-2", Int32 retVal = 0, String messageText = null)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            QuestionsModel model = new QuestionsModel(); //QuestionsWorker().GetModel();

            if (matchId == 0)
            {
                //return RedirectToAction("MatchSchedule", "Home", new { retVal = -1 });
                Response.Redirect(_AppSettings.Value.Admin.BasePath + "MatchSchedule?retVal=-1");
            }

            if (retVal != 0)
            {
                ViewBag.MessageType = retVal == 1 ? "_Success" : "_Error";
                ViewBag.MessageText = messageText;
            }

            model = new QuestionsWorker().GetModel(_SimulationContext, matchId);
            model.matchQuestions = new List<MatchQuestions>();
            model.MatchId = matchId;
            model.QuestionStatus = questionStatus;
            model.Header = header;

            model.matchQuestions = _QuestionsContext.GetFilteredQuestions(matchId, questionStatus);

            foreach (MatchQuestions questions in model.matchQuestions)
            {
                for (int i = 0; i < _OptionsCount; i++)
                {
                    if (i >= questions.Options.Count)
                    {
                        questions.Options.Add(new Contracts.Feeds.Options
                        {
                            QuestionId = questions.QuestionId,
                            OptionId = i,
                            OptionDesc = String.Empty
                        });
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [Route("questions")]
        public IActionResult Questions(QuestionsModel model)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            String questionStatus = model.QuestionStatus;
            Int32 matchId = model.MatchId;

            //return RedirectToAction("Questions", "Home", new { matchId = model.MatchId, questionStatus = model.QuestionStatus });
            return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&questionStatus={1}",
                                                                                model.MatchId, model.QuestionStatus));
        }

        [HttpGet]
        [Route("addeditquestions")]
        public IActionResult AddEditQuestions(Int32 matchId, Int32 questionId, String questionType, String header)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;
            ViewBag.QuestionMultiplierEnabled = _QuestionMultiplierEnabled.ToString();
            ViewBag.IsLivePredictor = _IsLivePredictor.ToString();

            MatchQuestions model = new MatchQuestions();
            model.MatchId = matchId;
            model.QuestionId = questionId;
            model.QuestionType = questionType;

            model.CoinMultiplier = new List<CoinMultiplier>();

            if (_QuestionMultiplierEnabled == 0 && questionId == 0)
            {
                model.CoinMultiplier.Add(new CoinMultiplier { Id = 2, Name = "2x", IsSelected = 1 });
            }
            else
            {
                model.CoinMultiplier.Add(new CoinMultiplier { Id = 2, Name = "2x", IsSelected = 1 });
                model.CoinMultiplier.Add(new CoinMultiplier { Id = 3, Name = "3x", IsSelected = 0 });
                model.CoinMultiplier.Add(new CoinMultiplier { Id = 4, Name = "4x", IsSelected = 0 });
                model.CoinMultiplier.Add(new CoinMultiplier { Id = 5, Name = "5x", IsSelected = 0 });
            }
            model.LstQstnList = new List<ListControl>();
            model.LstQstnList.Add(new ListControl { Id = 0, Name = "No", IsSelected = 1 });
            model.LstQstnList.Add(new ListControl { Id = 1, Name = "Yes", IsSelected = 0 });

            if (questionId == 0)
            {
                model.Options = new List<Contracts.Feeds.Options>();
            }
            else
            {
                model = _QuestionsContext.GetMatchQuestionsDetail(matchId, questionId);

                model.CoinMultiplier = new List<CoinMultiplier>();

                if (_QuestionMultiplierEnabled == 0 && model.QuestionType.ToUpper() == "QS_PRED")
                {
                    model.CoinMultiplier.Add(new CoinMultiplier { Id = 2, Name = "2x", IsSelected = 1 });
                }
                else if (_QuestionMultiplierEnabled == 0 && model.QuestionType.ToUpper() != "QS_PRED")
                {
                    model.CoinMultiplier.Add(new CoinMultiplier { Id = 1, Name = "1x", IsSelected = 1 });
                }
                else
                {
                    model.CoinMultiplier.Add(new CoinMultiplier { Id = 2, Name = "2x", IsSelected =  0});
                    model.CoinMultiplier.Add(new CoinMultiplier { Id = 3, Name = "3x", IsSelected = 0 });
                    model.CoinMultiplier.Add(new CoinMultiplier { Id = 4, Name = "4x", IsSelected = 0 });
                    model.CoinMultiplier.Add(new CoinMultiplier { Id = 5, Name = "5x", IsSelected = 0 });
                }

                if (_QuestionMultiplierEnabled == 1)
                { 
                    if (model.CoinMult == 0 || model.CoinMult == 1)
                        model.CoinMult = 2;
                    model.CoinMultiplier.Where(x => model.CoinMult == x.Id).Select(x => x.IsSelected == 1);
                }
                else if(_QuestionMultiplierEnabled == 0 && model.QuestionType.ToUpper() == "QS_PRED")
                {
                    model.CoinMult = 2;
                }
                model.LstQstnList = new List<ListControl>();
                model.LstQstnList.Add(new ListControl { Id = 0, Name = "No", IsSelected = 0 });
                model.LstQstnList.Add(new ListControl { Id = 1, Name = "Yes", IsSelected = 0 });

                model.LstQstnList.Where(x => model.LastQstn == x.Id).Select(x => x.IsSelected == 1);
            }

            for (int i = model.Options.Count; i < _OptionsCount; i++)
            {
                Contracts.Feeds.Options option = new Contracts.Feeds.Options();
                model.Options.Add(option);
            }

            ViewBag.Header = header;
            return View(model);
        }

        [HttpPost]
        [Route("addeditquestions")]
        public async  Task<IActionResult> AddEditQuestions(MatchQuestions model)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;
            ViewBag.QuestionMultiplierEnabled = _QuestionMultiplierEnabled.ToString();
            ViewBag.IsLivePredictor = _IsLivePredictor.ToString();

            Int32 retVal = -30;
            Int32 matchId = model.MatchId;
            Int32 questionId = model.QuestionId;
            String questionType = model.QuestionType;
            HTTPResponse response = _GameplayBlanketContext.GetFixtures("en").Result;
            List<Fixtures> fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)response.Data).Value));
            Fixtures fixture = fixtures.Where(a => a.MatchId == model.MatchId).FirstOrDefault();
            String header = fixture.TeamAName + " VS " + fixture.TeamBName;// TempData["Header"].ToString();
            String error = String.Empty;

            model.Options = model.Options.Where(a => a.OptionDesc != String.Empty && a.OptionDesc != null && a.OptionDesc != "").ToList();

            if (_QuestionMultiplierEnabled == 0)
            {
                model.CoinMult = 1;
            }
            if (_IsLivePredictor == 0)
            {
                model.LastQstn = 0;
            }

                if (model.Options.Count <= 1)
            {
                //return RedirectToAction("AddEditQuestions", "Home", new { matchId = model.MatchId, questionId = 0, questionType = model.QuestionType, header = header });
                return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("AddEditQuestions?matchId={0}&questionId={1}&questionType={2}&header={3}",
                                                                                model.MatchId, 0, model.QuestionType, header));
            }

            //model.Status = model.Options.Any(a => a.IsCorrectBool && model.QuestionType == "QS_PRED") ? Convert.ToInt32(QuestionStatus.Resolved) : model.Status;
            model.Status = model.Options.Any(a => a.IsCorrectBool) ? Convert.ToInt32(QuestionStatus.Resolved) : model.Status;

            //if (_QuestionMultiplierEnabled == 0)
            //    model.CoinMult = 1;
            retVal = _QuestionsContext.SaveQuestions(model);

            Int32 questionstatus;
            String successText;
            if (model.Status != Convert.ToInt32(QuestionStatus.Unpublished))
            {
                questionstatus = Convert.ToInt32(QuestionStatus.Published);
                successText = $"Question {((QuestionStatus)model.Status).ToString()} successfully.";
            }
            else
            {
                questionstatus = model.Status;
                successText = $"Question saved successfully.";
            }

            if (retVal == 1)
            {
                retVal = _IngestionDBContext.Questions(model.MatchId).Result;
                await _IngestionDBContext.UpdateMixApiLeaderboard(1, model.MatchId);
            }
            else
                error = "Error while saving questions. ";

            if (retVal == 1)
            {
                //return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionstatus.ToString(), retVal = retVal, messageText = successText });
                return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&header={1}&questionStatus={2}&retVal={3}&messageText={4}",
                                                                                matchId, header, questionstatus.ToString(), retVal, successText));
            }
            else
                error = "Error while Ingesting questions. ";

            ViewBag.Header = header;
            ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
            ViewBag.MessageText = (retVal == 1) ? $"Question saved successfully."
                : error + $"RetVal = {retVal}";

            if (model.Options.Count < _OptionsCount)
            {
                for (int i = model.Options.Count; i < _OptionsCount; i++)
                {
                    Contracts.Feeds.Options option = new Contracts.Feeds.Options();
                    model.Options.Add(option);
                }
            }

            return View(model);
        }

        [Route("editingquestionstatus")]
        public async Task<IActionResult> EditingQuestionStatus(Int32 matchId, Int32 questionId, Int32 questionStatus, String header)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -30;
            MatchQuestions matchQuestionModel = _QuestionsContext.GetMatchQuestionsDetail(matchId, questionId);
            matchQuestionModel.Status = questionStatus;
            retVal = _QuestionsContext.SaveQuestions(matchQuestionModel);

            if (retVal == 1)
                retVal = _IngestionDBContext.Questions(matchId).Result;
            if(Convert.ToInt32(QuestionStatus.Published) == questionStatus)
            {
                await _IngestionDBContext.UpdateMixApiLeaderboard(1, matchId);
            }

            String questionStatusString = ((QuestionStatus)questionStatus).ToString();
            String messageText = retVal == 1 ? $"Question {questionStatusString} successfully." : $"Error while {questionStatusString} question.";

            //if (retVal == 1)
            //    return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText });

            //return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText }); ;
            return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&header={1}&questionStatus={2}&retVal={3}&messageText={4}",
                                                                               matchId, header, questionStatus.ToString(), retVal, messageText));
        }

        [Route("sendnotification")]
        public IActionResult SendNotification(Int32 matchId, String notificationText, Int32 questionStatus, String header)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -30;
            List<NotificationPlatforms> listPlatform = new List<NotificationPlatforms>();

            listPlatform.Add(NotificationPlatforms.Android);
            listPlatform.Add(NotificationPlatforms.IOS);

            foreach (NotificationPlatforms p in listPlatform)
            {
                List<NotificationMessages> notificationList = new List<NotificationMessages>();
                NotificationMessages n = new NotificationMessages();
                Boolean success = false;

                n.EventId = (Int32)Contracts.Notification.NotificationEvents.Generic;
                n.Language = "en";
                n.Message = notificationText;
                n.Subject = "Generic";
                notificationList.Add(n);
                success = _NotificationPublishContext.SendPushNotification(p, notificationList, false, matchId).Result;
                if (success)
                    retVal = 1;
            }

            String questionStatusString = ((QuestionStatus)questionStatus).ToString();
            String messageText = retVal == 1 ? $"Notification sent successfully." : $"Error while sending notification";

            //if (retVal == 1)
            //    return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText });

            //return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = questionStatus.ToString(), retVal = retVal, messageText = messageText });
            return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&header={1}&questionStatus={2}&retVal={3}&messageText={4}",
                                                                               matchId, header, questionStatus.ToString(), retVal, messageText));
        }

        [Route("pointcalculation")]
        public async Task<IActionResult> PointCalculation(Int32 matchId, Int32 questionStatus, String header)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -30;
            String lang = "en";
            bool success = false;

            List<Fixtures> fixtures = new List<Fixtures>();
            Fixtures mMatchFixture = new Fixtures();
            HTTPResponse httpResponse = _GameplayBlanketContext.GetFixtures(lang).Result;


            if (httpResponse.Meta.RetVal == 1)
            {
                if (httpResponse.Data != null)
                    fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                mMatchFixture = fixtures.Where(c => c.MatchId == matchId).FirstOrDefault();
            }
            else
                throw new Exception("GetFixtures RetVal is not 1.");

            //MatchFeed mMatchFeed = _GameLocking.GetMatchScoresFeed(mMatchFixture.Matchfile.ToString());
            //if (mMatchFeed.Matchdetail.Status.ToLower().Trim() == "match ended")
            //{
             List<MatchQuestions> matchQuestions = _QuestionsContext.GetMatchQuestions(matchId);
            //if (mMatchFixture != null && !String.IsNullOrEmpty(mMatchFixture.Matchfile))
            //{

            if (matchQuestions != null && matchQuestions.Any(x => x.Status != Convert.ToInt32(QuestionStatus.Published) 
                && x.Status != Convert.ToInt32(QuestionStatus.Locked))) { 

                success = _PointCalculationContext.UserPointsProcessMatchdayUpdated(matchId);

                if (success && _ShowPointProcessScreen == 1)
                    await _IngestionDBContext.UpdateMixApiPointCalOn(_AppSettings.Value.Properties.TourId);

                    ViewBag.MessageType = success ? "_Success" : "_Error";
                    ViewBag.MessageText = success ? $"Points status updated successfully. MatchId = {matchId}"
                        : $"Error while updating points status. MatchId = {matchId}";
            }
            else
            {
                ViewBag.MessageType = "_Error";
                ViewBag.MessageText = "Please resolve all questions";
            }
            //}
            //}
            //else
            //{
            //    ViewBag.MessageType = "_Error";
            //    ViewBag.MessageText = "Match has not been ended yet.";
            //}
            //return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal = ViewBag.MessageType == "_Error" ? -1 : 1, messageText = ViewBag.MessageText });
            return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&header={1}&questionStatus={2}&retVal={3}&messageText={4}",
                                                                   matchId, header, Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), ViewBag.MessageType == "_Error" ? -1 : 1, ViewBag.MessageText));
        }

        public bool LockInning(Int32 matchId, Int32 inningNo)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 lockVal = 0, optType = 1;
            try
            {
                lockVal = _GameLocking.Lock(optType, matchId, inningNo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (lockVal == 1);
        }

        [Route("abandonmatch")]
        public async Task<IActionResult> AbandonMatch(Int32 matchId, String header)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -30;
            String error = String.Empty;

            retVal = _QuestionsContext.AbandonMatch(matchId);

            if (retVal == 1)
            {
                retVal = IngestThis(matchId).Result;
                if(_ShowMatchAbandonScreen == 1)
                await _IngestionDBContext.UpdateMixApiAbandon(_AppSettings.Value.Properties.TourId);
            }
            else
            {
                error = $"There was problem in abandoning MatchId = {matchId}";
                //return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal = retVal, messageText = error });
                return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&header={1}&questionStatus={2}&retVal={3}&messageText={4}",
                                                                   matchId, header, Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal, error));
            }

            String messageText = retVal == 1 ? $"MatchId = {matchId} abandoned successfully." :
                                                $"Error while ingesting Fixtures.";

            //return RedirectToAction("Questions", "Home", new { matchId = matchId, header = header, questionStatus = Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal = retVal, messageText = messageText });
            return Redirect(_AppSettings.Value.Admin.BasePath + String.Format("Questions?matchId={0}&header={1}&questionStatus={2}&retVal={3}&messageText={4}",
                                                                   matchId, header, Convert.ToInt32(QuestionStatus.Points_Calculation).ToString(), retVal, messageText));
        }

        #endregion " QUESTIONS "

        #region " UserDetailsReport "

        [HttpGet]
        [Route("userdetailsreport")]
        public IActionResult UserDetailsReport()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            return View();
        }

        [HttpPost]
        [Route("userdetailsreport")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDetailsReport(String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -40;
            String error = "";
            String _HTML = "";

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "generateuserdetailsreport":
                        _HTML = _AnalyticsDBContext.GetUserDetailsReport(ref error);

                        if(!String.IsNullOrEmpty(_HTML))
                        {
                           await _Asset.SET(_Asset.UserDetailsReport(), _HTML, false);
                        }
                        break;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
                else
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide the input values.";
                }
            }

            return View();
        }

        #endregion

        #region " TEMPLATE INGESTION "

        [HttpGet]
        [Route("templateingestion")]
        public async Task<IActionResult> TemplateIngestion(String LangCode)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            TemplateModel model = await new TemplateWorker().GetModel(_TemplateContext, _AppSettings.Value, LangCode);
            return View(model);
        }

        [HttpPost]
        [Route("templateingestion")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> TemplateIngestion(TemplateModel model, String process)
        {
            bool success = false;
            Int32 retVal = 1;
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;
            String LangCode = model.LangCode;

            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "PreHeaderTemplate":
                        success = await _TemplateContext.UpdatePreHeaderTemplate(model.PreHeaderTemplate);
                        retVal = success ? 1 : -40;
                        break;

                    case "PostFooterTemplate":
                        success = await _TemplateContext.updatePostFooterTemplate(model.PostFooterTemplate);
                        retVal = success ? 1 : -40;
                        break;

                    case "PostUnavailableViewTemplate":
                        success = await _TemplateContext.updateUnavilablePageTemplate(LangCode);
                        retVal = success ? 1 : -40;
                        break;

                    case "GamePlayPageMetaTags":
                        success = await _TemplateContext.UpdateHomeMeta(model.GamePlayPageMetaTags);
                        retVal = success ? 1 : -40;
                        break;

                    case "RulesPageMetaTags":
                        success = await _TemplateContext.UpdateRulesMeta(model.RulesPageMetaTags);
                        retVal = success ? 1 : -40;
                        break;

                    case "FAQPageMetaTags":
                        success = await _TemplateContext.UpdateFAQMeta(model.FAQPageMetaTags);
                        retVal = success ? 1 : -40;
                        break;

                    default:
                        retVal = -30;
                        break;
                }

                if (process == "PageTemplate")
                {
                    //success = await _TemplateContext.updatePageTemplate(LangCode, true);
                    success = await _TemplateContext.updatePageTemplate(LangCode, false);
                    retVal = success ? 1 : -40;
                }
                else if (process == "PageTemplateMobile")
                {
                    success = await _TemplateContext.updatePageTemplateMobile(LangCode, true);
                    //success = await _TemplateContext.updatePageTemplateMobile(LangCode, false);
                    retVal = success ? 1 : -40;
                }

                if (retVal != -40)
                {
                    if (retVal == -30)
                    {
                        ViewBag.MessageType = "_Info";
                        ViewBag.MessageText = "Please provide the input values.";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} ingested successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
            }

            TemplateModel dataModel = await new TemplateWorker().GetModel(_TemplateContext, _AppSettings.Value, LangCode);
            return View(dataModel);
        }

        #endregion " TEMPLATE INGESTION "

        #region " Match SIMULATION "

        [HttpGet]
        [Route("matchsimulation")]
        public async Task<IActionResult> MatchSimulation()
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            MatchSimulationModel mModel = new MatchSimulationModel();


            mModel = new MatchSimulationWorker().GetModel(_SimulationContext, _Answers, null);

            return View(mModel);
        }

        [HttpPost]
        [Route("matchsimulation")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> MatchSimulation(MatchSimulationModel model, String process)
        {
            ViewBag.BasePath = _AppSettings.Value.Admin.BasePath;

            Int32 retVal = -40;

            Tuple<int, string> tuple = App_Code.Extension.DefaultInput();
            DateTime dt = DateTime.Now;


            if (ModelState.IsValid)
            {
                switch (process)
                {
                    case "submit":
                        if (model.MatchId != null && model.MatchId.Value > 0
                            && model.QuestionId != null && model.QuestionId > 0
                            && model.Options.Where(x => x.checkBox == true).Count() > 0)
                                retVal = _SimulationContext.SubmitMatchAnswers(model.MatchId.Value, 
                                          model.QuestionId.Value,
                                          model.Options.Where(x => x.checkBox == true).Select(x => x.Id).ToList());
                        break;
                    case "questionanswerprocessupdate":
                        if(model.MatchId != null && model.MatchId.Value > 0)
                            retVal = _SimulationContext.QuestionAnswerProcessUpdate(model.MatchId.Value);
                        break;
                    //default:
                    //    model.Options = null;
                    //    break;
                }

                if (retVal != -40)
                {
                    if (process == "submit")
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"Answers submited successfully."
                            : $"Error while submiting aswers. RetVal = {retVal}";
                    }
                    else
                    {
                        ViewBag.MessageType = (retVal == 1) ? "_Success" : "_Error";
                        ViewBag.MessageText = (retVal == 1) ? $"{process} submited successfully."
                            : $"Error while ingesting {process}. RetVal = {retVal}";
                    }
                }
            }

            MatchSimulationModel mModel = new MatchSimulationModel();

            mModel = new MatchSimulationWorker().GetModel(_SimulationContext, _Answers, model);

            return View(mModel);
        }

        #endregion

        public async Task<int> IngestThis(Int32 matchId)
        {
            Int32 retval = await _IngestionDBContext.Fixtures();
            //retval = await _IngestionDBContext.MatchInningStatus(matchId);
            retval = await _IngestionDBContext.Questions(matchId);
            retval = await _IngestionDBContext.CurrentGamedayMatches();

            //bool success = await _IngestionDBContext.UpdateMixApiAbandon(_AppSettings.Value.Properties.TourId);
            //retval = Convert.ToInt32(success);

            return retval;
        }
        /*--------------------------------*/

        private void Toast(Tuple<int, string> tuple, String process, DateTime startTime)
        {
            if (tuple != null && tuple.Item1 != -30)
            {
                if (tuple.Item1 == -20)
                {
                    ViewBag.MessageType = "_Info";
                    ViewBag.MessageText = "Please provide input valuesdfsf.";
                }
                else
                {
                    String timeTaken = GenericFunctions.TimeDifference(startTime, DateTime.Now);

                    ViewBag.MessageType = (tuple.Item1 == 1) ? "_Success" : "_Error";
                    ViewBag.MessageText = (tuple.Item1 == 1) ? (process + $" process successful. [ Time taken: {timeTaken} ] " + ((!String.IsNullOrEmpty(tuple.Item2) && tuple.Item2.Trim() != "") ? tuple.Item2 : ""))
                        : $"{process} process error. [RetVal = {tuple.Item1}] [ Time taken: {timeTaken} ] {tuple.Item2}";
                }
            }
        }
    }
}
