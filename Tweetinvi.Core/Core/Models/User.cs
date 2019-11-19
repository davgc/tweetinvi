﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tweetinvi.Core.Controllers;
using Tweetinvi.Iterators;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Models.Entities;
using Tweetinvi.Parameters;

namespace Tweetinvi.Logic
{
    /// <summary>
    /// Tweetinvi User.
    /// </summary>
    public class User : IUser
    {
        private const string REGEX_PROFILE_IMAGE_SIZE = "_[^\\W_]+(?=(?:\\.[a-zA-Z0-9_]+$))";
        public ITwitterClient Client { get; set; }

        protected IUserDTO _userDTO;
        protected readonly ITimelineController _timelineController;
        private readonly ITwitterListController _twitterListController;

        public IUserDTO UserDTO
        {
            get { return _userDTO; }
            set { _userDTO = value; }
        }

        public IUserIdentifier UserIdentifier
        {
            get { return _userDTO; }
        }

        #region Public Attributes

        #region Twitter API Attributes

        // This region represents the information accessible from a Twitter API
        // when querying for a User

        public long? Id
        {
            get { return _userDTO?.Id; }
            set { throw new InvalidOperationException("Cannot set the Id of a user"); }
        }

        public string IdStr
        {
            get { return _userDTO?.IdStr; }
            set { throw new InvalidOperationException("Cannot set the Id of a user"); }
        }

        public string ScreenName
        {
            get { return _userDTO?.ScreenName; }
            set { throw new InvalidOperationException("Cannot set the ScreenName of a user"); }
        }

        public string Name
        {
            get { return _userDTO.Name; }
        }

        public string Description
        {
            get { return _userDTO.Description; }
        }

        public ITweetDTO Status
        {
            get { return _userDTO.Status; }
        }

        public DateTime CreatedAt
        {
            get { return _userDTO.CreatedAt; }
        }

        public string Location
        {
            get { return _userDTO.Location; }
        }

        public bool GeoEnabled
        {
            get { return _userDTO.GeoEnabled; }
        }

        public string Url
        {
            get { return _userDTO.Url; }
        }

        public int StatusesCount
        {
            get { return _userDTO.StatusesCount; }
        }

        public int FollowersCount
        {
            get { return _userDTO.FollowersCount; }
        }

        public int FriendsCount
        {
            get { return _userDTO.FriendsCount; }
        }

        public bool Following
        {
            get { return _userDTO.Following; }
        }

        public bool Protected
        {
            get { return _userDTO.Protected; }
        }

        public bool Verified
        {
            get { return _userDTO.Verified; }
        }

        public IUserEntities Entities
        {
            get { return _userDTO.Entities; }
        }

        public string ProfileImageUrl
        {
            get { return _userDTO.ProfileImageUrl; }
        }

        public string ProfileImageUrlFullSize
        {
            get
            {
                var profileImageURL = ProfileImageUrl;
                if (string.IsNullOrEmpty(profileImageURL))
                {
                    return null;
                }

                return Regex.Replace(profileImageURL, REGEX_PROFILE_IMAGE_SIZE, string.Empty);
            }
        }

        public string ProfileImageUrl400x400
        {
            get
            {
                var profileImageURL = ProfileImageUrl;
                if (string.IsNullOrEmpty(profileImageURL))
                {
                    return null;
                }

                return Regex.Replace(profileImageURL, REGEX_PROFILE_IMAGE_SIZE, "_400x400");
            }
        }

        public string ProfileImageUrlHttps
        {
            get { return _userDTO.ProfileImageUrlHttps; }
        }

        public bool FollowRequestSent
        {
            get { return _userDTO.FollowRequestSent; }
        }

        public bool DefaultProfile
        {
            get { return _userDTO.DefaultProfile; }
        }

        public bool DefaultProfileImage
        {
            get { return _userDTO.DefaultProfileImage; }
        }

        public int FavouritesCount
        {
            get { return _userDTO.FavoritesCount ?? 0; }
        }

        public int ListedCount
        {
            get { return _userDTO.ListedCount ?? 0; }
        }

        public string ProfileSidebarFillColor
        {
            get { return _userDTO.ProfileSidebarFillColor; }
        }

        public string ProfileSidebarBorderColor
        {
            get { return _userDTO.ProfileSidebarBorderColor; }
        }

        public bool ProfileBackgroundTile
        {
            get { return _userDTO.ProfileBackgroundTile; }
        }

        public string ProfileBackgroundColor
        {
            get { return _userDTO.ProfileBackgroundColor; }
        }

        public string ProfileBackgroundImageUrl
        {
            get { return _userDTO.ProfileBackgroundImageUrl; }
        }

        public string ProfileBackgroundImageUrlHttps
        {
            get { return _userDTO.ProfileBackgroundImageUrlHttps; }
        }

        public string ProfileBannerURL
        {
            get { return _userDTO.ProfileBannerURL; }
        }

        public string ProfileTextColor
        {
            get { return _userDTO.ProfileTextColor; }
        }

        public string ProfileLinkColor
        {
            get { return _userDTO.ProfileLinkColor; }
        }

        public bool ProfileUseBackgroundImage
        {
            get { return _userDTO.ProfileUseBackgroundImage; }
        }

        public bool IsTranslator
        {
            get { return _userDTO.IsTranslator; }
        }

        public bool ContributorsEnabled
        {
            get { return _userDTO.ContributorsEnabled; }
        }

        public int? UtcOffset
        {
            get { return _userDTO.UtcOffset; }
        }

        public string TimeZone
        {
            get { return _userDTO.TimeZone; }
        }

        public IEnumerable<string> WithheldInCountries
        {
            get { return _userDTO.WithheldInCountries; }
        }

        public string WithheldScope
        {
            get { return _userDTO.WithheldScope; }
        }

        [Obsolete("Twitter's documentation states that this property is deprecated")]
        public bool Notifications
        {
            get { return _userDTO.Notifications; }
        }

        #endregion

        #region Tweetinvi API Attributes

        public List<long> FriendIds { get; set; }
        public List<IUser> Friends { get; set; }
        public List<long> FollowerIds { get; set; }
        public List<IUser> Followers { get; set; }
        public List<IUser> Contributors { get; set; }
        public List<IUser> Contributees { get; set; }
        public List<ITweet> Timeline { get; set; }
        public List<ITweet> Retweets { get; set; }
        public List<ITweet> FriendsRetweets { get; set; }
        public List<ITweet> TweetsRetweetedByFollowers { get; set; }

        #endregion

        #endregion

        public User(
            IUserDTO userDTO,
            ITimelineController timelineController,
            ITwitterListController twitterListController)
        {
            _userDTO = userDTO;
            _timelineController = timelineController;
            _twitterListController = twitterListController;
        }

        // Friends
        public virtual ITwitterIterator<long> GetFriendIds()
        {
            return Client?.Users.GetFriendIds(new GetFriendIdsParameters(this));
        }

        public virtual IMultiLevelCursorIterator<long, IUser> GetFriends()
        {
            return Client?.Users.GetFriends(new GetFriendsParameters(this));
        }

        // Followers
        public virtual ITwitterIterator<long> GetFollowerIds()
        {
            return Client?.Users.GetFollowerIds(new GetFollowerIdsParameters(this));
        }

        public virtual IMultiLevelCursorIterator<long, IUser> GetFollowers()
        {
            return Client?.Users.GetFollowers(new GetFollowersParameters(this));
        }

        // Relationship
        public Task<IRelationshipDetails> GetRelationshipWith(IUserIdentifier user)
        {
            return Client.Users.GetRelationshipBetween(this, user);
        }

        public Task<IRelationshipDetails> GetRelationshipWith(long? userId)
        {
            return Client.Users.GetRelationshipBetween(this, userId);
        }

        public Task<IRelationshipDetails> GetRelationshipWith(string username)
        {
            return Client.Users.GetRelationshipBetween(this, username);
        }

        // Timeline
        public Task<IEnumerable<ITweet>> GetUserTimeline(int maximumNumberOfTweets = 40)
        {
            return _timelineController.GetUserTimeline(this, maximumNumberOfTweets);
        }

        public Task<IEnumerable<ITweet>> GetUserTimeline(IUserTimelineParameters timelineParameters)
        {
            return _timelineController.GetUserTimeline(this, timelineParameters);
        }

        // Favorites
        public virtual ITwitterIterator<ITweet, long?> GetFavoriteTweets()
        {
            return Client.Tweets.GetFavoriteTweets(this);
        }

        // Lists
        public Task<IEnumerable<ITwitterList>> GetSubscribedLists(int maximumNumberOfListsToRetrieve = TweetinviConsts.LIST_GET_USER_SUBSCRIPTIONS_COUNT)
        {
            return _twitterListController.GetUserSubscribedLists(this, maximumNumberOfListsToRetrieve);
        }

        public Task<IEnumerable<ITwitterList>> GetOwnedLists(int maximumNumberOfListsToRetrieve = TweetinviConsts.LIST_OWNED_COUNT)
        {
            return _twitterListController.GetUserOwnedLists(this, maximumNumberOfListsToRetrieve);
        }

        // Block User
        public virtual Task<bool> BlockUser()
        {
            return Client.Account.BlockUser(this);
        }

        public virtual Task<bool> UnBlockUser()
        {
            return Client.Account.UnBlockUser(this);
        }

        // Spam
        public virtual Task<bool> ReportUserForSpam()
        {
            return Client.Account.ReportUserForSpam(this);
        }

        // Stream Profile Image
        public Task<Stream> GetProfileImageStream()
        {
            return GetProfileImageStream(ImageSize.Normal);
        }

        public Task<Stream> GetProfileImageStream(ImageSize imageSize)
        {
            return Client.Users.GetProfileImageStream(new GetProfileImageParameters(this)
            {
                ImageSize = imageSize
            });
        }

        // Contributors
        public IEnumerable<IUser> GetContributors(bool createContributorList = false)
        {
            // string query = Resources.User_GetContributors;
            throw new NotImplementedException();
        }

        // Contributees
        public IEnumerable<IUser> GetContributees(bool createContributeeList = false)
        {
            // string query = Resources.User_GetContributees;
            throw new NotImplementedException();
        }

        public bool Equals(IUser other)
        {
            return Id == other.Id || ScreenName == other.ScreenName;
        }

        public override string ToString()
        {
            return _userDTO?.Name ?? "Undefined";
        }
    }
}