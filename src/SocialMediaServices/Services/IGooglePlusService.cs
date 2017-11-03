using System.Collections.Generic;
using System.Threading.Tasks;
using SocialMediaServices.Models;

namespace SocialMediaServices.Services
{
    /// <summary>
    /// Utalizes the Google+ API
    /// <para/>
    /// https://developers.google.com/+/web/api/rest/
    /// </summary>
    public interface IGooglePlusService
    {
        /// <summary>
        /// Retrieves comments for the first Google+ post
        /// </summary>
        /// <param name="activityId">Post Id</param>
        /// <returns>A List of comments</returns>
        Task<IList<Comment>> GetCommentsAsync(string activityId);

        /// <summary>
        /// Retrieves a person Id from Google+ given the diaply name
        /// </summary>
        /// <param name="name">User display name</param>
        /// <returns>User Id</returns>
        Task<string> GetPersonIdAsync(string name);

        /// <summary>
        /// Retrieves info for the first Google+ post by the user 
        /// with the given video as an attachment.
        /// </summary>
        /// <param name="personId">User Id</param>
        /// <param name="videoId">Video Id</param>
        /// <returns>Post(Activity) statistics</returns>
        Task<Activity> GetActivityWithVideoAsync(string personId, string videoId);
    }
}