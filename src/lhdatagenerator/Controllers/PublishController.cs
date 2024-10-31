// -----------------------------------------------------------------------
// <copyright file="PublishController.cs" company="Sitecore Corporation">
// Copyright (c) Sitecore Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sitecore.Performance.LhDataGenerator.Controllers
{
    using System;
    using System.Threading;
    using System.Web.Mvc;

    using Sitecore.Data;
    using Sitecore.Globalization;
    using Sitecore.Jobs;
    using Sitecore.Mvc.Extensions;
    using Sitecore.Publishing;

    /// <summary>
    /// Class ContentItems.
    /// </summary>
    public class PublishController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishController"/> class.
        /// </summary>
        public PublishController()
        {
        }

        /// <summary>
        /// Waits for complete.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>
        /// Bool for success.
        /// </returns>
        public bool WaitForComplete(Handle handle, int interval)
        {
            var state = PublishManager.GetStatus(handle).State;
            while (state != JobState.Finished)
            {
                Thread.Sleep(interval);
                state = PublishManager.GetStatus(handle).State;
            }

            return true;
        }

        /// <summary>
        /// Initiates Smart Publishing of the entire site.
        /// </summary>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        /// <param name="interval">The interval.</param>
        /// <param name="targetDb">The target database.</param>
        /// <returns>
        /// Handle of the publishing job, if wait = false, otherwise PublishComplete.
        /// </returns>
        public ActionResult PublishSmart(bool wait = false, int interval = 1000, string targetDb = "web")
        {
            var result = new RequestResult();
            result.LogInfo("PublishSmart: Start");

            var src = General.GetMasterDb();
            Database[] target = new Database[1];
            target[0] = General.GetDb(targetDb);
            Language[] languages = new Language[1];
            languages[0] = Language.Current;

            var handle = Sitecore.Publishing.PublishManager.PublishSmart(src, target, languages);
            if (wait)
            {
                result.Success = this.WaitForComplete(handle, interval);
                result.LogInfo("PublishSmart: Wait");
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result.Success = true;
                result.LogInfo($"Handle: {handle.ToString()}");
                result.LogInfo("PublishSmart: End");
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Publishes a specific item and its descendants.
        /// </summary>
        /// <param name="itemPath">Publish path</param>
        /// <param name="deep">if set to <c>true</c> [children].</param>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        /// <param name="interval">The interval.</param>
        /// <param name="targetDb">The target database.</param>
        /// <returns>
        /// Handle of the publishing job, if wait = false, otherwise PublishComplete.
        /// </returns>
        public ActionResult PublishItem(string itemPath, bool deep, bool wait = false, int interval = 1000, string targetDb = "web")
        {
            var result = new RequestResult();
            result.LogInfo("PublishItem: Start");

            // var homePath = "/sitecore/content/Demo SXA Sites/LighthouseLifestyle/home";
            // var itemPath = $"{homePath}/Top_{pagePrefix}";
            var src = General.GetMasterDb();

            Database[] target = new Database[1];
            target[0] = General.GetDb(targetDb);
            Language[] languages = new Language[1];
            languages[0] = Language.Current;

            var item = src.GetItem(itemPath);
            var handle = Sitecore.Publishing.PublishManager.PublishItem(item, target, languages, deep, false);
            if (wait)
            {
                result.Success = this.WaitForComplete(handle, interval);
                result.LogInfo("PublishItem: Wait");
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result.Success = true;
                result.LogInfo($"Handle: {handle.ToString()}");
                result.LogInfo("PublishItem: End");
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Publishes a specific item and its descendants.
        /// </summary>
        /// <param name="itemPath">Publish path</param>
        /// <param name="children">if set to <c>true</c> [children].</param>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        /// <param name="smart">if set to <c>true</c> [smart].</param>
        /// <param name="related">if set to <c>true</c> [related].</param>
        /// <param name="interval">The interval.</param>
        /// <param name="targetDb">The target database.</param>
        /// <returns>
        /// Handle of the publishing job, if wait = false, otherwise PublishComplete.
        /// </returns>
        public ActionResult PublishEdgeItem(string itemPath, bool children, bool wait = false, bool smart = false, bool related = false, int interval = 1000, string targetDb = "experienceedge")
        {
            var result = new RequestResult();
            result.LogInfo("PublishItem: Start");

            // var homePath = "/sitecore/content/Demo SXA Sites/LighthouseLifestyle/home";
            // var itemPath = $"{homePath}/Top_{pagePrefix}";
            var src = General.GetMasterDb();
            var publishingTargets = Sitecore.Publishing.PublishManager.GetPublishingTargets(src);

            Database[] target = new Database[1];
            target[0] = General.GetDb(targetDb);
            Language[] languages = new Language[1];
            languages[0] = Language.Current;

            var item = src.GetItem(itemPath);
            var handle = Sitecore.Publishing.PublishManager.PublishItem(item, target, languages, children, smart, related);
            if (wait)
            {
                result.Success = this.WaitForComplete(handle, interval);
                result.LogInfo("PublishItem: Wait");
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result.Success = true;
                result.LogInfo($"Handle: {handle.ToString()}");
                result.LogInfo("PublishItem: End");
                result.LogInfo($"Targets: {publishingTargets.ToString()}");
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Retrieves the status of a publish job.
        /// </summary>
        /// <param name="handleString">The handle string.</param>
        /// <returns>
        /// The publish result.
        /// </returns>
        public ActionResult PublishResult(string handleString)
        {
            var result = new RequestResult();
            result.LogInfo("PublishResult: Start");

            Handle handle = Handle.Parse(handleString);

            string publishState;
            string publishResult;
            string processed;
            string messages = "";
            if (handle == null)
            {
                publishState = "null";
                publishResult = "null";
                processed = "null";
                messages = "null";
            }
            else
            {
                try
                {
                    var status = PublishManager.GetStatus(handle);
                    publishState = status.State.ToString();
                    publishResult = status.Failed.ToString();
                    processed = status.Processed.ToString();
                    foreach (string message in status.Messages)
                    {
                        messages.Append(message, ';');
                    }
                }
                catch (NullReferenceException)
                {
                    publishState = "Exception";
                    publishResult = handle.ToString();
                    processed = "null";
                    messages = "null";
                }
            }

            result.Success = true;
            result.LogInfo($"State: {publishState}");
            result.LogInfo($"Result: {publishResult}");
            result.LogInfo($"Processed: {processed}");
            result.LogInfo($"Messages: {messages}");
            result.LogInfo("PublishResult: End");
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
