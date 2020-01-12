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
namespace YAF.Pages
{
    #region Using

    using System;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The my topics page.
    /// </summary>
    public partial class mytopics : ForumPage
    {
        /// <summary>
        /// Indicates if the Active Tab was loaded
        /// </summary>
        private bool activeLoaded;

        /// <summary>
        /// Indicates if the Unanswered Tab was loaded
        /// </summary>
        private bool unansweredLoaded;

        /// <summary>
        /// Indicates if the Unread Tab was loaded
        /// </summary>
        private bool unreadLoaded;

        /// <summary>
        /// Indicates if the My Topics Tab was loaded
        /// </summary>
        private bool mytopicsLoaded;

        /// <summary>
        /// Indicates if the Favorite Tab was loaded
        /// </summary>
        private bool favoriteLoaded;

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the mytopics class.
        /// </summary>
        public mytopics()
            : base("MYTOPICS")
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the current tab.
        /// </summary>
        /// <value>
        /// The current tab.
        /// </value>
        private TopicListMode CurrentTab
        {
            get
            {
                if (this.ViewState["CurrentTab"] != null)
                {
                    return (TopicListMode)this.ViewState["CurrentTab"];
                }

                return TopicListMode.Active;
            }

            set => this.ViewState["CurrentTab"] = value;
        }

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicStarterPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("TOPIC_STARTER")}&nbsp;...",
                    ".topic-starter-popover",
                    "hover"));

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "TopicLinkPopoverJs",
                JavaScriptBlocks.TopicLinkPopoverJs(
                    $"{this.GetText("LASTPOST")}&nbsp;{this.GetText("SEARCH", "BY")} ...",
                    ".topic-link-popover",
                    "focus hover"));

            this.PageContext.PageElements.RegisterJsBlock(
                "TopicsTabsJs",
                JavaScriptBlocks.BootstrapTabsLoadJs(
                    this.TopicsTabs.ClientID,
                    this.hidLastTab.ClientID,
                    this.Page.ClientScript.GetPostBackEventReference(this.ChangeTab, string.Empty)));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The Page_ Load Event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                this.RefreshTab();

                return;
            }

            this.UserTopicsTabTitle.Visible = !this.PageContext.IsGuest;
            this.UserTopicsTabContent.Visible = !this.PageContext.IsGuest;

            this.UnreadTopicsTabTitle.Visible = !this.PageContext.IsGuest &&
                                                this.Get<BoardSettings>().UseReadTrackingByDatabase;
            this.UnreadTopicsTabContent.Visible = !this.PageContext.IsGuest &&
                                                  this.Get<BoardSettings>().UseReadTrackingByDatabase;

            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(
                this.PageContext.IsGuest ? this.GetText("GUESTTITLE") : this.GetText("MEMBERTITLE"), string.Empty);

            this.ForumJumpHolder.Visible = this.Get<BoardSettings>().ShowForumJump &&
                                           this.PageContext.Settings.LockedForum == 0;
        }

        #endregion

        /// <summary>
        /// Load the Selected Tab Content
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ChangeTabClick(object sender, EventArgs e)
        {
            switch (this.hidLastTab.Value)
            {
                case "UnansweredTopicsTab":
                    this.CurrentTab = TopicListMode.Unanswered;
                    break;
                case "UnreadTopicsTab":
                    this.CurrentTab = TopicListMode.Unread;
                    break;
                case "MyTopicsTab":
                    this.CurrentTab = TopicListMode.User;
                    break;
                case "FavoriteTopicsTab":
                    this.CurrentTab = TopicListMode.Favorite;
                    break;
                default:
                    this.CurrentTab = TopicListMode.Active;
                    break;
            }

            this.RefreshTab();
        }

        /// <summary>
        /// Refreshes the tab.
        /// </summary>
        private void RefreshTab()
        {
            switch (this.CurrentTab)
            {
                case TopicListMode.Unanswered:

                    if (!this.unansweredLoaded)
                    {
                        this.UnansweredTopics.BindData();

                        this.ActiveTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.unansweredLoaded = true;
                    }

                    break;
                case TopicListMode.Unread:

                    if (!this.unreadLoaded)
                    {
                        this.UnreadTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.unreadLoaded = true;
                    }

                    break;
                case TopicListMode.User:

                    if (!this.mytopicsLoaded)
                    {
                        this.MyTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.FavoriteTopics.DataBind();
                        }

                        this.mytopicsLoaded = true;
                    }
                    else
                    {
                        this.MyTopics.DataBind();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.FavoriteTopics.DataBind();
                        }
                    }

                    break;
                case TopicListMode.Favorite:

                    if (!this.favoriteLoaded)
                    {
                        this.FavoriteTopics.BindData();

                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                        }

                        this.favoriteLoaded = true;
                    }

                    break;
                case TopicListMode.Active:

                    if (!this.activeLoaded)
                    {
                        this.ActiveTopics.DataBind();
                        this.UnansweredTopics.DataBind();
                        if (this.UnreadTopicsTabTitle.Visible)
                        {
                            this.UnreadTopics.DataBind();
                        }

                        if (this.UserTopicsTabTitle.Visible)
                        {
                            this.MyTopics.DataBind();
                            this.FavoriteTopics.DataBind();
                        }

                        this.activeLoaded = true;
                    }

                    break;
            }
        }
    }
}