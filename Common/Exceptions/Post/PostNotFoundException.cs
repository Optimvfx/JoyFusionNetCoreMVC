using Common.Exceptions.General;

namespace Common.Exceptions.Post
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            NotFoundModel = "Post";
        }
    }
}
