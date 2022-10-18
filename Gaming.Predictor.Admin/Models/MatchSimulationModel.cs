using Gaming.Predictor.Blanket.Scoring;
using Gaming.Predictor.Contracts.Admin;
using Gaming.Predictor.Contracts.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gaming.Predictor.Admin.Models
{
    public class MatchSimulationModel
    {
        public Int32? MatchId { get; set; }
        public Int32? QuestionId { get; set; }
        public List<OptionsListControl> Options { get; set; }
        public List<QuestionsControl> Questions { get; set; }
        public List<MatchControl> Matches { get; set; }
    }

    public class OptionsListControl
    {
        public bool checkBox { get; set; }
        public Int32 Id { get; set; }
        public String OptionDescription { get; set; }
    }
    public class QuestionsControl
    {
        public Int32 Id { get; set; }
        public String QuestionDescription { get; set; }
    }


    public class MatchSimulationWorker
    {
        public MatchSimulationModel GetModel(Blanket.Simulation.Simulation simulationContext, Answers _Answers,
           MatchSimulationModel formModel, bool fetchData = false)
        {

            MatchSimulationModel model = new MatchSimulationModel();
            model.Questions = new List<QuestionsControl>();
            model.Questions.Add(new QuestionsControl { Id = 0, QuestionDescription = "Select Question" });
            model.Options = new List<OptionsListControl>();
            model.Matches = new List<MatchControl>();
            model.Matches.Add(new MatchControl { Id = "0", MatchName = "Select Match", GamedayId = "0", MatchFile = "0" });

            if(formModel != null && formModel.MatchId != null && formModel.MatchId.Value > 0)
                model.MatchId = formModel.MatchId;

            if (formModel != null && formModel.QuestionId != null && formModel.QuestionId.Value > 0)
                model.QuestionId = formModel.QuestionId;

            List<Fixtures> mFixtures = new List<Fixtures>();

            #region " Match Dropdown "

            mFixtures = simulationContext.getFixtures();

            model.Matches.AddRange(mFixtures.Select(o => new MatchControl()
            {
                Id = o.MatchId.ToString(),
                MatchName = o.MatchId.ToString() + "-" + o.TeamAShortName + " vs " + o.TeamBShortName,
                GamedayId = o.GamedayId.ToString(),
                MatchFile = o.Matchfile
            }).ToList());

            #endregion

            #region " Questions Dropdown "
            if (formModel != null && formModel.MatchId != null && formModel.MatchId.Value > 0)
            {
                List<MatchQuestions> mQuestionsList = new List<MatchQuestions>();
                mQuestionsList = _Answers.GetAdminQUestions(formModel.MatchId.Value);

                if (mQuestionsList != null && mQuestionsList.Count > 0)
                    model.Questions.AddRange(mQuestionsList.Select(x => new QuestionsControl {
                        Id = x.QuestionId,
                        QuestionDescription = x.QuestionDesc
                    }).ToList());

            }
            #endregion

            #region " Options List "
            if (formModel != null && formModel.QuestionId != null && formModel.QuestionId.Value > 0)
            {
                List<MatchQuestions> mQuestionsList = new List<MatchQuestions>();
                mQuestionsList = _Answers.GetAdminQUestions(formModel.MatchId.Value);

                MatchQuestions selectedQuestion = mQuestionsList.Where(x => x.QuestionId == formModel.QuestionId.Value).FirstOrDefault();

                if (selectedQuestion != null && selectedQuestion.Options != null)
                {
                    List<Options> options = selectedQuestion.Options;
                    model.Options = options.Select(x => new OptionsListControl
                     {
                         Id = x.OptionId,
                         OptionDescription = x.OptionDesc.ToString()
                     }).ToList();
                }
            }
            #endregion

            return model;

        }
    }
}
