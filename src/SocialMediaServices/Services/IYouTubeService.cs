using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SocialMediaServices.Models;

namespace SocialMediaServices.Services
{
    /// <summary>
    /// Utalizes the YouTube V3 Data API
    /// <para />
    /// https://developers.google.com/youtube/v3/docs/
    /// </summary>
    public interface IYouTubeService
    {
        /// <summary>
        /// Queries the YouTube v3 Data API for all comments for the given video. 
        /// Including replies to comments.
        /// </summary>
        /// <param name="videoId">The YouTube video Id to get comments for</param>
        /// <returns>A list of comment id's for the given video</returns>
        Task<IList<Comment>> GetCommentsAsync(string videoId);

        /// <summary>
        /// Queries the YouTube v3 Data API for all comments for the given video. 
        /// Including replies to comments.
        /// </summary>
        /// <param name="videoId">The YouTube video Id to get comments for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>A list of comment id's for the given video</returns>
        /// <exception cref="OperationCanceledException" />
        Task<IList<Comment>> GetCommentsAsync(string videoId, CancellationToken ct);

        /// <summary>
        /// Queries the YouTube v3 Data API for all comments for the given video. 
        /// Including replies to comments.
        /// </summary>
        /// <param name="videoId">The YouTube video Id to get comments for</param>
        /// <param name="progress">Progress indicator</param>
        /// <returns>A list of comment id's for the given video</returns>
        Task<IList<Comment>> GetCommentsAsync(string videoId, IProgress<int> progress);

        /// <summary>
        /// Queries the YouTube v3 Data API for all comments for the given video. 
        /// Including replies to comments.
        /// </summary>
        /// <param name="videoId">The YouTube video Id to get comments for</param>
        /// <param name="progress">Progress indicator</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>A list of comment id's for the given video</returns>
        /// <exception cref="OperationCanceledException" />
        Task<IList<Comment>> GetCommentsAsync(string videoId, IProgress<int> progress, CancellationToken ct);

        /// <summary>
        /// Retrieves all videos in a playlist
        /// </summary>
        /// <param name="playlistId">Playlist Id</param>
        /// <returns>A list of video Ids</returns>
        Task<IList<Video>> GetPlaylistVideosAsync(string playlistId);

        /// <summary>
        /// Retrieves the Id of the playlist
        /// </summary>
        /// <param name="channelId">Id of YouTube channel</param>
        /// <param name="playlistName">Name of YouTube playlist</param>
        /// <returns>A list of video Ids</returns>
        Task<string> GetPlaylistIdAsync(string channelId, string playlistName);

        /// <summary>
        /// Retrieves the channel Id
        /// </summary>
        /// <param name="channelName">Name of YouTube channel</param>
        /// <returns>A list of video Ids</returns>
        Task<string> GetChannelIdAsync(string channelName);

        /// <summary>
        /// Returns information about a YouTube video.
        /// </summary>
        /// <param name="id">The YouTube video Id</param>
        /// <returns><see cref="Video"/></returns>
        Task<Video> GetVideoAsync(string id);

        /// <summary>
        /// Given a YouTube video URL will parse out the video Id query parameter.
        /// </summary>
        /// <param name="uri">The YouTube video Url</param>
        /// <returns>The video Id if uri is valid, else null</returns>
        string ParseVideoId(Uri uri);
    }
}