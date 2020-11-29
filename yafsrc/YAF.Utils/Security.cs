/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Utils
{
    using System.Web;

    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    /// <summary>
    /// The security.
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// Function that verifies a string is an integer value or it redirects to invalid "info" page.
        /// Used as a security feature against invalid values submitted to the page.
        /// </summary>
        /// <param name="intValue">
        /// The string value to test
        /// </param>
        /// <returns>
        /// The converted integer value
        /// </returns>
        public static int StringToIntOrRedirect(string intValue)
        {
            if (!int.TryParse(intValue, out var value))
            {
                // it's an invalid request. Redirect to the info page on invalid requests.
                BuildLink.RedirectInfoPage(InfoMessage.Invalid);
            }

            return value;
        }

        /// <summary>
        /// This method validates request whether it comes from same server in case it's HTTP POST.
        /// </summary>
        /// <param name="request">
        /// Request to validate.
        /// </param>
        public static void CheckRequestValidity(HttpRequest request)
        {
            // ip with 
            // deny access if POST request comes from other server
            if (request.HttpMethod == "POST" && request.UrlReferrer != null && request.Url.Host.IsSet()
                && request.UrlReferrer.Host != request.Url.Host)
            {
                BuildLink.AccessDenied();
            }
        }
    }
}