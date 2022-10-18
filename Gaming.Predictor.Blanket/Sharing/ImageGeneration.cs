﻿using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Contracts.Configuration;
using Gaming.Predictor.Contracts.Feeds;
using Gaming.Predictor.Interfaces.Asset;
using Gaming.Predictor.Interfaces.AWS;
using Gaming.Predictor.Interfaces.Connection;
using Gaming.Predictor.Interfaces.Session;
using Gaming.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Gaming.Predictor.Contracts.Sharing;
using Options = Gaming.Predictor.Contracts.Feeds.Options;

namespace Gaming.Predictor.Blanket.Sharing
{
    public class ImageGeneration : Common.BaseBlanket
    {
        private readonly DataAccess.Feeds.Gameplay _DBContext;
        private readonly Feeds.Gameplay _Feeds;
        private readonly Int32 _TourId;
        private readonly IHostingEnvironment _Env;

        public ImageGeneration(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset, IHostingEnvironment env): base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Feeds.Gameplay(postgre);
            _Feeds = new Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
            _Env = env;
        }

        #region " Constant properties "

        private string _physicalPath { get { return _Env.ContentRootPath + "//"; } }
        private string _ConfigFile { get { return @"ImageSharing/Config/Config.txt"; } }
        private string _BaseImagePath { get { return @"ImageSharing/Images/Template/bg.jpg"; } }
        private string _TeamImagePath { get { return @"ImageSharing/Images/Teams/#img#.png"; } }
        private string _TeamDefaultImagePath { get { return @"ImageSharing/Images/Teams/0.png"; } }
        private string _RightImagePath { get { return @"ImageSharing/Images/Template/2.png"; } }
        private string _WrongImagePath { get { return @"ImageSharing/Images/Template/1.png"; } }
        private string _PredictImagePath { get { return @"ImageSharing/Images/Template/#.png"; } }
        private string _FontFile { get { return @"ImageSharing/Fonts/"; } }

        #endregion


        public async Task<HTTPResponse> GenerateImage(String Guid, Int32 MatchID, Int32 GameDayID)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject responseObject = new ResponseObject();
            UserPredictionResult predictionResults = new UserPredictionResult();
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<MatchQuestions> mQuestions = new List<MatchQuestions>();
            Fixtures fixture = new Fixtures();
            Int32 OptType = 1;
            String lang = "en";
            Stream imageStream = null;
            String imageExtension = "jpg";
            bool success = false;
            Int32 retVal = -1;

            if (_Cookies._HasGameCookies)
            {
                //Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                //if (_Cookies._HasUserCookies)
                //{
                //    Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                //    try
                //    {
                //        httpResponse = await _Feeds.GetFixtures(lang);
                //        if (httpResponse.Data != null)
                //            mFixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                //        fixture = mFixtures.Where(c => c.MatchId == MatchID).FirstOrDefault();
                //        responseObject = _DBContext.GetPredictions(OptType, _TourId, UserId, TeamId, MatchID, GameDayID, ref httpMeta);
                //        predictionResults = GenericFunctions.Deserialize<UserPredictionResult>(GenericFunctions.Serialize(responseObject.Value));

                //        responseObject = _DBContext.GetQuestions(OptType, _TourId, MatchID, ref httpMeta);
                //        mQuestions = GenericFunctions.Deserialize<List<MatchQuestions>>(GenericFunctions.Serialize(responseObject.Value));

                //        imageStream = ImageProcess(Guid, MatchID, GameDayID, predictionResults, fixture, mQuestions);
                //        if (imageStream != null)
                //        {
                //            success = await _AWS.ReplaceImageOnS3(imageStream, Guid, MatchID.ToString(), GameDayID.ToString(), imageExtension);
                //        }                      
                //        if (success)
                //        {
                //            retVal = 1;                            
                //        }
                //        GenericFunctions.AssetMeta(retVal, ref httpMeta);
                //        responseObject.Value = retVal;
                //        responseObject.FeedTime = GenericFunctions.GetFeedTime();

                //    }
                //    catch (Exception ex)
                //    {
                //        GenericFunctions.AssetMeta(-1, ref httpMeta, "error");
                //        responseObject.Value = retVal;
                //        responseObject.FeedTime = GenericFunctions.GetFeedTime();
                //        HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Sharing.ImageGeneration.GenerateImage", ex.Message);
                //        _AWS.Log(httpLog);
                //    }
                //}
                //else
                //{
                //    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
                //}
            }

            httpResponse.Data = responseObject;
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        private Stream ImageProcess(String Guid, Int32 MatchID, Int32 GameDayID, UserPredictionResult userPredictions, Fixtures fixture, List<MatchQuestions> mQuestions)
        {
            Stream imageStream = null;
            Config config = new Config();

            try
            {
                String fileName = Guid + "_" + MatchID + "_" + GameDayID;
                config = GetConfig();

                String baseImagePath = _physicalPath + _BaseImagePath;
                Bitmap baseImage = (Bitmap)System.Drawing.Image.FromFile(baseImagePath);
                int baseImageWidth = baseImage.Width;

                PredictionProcess(config, userPredictions, mQuestions, ref baseImage);
                MatchDetailsProcess(config, fixture, ref baseImage);
                MatchPointsProcess(config, userPredictions, ref baseImage);


                // QUALITY CONTROL  

                System.Drawing.Imaging.ImageCodecInfo jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);

                // Create an Encoder object based on the GUID  
                // for the Quality parameter category.  
                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.  
                // An EncoderParameters object has an array of EncoderParameter  
                // objects. In this case, there is only one  
                // EncoderParameter object in the array.  
                System.Drawing.Imaging.EncoderParameters myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                System.Drawing.Imaging.EncoderParameter myEncoderParameter = null;


                myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 85L);

                myEncoderParameters.Param[0] = myEncoderParameter;
                // END QUALITY CONTROL   

                MemoryStream ms = new MemoryStream();
                //baseImage.Save(ms, imageFormat);
                baseImage.Save(ms, jpgEncoder, myEncoderParameters);
                imageStream = ms;

                baseImage.Dispose();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return imageStream;
        }

        private void PredictionProcess(Config config, UserPredictionResult userPredictions, List<MatchQuestions> mQuestions, ref Bitmap baseImage)
        {
            int i = 1;
            foreach(QuestionDetails mUserPrediction in userPredictions.QuestionDetails)
            {

                MatchQuestions mQuestion = mQuestions.Where(c => c.QuestionId == mUserPrediction.QuestionId).FirstOrDefault();
                List<Options> CorrectOptions = mQuestion.Options.Where(c => c.IsCorrect == 1).ToList();

                bool isCorrect = CorrectOptions.Any(c => c.OptionId == mUserPrediction.OptionId);
              
                    String concernedImagePath = "";
                    Coordinate coordinate = new Coordinate();
                    switch(i)
                    {
                        case 1:
                            coordinate = config.coordinates.Where(c => c.entity.ToLower() == "predictionone").FirstOrDefault();
                            break;
                        case 2:
                            coordinate = config.coordinates.Where(c => c.entity.ToLower() == "predictiontwo").FirstOrDefault();
                            break;
                        case 3:
                            coordinate = config.coordinates.Where(c => c.entity.ToLower() == "predictionthree").FirstOrDefault();
                            break;
                        case 4:
                            coordinate = config.coordinates.Where(c => c.entity.ToLower() == "predictionfour").FirstOrDefault();
                            break;
                        case 5:
                            coordinate = config.coordinates.Where(c => c.entity.ToLower() == "predictionfive").FirstOrDefault();
                            break;
                        case 6:
                            coordinate = config.coordinates.Where(c => c.entity.ToLower() == "predictionsix").FirstOrDefault();
                            break;                        
                    }

                using (Graphics g = Graphics.FromImage(baseImage))
                {
                    if (isCorrect)
                        concernedImagePath = _physicalPath + _RightImagePath;
                    else
                        concernedImagePath = _physicalPath + _WrongImagePath;

                    Bitmap concernedImage = (Bitmap)System.Drawing.Image.FromFile(concernedImagePath);

                    g.DrawImage(concernedImage, coordinate.xPos, coordinate.yPos, coordinate.width, coordinate.height);
                }
                i++;
            }
        }

        private void MatchDetailsProcess(Config config, Fixtures fixture, ref Bitmap baseImage)
        {

            String MatchName = "IPL - " + fixture.MatchdayName + ",";
            String MatchVenue = "";
            try
            {
                if(fixture.Venue.IndexOf(",") > -1)
                {
                    Int32 pos = fixture.Venue.LastIndexOf(",") + 1;
                    MatchVenue = fixture.Venue.Substring(pos, fixture.Venue.Length - pos).Trim();
                }
            }
            catch (Exception ex) { }

                String concernedImagePath = "";
                Coordinate coordinate = new Coordinate();
             

                using (Graphics g = Graphics.FromImage(baseImage))
                {

                coordinate = config.coordinates.Where(c => c.entity.ToLower() == "matchname").FirstOrDefault();
                int rectangleWidth = coordinate.width;
                int rectangleHeight = coordinate.height;
                int rectangleXPos = coordinate.xPos;
                int rectangleYPos = coordinate.yPos;

                Rectangle rect = new Rectangle(rectangleXPos, rectangleYPos, rectangleWidth, rectangleHeight);
                g.DrawRectangle(Pens.Transparent, Rectangle.Round(rect));

                //TITLE
                FontDetail matchNameFontProperty = config.font_details.Where(o => o.entity == "matchname").FirstOrDefault();
                Font objFont;
                SolidBrush matchNameBrush;
                FontSetter(matchNameFontProperty, FontStyle.Regular, out objFont, out matchNameBrush);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

               
                SizeF matchNameF = g.MeasureString(MatchName, objFont);
                Int32 fontWidth = Convert.ToInt32(matchNameF.Width);
                float difference = rectangleWidth - fontWidth;
                float xPosForMatchName = difference / 2;

                g.DrawString(MatchName, objFont, matchNameBrush, rectangleXPos + xPosForMatchName, rectangleYPos);


                #region " Match Venue "

                 coordinate = config.coordinates.Where(c => c.entity.ToLower() == "matchvenue").FirstOrDefault();
                 rectangleWidth = coordinate.width;
                 rectangleHeight = coordinate.height;
                 rectangleXPos = coordinate.xPos;
                 rectangleYPos = coordinate.yPos;

                 rect = new Rectangle(rectangleXPos, rectangleYPos, rectangleWidth, rectangleHeight);
                g.DrawRectangle(Pens.Transparent, Rectangle.Round(rect));

                //TITLE
                 matchNameFontProperty = config.font_details.Where(o => o.entity == "matchvenue").FirstOrDefault();
               
                SolidBrush matchVenueBrush;
                FontSetter(matchNameFontProperty, FontStyle.Regular, out objFont, out matchVenueBrush);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


                SizeF matchVenueF = g.MeasureString(MatchVenue, objFont);
                 fontWidth = Convert.ToInt32(matchVenueF.Width);
                 difference = rectangleWidth - fontWidth;
                float xPosForMatchVenue = difference / 2;

                g.DrawString(MatchVenue, objFont, matchVenueBrush, rectangleXPos + xPosForMatchVenue, rectangleYPos);

                #endregion

                #region " Teams Images "

                // TEAM A

                coordinate = config.coordinates.Where(c => c.entity.ToLower() == "teama").FirstOrDefault();

                concernedImagePath = _physicalPath + ReplaceImageName(_TeamImagePath, fixture.TeamA.ToString());
                if (!File.Exists(concernedImagePath))
                    concernedImagePath = _physicalPath + _TeamDefaultImagePath;

                Bitmap concernedImage = (Bitmap)System.Drawing.Image.FromFile(concernedImagePath);
                float imageWidth =  coordinate.width;
                float imageHeight =  coordinate.width;
                float imageXPos =  coordinate.xPos;
                float imageYPos =  coordinate.yPos;

                g.DrawImage(concernedImage, imageXPos, imageYPos, imageWidth, imageHeight);


                // TEAM B
                coordinate = config.coordinates.Where(c => c.entity.ToLower() == "teamb").FirstOrDefault();

                concernedImagePath = _physicalPath + ReplaceImageName(_TeamImagePath, fixture.TeamB.ToString());
                if (!File.Exists(concernedImagePath))
                    concernedImagePath = _physicalPath + _TeamDefaultImagePath;

                 concernedImage = (Bitmap)System.Drawing.Image.FromFile(concernedImagePath);
                 imageWidth = coordinate.width;
                 imageHeight = coordinate.width;
                 imageXPos = coordinate.xPos;
                 imageYPos = coordinate.yPos;

                g.DrawImage(concernedImage, imageXPos, imageYPos, imageWidth, imageHeight);


                #endregion

                // concernedImagePath = _physicalPath + _WrongImagePath;

                // Bitmap concernedImage = (Bitmap)System.Drawing.Image.FromFile(concernedImagePath);

                //g.DrawImage(concernedImage, coordinate.xPos, coordinate.yPos, coordinate.width, coordinate.height);
            }

        }

        private void MatchPointsProcess(Config config, UserPredictionResult userPredictions, ref Bitmap baseImage)
        {
            String concernedImagePath = "";
            Coordinate coordinate = new Coordinate();
            Int32 PointsXDiffenrece = config.pointsXDifference;
            Int32 PointsSubTitleXDiffenrece = config.pointsSubTitleXDifference;


            using (Graphics g = Graphics.FromImage(baseImage))
            {
                String userPoints = userPredictions.PointRank.FirstOrDefault().Points.ToString();
                String pointsSubTitle = "  pts";

                coordinate = config.coordinates.Where(c => c.entity.ToLower() == "pointstitle").FirstOrDefault();
                int rectangleWidth = coordinate.width;
                int rectangleHeight = coordinate.height;
                int rectangleXPos = coordinate.xPos;
                int rectangleYPos = coordinate.yPos;

                Rectangle rect = new Rectangle(rectangleXPos, rectangleYPos, rectangleWidth, rectangleHeight);
                g.DrawRectangle(Pens.Transparent, Rectangle.Round(rect));

                //TITLE
                FontDetail pointsTitleFontProperty = config.font_details.Where(o => o.entity == "pointstitle").FirstOrDefault();
                Font objFont;
                SolidBrush pointsTitleBrush;
                FontSetter(pointsTitleFontProperty, FontStyle.Regular, out objFont, out pointsTitleBrush);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;

                SizeF pointsTitleF = g.MeasureString(userPoints, objFont);
                Int32 fontWidth = Convert.ToInt32(pointsTitleF.Width);
                float difference = rectangleWidth - fontWidth;
                float xPosForpointsTitle = difference / 2;

                // Points SubTitle
                FontDetail pointsSubTitleFontProperty = config.font_details.Where(o => o.entity == "pointssubtitle").FirstOrDefault();
                Font pointsSubTitleObjFont;
                SolidBrush pointsSubTitleBrush;
                FontSetter(pointsSubTitleFontProperty, FontStyle.Regular, out pointsSubTitleObjFont, out pointsSubTitleBrush);
                SizeF pointsSubTitleF = g.MeasureString(pointsSubTitle, pointsSubTitleObjFont);
                Int32 pointsSubTitleFontWidth = Convert.ToInt32(pointsSubTitleF.Width);
                float pointsSubTitleDifference = rectangleWidth - pointsSubTitleFontWidth;
                float pointsSubTitleXPosForpointsTitle = difference / 2;

                g.DrawString(userPoints, objFont, pointsTitleBrush, (rectangleXPos + xPosForpointsTitle) - PointsXDiffenrece, rectangleYPos);


                coordinate = config.coordinates.Where(c => c.entity.ToLower() == "pointssubtitle").FirstOrDefault();
                rectangleWidth = coordinate.width;
                rectangleHeight = coordinate.height;
                rectangleXPos = coordinate.xPos;
                rectangleYPos = coordinate.yPos;

                if (userPoints.Length == 1)
                    PointsXDiffenrece = 29;
                else if(userPoints.Length == 2)
                    PointsXDiffenrece = 88;
                else if(userPoints.Length == 3)
                    PointsXDiffenrece = 152;
                else if(userPoints.Length == 4)
                    PointsXDiffenrece = 212;

                g.DrawString(pointsSubTitle, pointsSubTitleObjFont, pointsSubTitleBrush, (rectangleXPos + xPosForpointsTitle) + PointsXDiffenrece, rectangleYPos);





            }

        }

        private string ReplaceImageName(string sourceText, string newName)
        {
            return sourceText.Replace("#img#", newName);
        }

        private void FontSetter(FontDetail fontProperty, FontStyle style, out Font font, out SolidBrush brush)
        {
            //Font Family
            //System.Drawing.FontFamily FontFamily = new System.Drawing.FontFamily(fontProperty.name);
            //System.Drawing.Font Font = new System.Drawing.Font(FontFamily, Int32.Parse(fontProperty.size), style, GraphicsUnit.Pixel);

            System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();

            String fontPath = _physicalPath + _FontFile + fontProperty.name + ".ttf";
            // String fontPath = _FontFile + fontProperty.name + ".ttf";

            pfc.AddFontFile(fontPath);

            System.Drawing.Font Font = new System.Drawing.Font(pfc.Families[0], Int32.Parse(fontProperty.size), style, GraphicsUnit.Pixel);

            //Font color
            //Color Color = System.Drawing.ColorTranslator.FromHtml(fontProperty.color);
            Color Color = new Color();
            Color = Color.FromArgb(fontProperty.Red, fontProperty.Green, fontProperty.Blue);

            SolidBrush Brush = new SolidBrush(Color);

            font = Font;
            brush = Brush;
        }

        private Config GetConfig()
        {
            String mData = String.Empty;
            Config config = new Config();

            String physicalPath = _physicalPath + _ConfigFile;

            try
            {
                if (File.Exists(physicalPath))
                {
                    mData = File.ReadAllText(physicalPath);
                }
                else
                    throw new Exception("Config file does not exist.");
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Sharing.ImageShare.GetConfig: " + ex.Message);
            }

            if (mData != String.Empty)
                config = GenericFunctions.Deserialize<Config>(mData);

            return config;
        }

        private System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
