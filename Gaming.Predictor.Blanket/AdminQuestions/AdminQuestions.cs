using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Enums;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaming.Predictor.Blanket.AdminQuestions
{
    public class AdminQuestions : Common.BaseBlanket
    {
        private readonly DataAccess.AdminQuestions.AdminQuestions _QuestionContext;
        private readonly Int32 _TourId;

        public AdminQuestions(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _QuestionContext = new DataAccess.AdminQuestions.AdminQuestions(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public Int32 SaveQuestions(MatchQuestions model)
        {
            Int32 matchId = model.MatchId;
            Int32 questionId = model.QuestionId; String questionDesc = model.QuestionDesc.Trim();
            String questionType = model.QuestionType;

            //Int32 questionStatus = model.QuestionStatus;
            Int32 coinMult = model.CoinMult;
            Int32 lastQstn = model.LastQstn;
            Int32 questionStatus = model.Status;
            List<int> optionIds = new List<int>(); List<String> optionDescs = new List<string>();
            List<int> isCorrects = new List<int>();
            Int32 retVal = -40;
            try
            {
                //model.QuestionDesc = model.QuestionDesc.Trim();
                Int32 i = 1;
                foreach (var option in model.Options)
                {
                    if (option.OptionDesc != String.Empty)
                    {
                        if (option.OptionId == 0)
                        {
                            option.OptionId = i;
                        }
                        optionIds.Add(option.OptionId);
                        optionDescs.Add(option.OptionDesc.Trim());
                        isCorrects.Add(option.IsCorrectBool ? 1 : 0);
                        i++;
                    }
                }
                Int32 optType = 1;
                retVal = _QuestionContext.SaveQuestions(optType, _TourId, matchId, questionId, questionDesc, questionType, questionStatus, optionIds.ToArray(), optionDescs.ToArray(), isCorrects.ToArray(),coinMult,lastQstn);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.SaveQuestions", ex.Message);
                _AWS.Log(httpLog);
            }
            return retVal;
        }

        public List<MatchQuestions> GetMatchQuestions(Int32 matchId)
        {
            Int32 retVal = -40;
            List<MatchQuestions> questions = new List<MatchQuestions>();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                Int32 optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetMatchQuestions", ex.Message);
                _AWS.Log(httpLog);
            }
            return questions;
        }

        public List<MatchQuestions> GetFilteredQuestions(Int32 matchId, String questionStatus)
        {
            Int32 retVal = -40;
            List<MatchQuestions> questions = new List<MatchQuestions>();
            List<MatchQuestions> filteredquestions = new List<MatchQuestions>();
            Int32 questionsStatusInt = Convert.ToInt32(questionStatus);
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                Int32 optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal).ToList();
                if (questionsStatusInt == Convert.ToInt32(QuestionStatus.Published) || questionsStatusInt == Convert.ToInt32(QuestionStatus.Locked))
                {
                    filteredquestions = questions.Where(a => a.Status == Convert.ToInt32(QuestionStatus.Published) || a.Status == Convert.ToInt32(QuestionStatus.Locked)).ToList();
                }
                else
                {
                    filteredquestions = questions.Where(a => a.Status == (questionsStatusInt == -2 ? a.Status : questionsStatusInt)).ToList();
                }

                filteredquestions.Reverse();

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetFilteredQuestions", ex.Message);
                _AWS.Log(httpLog);
            }
            return filteredquestions;
        }

        public MatchQuestions GetMatchQuestionsDetail(Int32 matchId, Int32 questionId)
        {
            Int32 retVal = -40;
            MatchQuestions questions = new MatchQuestions();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                Int32 optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal)
                    .Where(a => a.QuestionId == questionId).FirstOrDefault();

                foreach (var option in questions.Options)
                {
                    option.IsCorrectBool = option.IsCorrect == 1;
                }

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetMatchQuestionsDetail", ex.Message);
                _AWS.Log(httpLog);
            }
            return questions;
        }

        public Int32 AbandonMatch(Int32 abandonMatchId)
        {

            Int32 retVal = -40;
            try
            {
                //model.QuestionDesc = model.QuestionDesc.Trim();
                Int32 optType = 1;
                retVal = _QuestionContext.AbandonMatch(optType, _TourId, abandonMatchId);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.SaveQuestions", ex.Message);
                _AWS.Log(httpLog);
            }
            return retVal;
        }
    }
}
