using System.Collections.Generic;
using System.Threading.Tasks;
using SocialMediaServices.Models;

namespace SocialMediaServices.Services
{
    /// <summary>
    /// Utalizes the Facebook Graph APIP
    /// <para/>
    /// https://developers.facebook.com/docs/graph-api
    /// </summary>
    public interface IFaceBookService
    {

        /// <summary>
        /// Gets all comments for post
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <returns>Comments for post</returns>
        Task<IList<Comment>> GetCommentsAsync(string postId);

        /// <summary>
        /// Gets All pages matching name
        /// </summary>
        /// <param name="name">Page name</param>
        /// <returns>Matching pages</returns>
        Task<IList<Page>> GetPagesAsync(string name);

        /// <summary>
        /// Gets all posts for page
        /// </summary>
        /// <param name="pageId">Page Id</param>
        /// <returns>Posts for page</returns>
        Task<IList<Post>> GetPostsAsync(string pageId);
    }
}