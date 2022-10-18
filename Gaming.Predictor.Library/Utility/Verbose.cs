using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Library.Utility
{
    public class Verbose
    {
        public static String Message(Int64 input)
        {
            String message = "";

            switch (input)
            {
                case -40:
                    message = "Default object from Blanket layer";
                    break;
                case -50:
                    message = "Default object from DataAccess layer";
                    break;
                case -60:
                    message = "Default object from DataInitializer layer";
                    break;
                case -70:
                    message = "Not Authorized";
                    break;
                case -90:
                    message = "Invalid Request. SocialId empty.";
                    break;
                case -100:
                    message = "User not recognized";
                    break;
                //case -110:
                //    message = "Image Stream is empty.";
                //    break;
                //case 4:
                //    message = "TeamName is empty";
                //    break;
                //case -555:
                //    message = "Profane content";
                //    break;
                case -2:
                    message = "Atlist 1 question must be answer.";
                    break;
                case -51:
                    message = "Booster Already Applied.";
                    break;
                case -52:
                    message = "Invalid booster request";
                    break;

            }

            return message;
        }
    }
}
