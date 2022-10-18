using Gaming.Predictor.Contracts.Common;
using Gaming.Predictor.Library.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Predictor.Blanket
{
    public class ResponseResult
    {
        #region " Version 1 - Constructing then assigning ResponseObject "

        /// <summary>
        /// Generic Return object in Blanket
        /// </summary>
        /// <param name="data">Returning object</param>
        /// <returns></returns>
        protected HTTPResponse OkResponse(Object data)
        {
            ResponseObject res = new ResponseObject();
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int64 value = 1;

            if (data is int)
                value = Convert.ToInt64(data);

            String message = Verbose.Message(value);
            GenericFunctions.AssetMeta(value, ref httpMeta, message);

            res.Value = data;
            res.FeedTime = GenericFunctions.GetFeedTime();

            httpResponse.Data = res;
            httpResponse.Meta = httpMeta;

            return httpResponse;
        }

        /// <summary>
        /// Return object for DataAccess Wrapper functions in Blanket
        /// </summary>
        /// <param name="data">Returning object</param>
        /// <param name="meta">HTTPMeta object from DataAccess layer</param>
        /// <returns></returns>
        protected HTTPResponse OkResponse(Object data, HTTPMeta meta)
        {
            ResponseObject res = new ResponseObject();
            HTTPResponse httpResponse = new HTTPResponse();

            if (meta != null && String.IsNullOrEmpty(meta.Message))
            {
                Int64 value = 1;

                if (data is int)
                    value = Convert.ToInt64(data);

                String message = Verbose.Message(value);
                GenericFunctions.AssetMeta(value, ref meta, message);
            }

            res.Value = data;
            res.FeedTime = GenericFunctions.GetFeedTime();

            httpResponse.Data = res;
            httpResponse.Meta = meta;

            return httpResponse;
        }

        #endregion

        #region " Version 2 - Directly assigning ResponseObject "

        /// <summary>
        /// Generic Return object in Blanket
        /// </summary>
        /// <param name="data">Returning object</param>
        /// <returns></returns>
        protected HTTPResponse OkResponse(ResponseObject data)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            GenericFunctions.AssetMeta(1, ref httpMeta);

            httpResponse.Data = data;
            httpResponse.Meta = httpMeta;

            return httpResponse;
        }

        /// <summary>
        /// Return object for DataAccess Wrapper functions in Blanket
        /// </summary>
        /// <param name="data">Returning object</param>
        /// <param name="meta">HTTPMeta object from DataAccess layer</param>
        /// <returns></returns>
        protected HTTPResponse OkResponse(ResponseObject data, HTTPMeta meta)
        {
            HTTPResponse httpResponse = new HTTPResponse();

            if (meta != null && String.IsNullOrEmpty(meta.Message))
                GenericFunctions.AssetMeta(1, ref meta);

            httpResponse.Data = data;
            httpResponse.Meta = meta;

            return httpResponse;
        }

        #endregion

        #region " Catch Response "

        /// <summary>
        /// Error Return object. To be used in Catch block in Blanket.
        /// </summary>
        /// <param name="message">The text of Message property of HTTPMeta object</param>
        /// <returns></returns>
        protected HTTPResponse CatchResponse(String message)
        {
            ResponseObject res = new ResponseObject();
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            GenericFunctions.AssetMeta(-40, ref httpMeta, message);

            res.Value = null;
            res.FeedTime = GenericFunctions.GetFeedTime();

            httpResponse.Data = res;
            httpResponse.Meta = httpMeta;

            return httpResponse;
        }

        #endregion
    }
}
