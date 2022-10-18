using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Library.Utility;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Gaming.Predictor.DataInitializer.Feeds
{
    public class Gameplay
    {
        public static ResponseObject InitializeFixtures(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject fixtures = new ResponseObject();
            DataSet ds = null;
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<Fixtures> nFixtures = new List<Fixtures>();

            List<Fixtures_Stats> mFixturesStats = new List<Fixtures_Stats>();


            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                Fixtures fixture = new Fixtures();

                                fixture.MatchId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_matchid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_matchid"].ToString());
                                fixture.TeamGamedayId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_team_gamedayid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_team_gamedayid"].ToString());
                                fixture.TourGamedayId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_tour_gamedayid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_tour_gamedayid"].ToString());
                                fixture.PhaseId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_phaseid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_phaseid"].ToString());
                                fixture.GamedayId = Convert.IsDBNull(ds.Tables[0].Rows[i]["gameday"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["gameday"].ToString());
                                fixture.Date = Convert.IsDBNull(ds.Tables[0].Rows[i]["matchdate"]) ? "" : ds.Tables[0].Rows[i]["matchdate"].ToString();
                                fixture.Deadlinedate = Convert.IsDBNull(ds.Tables[0].Rows[i]["deadline_date"]) ? "" : ds.Tables[0].Rows[i]["deadline_date"].ToString();

                                fixture.TeamA = Convert.IsDBNull(ds.Tables[0].Rows[i]["home_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["home_teamid"].ToString());
                                fixture.TeamAName = Convert.IsDBNull(ds.Tables[0].Rows[i]["home_team_name"]) ? "" : ds.Tables[0].Rows[i]["home_team_name"].ToString();
                                fixture.TeamAShortName = Convert.IsDBNull(ds.Tables[0].Rows[i]["home_team_shortname"]) ? "" : ds.Tables[0].Rows[i]["home_team_shortname"].ToString();
                                //fixture.TeamACountryCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["home_team_countrycode"]) ? "" : ds.Tables[0].Rows[i]["home_team_countrycode"].ToString();
                                //fixture.TeamAScore = Convert.IsDBNull(ds.Tables[0].Rows[i]["home_team_score"]) ? 0 : decimal.Parse(ds.Tables[0].Rows[i]["home_team_score"].ToString());

                                fixture.TeamB = Convert.IsDBNull(ds.Tables[0].Rows[i]["away_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["away_teamid"].ToString());
                                fixture.TeamBName = Convert.IsDBNull(ds.Tables[0].Rows[i]["away_team_name"]) ? "" : ds.Tables[0].Rows[i]["away_team_name"].ToString();
                                fixture.TeamBShortName = Convert.IsDBNull(ds.Tables[0].Rows[i]["away_team_shortname"]) ? "" : ds.Tables[0].Rows[i]["away_team_shortname"].ToString();
                                //fixture.TeamBCountryCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["away_team_countrycode"]) ? "" : ds.Tables[0].Rows[i]["away_team_countrycode"].ToString();
                                //fixture.TeamBScore = Convert.IsDBNull(ds.Tables[0].Rows[i]["away_team_score"]) ? 0 : decimal.Parse(ds.Tables[0].Rows[i]["away_team_score"].ToString());

                                fixture.MatchdayName = Convert.IsDBNull(ds.Tables[0].Rows[i]["matchdayname"]) ? "" : ds.Tables[0].Rows[i]["matchdayname"].ToString();
                                fixture.MatchStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["match_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["match_status"].ToString());

                                fixture.Is_Lineup_Process = Convert.IsDBNull(ds.Tables[0].Rows[i]["is_lineup_process"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["is_lineup_process"].ToString());
                                fixture.Is_Toss_Process = Convert.IsDBNull(ds.Tables[0].Rows[i]["is_toss_process"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["is_toss_process"].ToString());
                                fixture.Inning_1_BAT_Teamid = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_1_bat_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_1_bat_teamid"].ToString());
                                fixture.Inning_1_BWL_Teamid = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_1_bwl_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_1_bwl_teamid"].ToString());
                                fixture.Inning_2_BAT_Teamid = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_2_bat_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_2_bat_teamid"].ToString());
                                fixture.Inning_2_BWL_Teamid = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_2_bwl_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_2_bwl_teamid"].ToString());
                                fixture.Match_Inning_Status = Convert.IsDBNull(ds.Tables[0].Rows[i]["match_inning_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["match_inning_status"].ToString());
                                fixture.Matchfile = Convert.IsDBNull(ds.Tables[0].Rows[i]["matchfile"]) ? "" : ds.Tables[0].Rows[i]["matchfile"].ToString();
                                fixture.Venue = Convert.IsDBNull(ds.Tables[0].Rows[i]["venue"]) ? "" : ds.Tables[0].Rows[i]["venue"].ToString();
                                fixture.IsQuestionAnswerProcess = Convert.IsDBNull(ds.Tables[0].Rows[i]["is_question_answer_process"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["is_question_answer_process"].ToString());

                                fixture.MatchDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["matchdate"]) ? "" : String.IsNullOrEmpty(ds.Tables[0].Rows[i]["matchdate"].ToString()) 
                                                                                                                ? ds.Tables[0].Rows[i]["matchdate"].ToString()
                                                                                                                : GenericFunctions.ToUSCulture(ds.Tables[0].Rows[i]["matchdate"].ToString()).ToString("MM/dd/yy hh:mm:ss tt \"GMT\"");
                                //todo: need to check user prediction count
                                fixture.TotalPredQstn = Convert.IsDBNull(ds.Tables[0].Rows[i]["usr_pred_cnt"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["usr_pred_cnt"].ToString());
                                fixture.IsTBC = Convert.IsDBNull(ds.Tables[0].Rows[i]["tbc_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["tbc_status"].ToString());
                                fixture.TotalPredQstnNotation = "0"; //GenericFunctions.MemberNotation(fixture.TotalPredQstn);

                                //fixture.UserPlayedCount = 0;
                                mFixtures.Add(fixture);
                            }
                        }

                        //if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        //{
                        //    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        //    {
                        //        Fixtures_Stats fixtureStats = new Fixtures_Stats();

                        //        fixtureStats.Matchid = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_matchid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_matchid"].ToString());
                        //        fixtureStats.TotalPrdQstn = Convert.IsDBNull(ds.Tables[0].Rows[i]["totalpredqstn"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["totalpredqstn"].ToString());

                        //        mFixturesStats.Add(fixtureStats);
                        //    }
                        //}

                        //foreach(var fixture in mFixtures)
                        //{
                        //    fixture.TotalPredQstn = mFixturesStats.Where(x => x.Matchid == fixture.MatchId).Select(o => o.TotalPrdQstn).FirstOrDefault();
                        //    fixture.TotalPredQstnNotation = GenericFunctions.MemberNotation(fixture.TotalPredQstn);

                        //    nFixtures.Add(fixture);
                        //}


                        fixtures.Value = mFixtures;
                        fixtures.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fixtures;
        }

        public static ResponseObject InitializeSkills(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject skills = new ResponseObject();
            DataSet ds = null;
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            skills.Value = (from a in ds.Tables[0].AsEnumerable()
                                            select new Skills
                                            {
                                                SkillId = Convert.IsDBNull(a["cf_skillid"]) ? 0 : Int32.Parse(a["cf_skillid"].ToString()),
                                                SkillName = Convert.IsDBNull(a["skill_name"]) ? "" : a["skill_name"].ToString()
                                            }).ToList();
                        }

                        skills.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return skills;
        }

        public static ResponseObject InitializeQuestions(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject questions = new ResponseObject();
            DataSet ds = null;
            List<MatchQuestions> mQuestions = new List<MatchQuestions>();

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                MatchQuestions question = new MatchQuestions();

                                question.QuestionId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_questionid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_questionid"].ToString());
                                question.QuestionNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_no"].ToString());
                                question.MatchId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_matchid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_matchid"].ToString());
                                question.InningNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_no"].ToString());
                                question.QuestionDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_dec"]) ? "" : ds.Tables[0].Rows[i]["question_dec"].ToString();
                                question.Status = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_status"].ToString());
                                question.QuestionType = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_type"]) ? "" : ds.Tables[0].Rows[i]["question_type"].ToString();
                                question.QuestionCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_sub_type"]) ? "" : ds.Tables[0].Rows[i]["question_sub_type"].ToString();
                                question.QuestionOccurrence = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_occurrence"]) ? "" : ds.Tables[0].Rows[i]["question_occurrence"].ToString();
                                //question.OptionJson = Convert.IsDBNull(ds.Tables[0].Rows[i]["option_json"]) ? "" : ds.Tables[0].Rows[i]["option_json"].ToString();
                                List<OptionList> OptionLists = Convert.IsDBNull(ds.Tables[0].Rows[i]["option_json"]) ? new List<OptionList>() : GenericFunctions.Deserialize<List<OptionList>>(ds.Tables[0].Rows[i]["option_json"].ToString());
                                question.PublishedDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["locked_date"]) ? "" : ds.Tables[0].Rows[i]["locked_date"].ToString();
                                question.QuestionTime = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_time"]) ? "0" : Math.Truncate(Convert.ToDouble(ds.Tables[0].Rows[i]["question_time"].ToString())).ToString();
                                question.QuestionPoints = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_points"]) ? "0" : ds.Tables[0].Rows[i]["question_points"].ToString();
                                question.CoinMult = Convert.IsDBNull(ds.Tables[0].Rows[i]["coin_mult"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["coin_mult"]);
                                question.LastQstn = Convert.IsDBNull(ds.Tables[0].Rows[i]["is_last_que"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["is_last_que"]);

                                question.Options = OptionLists.Select(s => new Options
                                {
                                    QuestionId = s.cf_questionid,
                                    OptionId = s.cf_optionid,
                                    OptionDesc = s.option_dec,
                                    AssetId = s.cf_assetid,
                                    AssetType = s.asset_type,
                                    IsCorrect = s.is_correct == null ? 0 : Convert.ToInt32(s.is_correct),
                                    MinVal = s.min_val,
                                    MaxVal = s.max_val
                                }).ToList<Options>();

                                question.IsLiveQuestion = (!String.IsNullOrEmpty(question.QuestionOccurrence) && question.QuestionOccurrence.Trim().ToLower() == "ing") ? 1 : 0;

                                mQuestions.Add(question);
                            }
                        }

                        questions.Value = mQuestions;
                        questions.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return questions;
        }

        public static ResponseObject InitializeQuestionsV1(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject questions = new ResponseObject();
            DataSet ds = null;
            List<MatchQuestions> mQuestions = new List<MatchQuestions>();
            List<MatchQuestionPlayers> matchQuestionPlayers = new List<MatchQuestionPlayers>();

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {

                        #region " Match Players "

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                            {
                                List<String> teamIdList = new List<string>();
                                List<String> playerIdList = new List<string>();
                                List<String> playerSkillIdList = new List<string>();

                                object teamId = ds.Tables[1].Rows[i]["team_id"];
                                object playerId = ds.Tables[1].Rows[i]["cf_playerid"];
                                object playerSkillId = ds.Tables[1].Rows[i]["skill_id"];

                                IEnumerable enumerableTeamId = teamId as IEnumerable;
                                if (enumerableTeamId != null)
                                    foreach (object element in enumerableTeamId)
                                        teamIdList.Add(element.ToString());

                                IEnumerable enumerablePlayerId = playerId as IEnumerable;
                                if (enumerablePlayerId != null)
                                    foreach (object element in enumerablePlayerId)
                                        playerIdList.Add(element.ToString());

                                IEnumerable enumerableSkillId = playerSkillId as IEnumerable;
                                if (enumerableSkillId != null)
                                    foreach (object element in enumerableSkillId)
                                        playerSkillIdList.Add(element.ToString());


                                int count = 0;
                                foreach (string team in teamIdList)
                                {
                                    MatchQuestionPlayers player = new MatchQuestionPlayers();
                                    player.TeamId = team;
                                    player.PlayerId = playerIdList[count].ToString();
                                    player.SkillId = playerSkillIdList[count].ToString();
                                    matchQuestionPlayers.Add(player);
                                    count++;
                                }
                            }
                        }

                        #endregion

                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                MatchQuestions question = new MatchQuestions();

                                question.QuestionId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_questionid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_questionid"].ToString());
                                question.QuestionNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_no"].ToString());
                                question.MatchId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_matchid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_matchid"].ToString());
                                question.InningNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_no"].ToString());
                                question.QuestionDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_dec"]) ? "" : ds.Tables[0].Rows[i]["question_dec"].ToString();
                                question.Status = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_status"].ToString());
                                question.QuestionType = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_type"]) ? "" : ds.Tables[0].Rows[i]["question_type"].ToString();
                                question.QuestionCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_sub_type"]) ? "" : ds.Tables[0].Rows[i]["question_sub_type"].ToString();
                                question.QuestionOccurrence = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_occurrence"]) ? "" : ds.Tables[0].Rows[i]["question_occurrence"].ToString();
                                //question.OptionJson = Convert.IsDBNull(ds.Tables[0].Rows[i]["option_json"]) ? "" : ds.Tables[0].Rows[i]["option_json"].ToString();
                                List<OptionList> OptionLists = Convert.IsDBNull(ds.Tables[0].Rows[i]["option_json"]) ? new List<OptionList>() : GenericFunctions.Deserialize<List<OptionList>>(ds.Tables[0].Rows[i]["option_json"].ToString());
                                question.PublishedDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["locked_date"]) ? "" : ds.Tables[0].Rows[i]["locked_date"].ToString();
                                question.QuestionTime = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_time"]) ? "0" : Math.Truncate(Convert.ToDouble(ds.Tables[0].Rows[i]["question_time"].ToString())).ToString();
                                question.QuestionPoints = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_points"]) ? "0" : ds.Tables[0].Rows[i]["question_points"].ToString();
                                question.CoinMult = Convert.IsDBNull(ds.Tables[0].Rows[i]["coin_mult"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["coin_mult"]);
                                question.LastQstn = Convert.IsDBNull(ds.Tables[0].Rows[i]["is_last_que"]) ? 0 : Convert.ToInt32(ds.Tables[0].Rows[i]["is_last_que"]);

                                if (question.QuestionCode.ToLower() == "hig_scor_plyr" || question.QuestionCode.ToLower() == "wckt_tkn_plyr")
                                {
                                    question.Options = OptionLists.Select(s => new Options
                                    {
                                        QuestionId = s.cf_questionid,
                                        OptionId = s.cf_optionid,
                                        OptionDesc = s.option_dec,
                                        AssetId = s.cf_assetid,
                                        AssetType = s.asset_type,
                                        IsCorrect = s.is_correct == null ? 0 : Convert.ToInt32(s.is_correct),
                                        MinVal = s.min_val,
                                        MaxVal = s.max_val,
                                        TeamId = matchQuestionPlayers.Any(x => x.PlayerId == s.cf_assetid.ToString())
                                                 ? matchQuestionPlayers.Where(x => x.PlayerId == s.cf_assetid.ToString())
                                                                        .Select(c => c.TeamId).FirstOrDefault()
                                                 : "0",
                                        SkillId = matchQuestionPlayers.Any(x => x.PlayerId == s.cf_assetid.ToString())
                                                 ? matchQuestionPlayers.Where(x => x.PlayerId == s.cf_assetid.ToString())
                                                                        .Select(c => c.SkillId).FirstOrDefault()
                                                 : "0",
                                    }).ToList<Options>();

                                }
                                else
                                {
                                    question.Options = OptionLists.Select(s => new Options
                                    {
                                        QuestionId = s.cf_questionid,
                                        OptionId = s.cf_optionid,
                                        OptionDesc = s.option_dec,
                                        AssetId = s.cf_assetid,
                                        AssetType = s.asset_type,
                                        IsCorrect = s.is_correct == null ? 0 : Convert.ToInt32(s.is_correct),
                                        MinVal = s.min_val,
                                        MaxVal = s.max_val
                                    }).ToList<Options>();
                                }

                                question.IsLiveQuestion = (!String.IsNullOrEmpty(question.QuestionOccurrence) && question.QuestionOccurrence.Trim().ToLower() == "ing") ? 1 : 0;

                                mQuestions.Add(question);
                            }
                        }

                        questions.Value = mQuestions;
                        questions.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return questions;
        }

        public static ResponseObject InitializeGetPredictions(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject predictions = new ResponseObject();
            DataSet ds = null;
            UserPredictionResult predictionResults = new UserPredictionResult();
            //UserPredictionResult prediction = new UserPredictionResult();

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            //{
                            //    prediction.QuestionId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_questionid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_questionid"].ToString());
                            //    prediction.Question_No = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_no"].ToString());
                            //    prediction.OptionId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_optionid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_optionid"].ToString());
                            //    prediction.Points = Convert.IsDBNull(ds.Tables[0].Rows[i]["mh_points"]) ? 0 : Int64.Parse(ds.Tables[0].Rows[i]["mh_points"].ToString());
                            //    prediction.Rank = Convert.IsDBNull(ds.Tables[0].Rows[i]["mh_rank"]) ? 0 : Int64.Parse(ds.Tables[0].Rows[i]["mh_rank"].ToString());
                            //    predictionResults.Add(prediction);
                            //}
                            predictionResults.QuestionDetails = (from a in ds.Tables[0].AsEnumerable()
                                                                 select new QuestionDetails
                                                                 {
                                                                     QuestionId = Convert.IsDBNull(a["cf_questionid"]) ? 0 : Int32.Parse(a["cf_questionid"].ToString()),
                                                                     Question_No = Convert.IsDBNull(a["question_no"]) ? 0 : Int32.Parse(a["question_no"].ToString()),
                                                                     OptionId = Convert.IsDBNull(a["cf_optionid"]) ? 0 : Int32.Parse(a["cf_optionid"].ToString()),
                                                                 }
                                                               ).ToList();
                        }
                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            predictionResults.PointRank = (from a in ds.Tables[1].AsEnumerable()
                                                           select new PointsRank
                                                           {
                                                               Points = Convert.IsDBNull(a["mh_points"]) ? 0 : Convert.ToInt32(Math.Truncate(Convert.ToDouble((a["mh_points"].ToString())))),
                                                               Rank = Convert.IsDBNull(a["mh_rank"]) ? "0" : a["mh_rank"].ToString()
                                                           }
                                                           ).ToList();
                        }

                        predictions.Value = predictionResults;
                        predictions.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return predictions;
        }

        public static ResponseObject InitializeGameplays(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject predictions = new ResponseObject();
            DataSet ds = null;
            List<GameDays> gameDayIds = new List<GameDays>();
            //UserPredictionResult prediction = new UserPredictionResult();

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            gameDayIds = (from a in ds.Tables[0].AsEnumerable()
                                          select new GameDays
                                          {
                                              TourGamedayId = Convert.IsDBNull(a["cf_tour_gamedayid"]) ? 0 : Int32.Parse(a["cf_tour_gamedayid"].ToString()),
                                          }
                                         ).ToList();
                        }
                        predictions.Value = gameDayIds;
                        predictions.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return predictions;
        }

        public static ResponseObject InitializeGetRecentResults(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject RecentResults = new ResponseObject();
            DataSet ds = null;
            List<RecentResults> RecentResultsList = new List<RecentResults>();
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                RecentResults RecentResult = new RecentResults();
                                RecentResult.TeamID = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_teamid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_teamid"].ToString());
                                RecentResult.TotalPlayed = Convert.IsDBNull(ds.Tables[0].Rows[i]["total_played"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["total_played"].ToString());
                                RecentResult.TotalWin = Convert.IsDBNull(ds.Tables[0].Rows[i]["total_winn"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["total_winn"].ToString());
                                RecentResult.TotalLoss = Convert.IsDBNull(ds.Tables[0].Rows[i]["total_loss"]) ? 0 : Int64.Parse(ds.Tables[0].Rows[i]["total_loss"].ToString());
                                RecentResultsList.Add(RecentResult);
                            }
                        }
                        RecentResults.Value = RecentResultsList;
                        RecentResults.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RecentResults;
        }


        public static ResponseObject InitializeGetMatchInningStatus(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject InningStatuses = new ResponseObject();
            DataSet ds = null;
            List<InningStatus> InningStatusList = new List<InningStatus>();
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                InningStatus inningStatus = new InningStatus();
                                inningStatus.MatchId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_matchid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_matchid"].ToString());
                                inningStatus.MatchStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["match_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["match_status"].ToString());
                                inningStatus.MatchInningStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["match_inning_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["match_inning_status"].ToString());
                                InningStatusList.Add(inningStatus);
                            }
                        }
                        InningStatuses.Value = InningStatusList;
                        InningStatuses.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return InningStatuses;
        }

        public static ResponseObject InitializeGetUserProfile(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject UserData = new ResponseObject();
            DataSet ds = null;
            UserProfile userProfile = new UserProfile();

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            userProfile.UserDataPointsRankList = (from a in ds.Tables[0].AsEnumerable()
                                                                  select new UserDataPointsRank
                                                                  {
                                                                      EmailId = Convert.IsDBNull(a["email_id"]) ? "" : Encryption.AesDecrypt(a["email_id"].ToString()),
                                                                      PhoneNumber = Convert.IsDBNull(a["phoneno"]) ? "" : Encryption.AesDecrypt(a["phoneno"].ToString()),
                                                                      UserName = Convert.IsDBNull(a["user_name"]) ? "" : a["user_name"].ToString(),
                                                                      ProfilePicture = Convert.IsDBNull(a["user_profile_pic"]) ? "" : a["user_profile_pic"].ToString(),
                                                                      Points = Convert.IsDBNull(a["ov_points"]) ? 0 : Convert.ToInt32(Math.Truncate(Convert.ToDouble((a["ov_points"].ToString())))),
                                                                      Rank = Convert.IsDBNull(a["ov_rank"]) ? "0" : a["ov_rank"].ToString()
                                                                  }
                                                               ).ToList();
                        }
                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            userProfile.UserMatchDataList = (from a in ds.Tables[1].AsEnumerable()
                                                             select new UserMatchData
                                                             {
                                                                 MatchId = Convert.IsDBNull(a["cf_matchid"]) ? 0 : Convert.ToInt32(a["cf_matchid"].ToString()),
                                                                 Points = Convert.IsDBNull(a["cur_points"]) ? 0 : Convert.ToInt32(Math.Truncate(Convert.ToDouble((a["cur_points"].ToString())))),
                                                                 Rank = Convert.IsDBNull(a["cur_rank"]) ? "0" : a["cur_rank"].ToString(),
                                                                 HomeTeamId = Convert.IsDBNull(a["home_teamid"]) ? "" : a["home_teamid"].ToString(),
                                                                 HomeTeamName = Convert.IsDBNull(a["home_team_name"]) ? "" : a["home_team_name"].ToString(),
                                                                 HomeTeamShortName = Convert.IsDBNull(a["home_team_short_name"]) ? "" : a["home_team_short_name"].ToString(),
                                                                 AwayTeamId = Convert.IsDBNull(a["away_teamid"]) ? "" : a["away_teamid"].ToString(),
                                                                 AwayTeamName = Convert.IsDBNull(a["away_team_name"]) ? "" : a["away_team_name"].ToString(),
                                                                 AwayTeamShortName = Convert.IsDBNull(a["away_team_short_name"]) ? "" : a["away_team_short_name"].ToString(),
                                                                 Date = Convert.IsDBNull(a["matchdate"]) ? "" : a["matchdate"].ToString(),
                                                             }
                                                           ).ToList();
                        }

                        UserData.Value = userProfile;
                        UserData.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return UserData;
        }
    }
}