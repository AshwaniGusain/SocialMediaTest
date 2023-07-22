using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia
{
    public class Program
    {
        static List<User> users = new List<User>();
        static User currentUser = null;

        static void Main()
        {
            Console.WriteLine("Welcome to the Social Network Console App!");

            while (true)
            {
                Console.WriteLine("\nAvailable commands: [signup], [login], [post], [follow], [reply], [upvote], [downvote], [shownewsfeed], [logout]");
                Console.Write("Enter command: ");
                string input = Console.ReadLine();
                string[] commandParts = input.Split(new[] { ' ' }, 2);

                switch (commandParts[0].ToLower())
                {
                    case "signup":
                        SignUpUser();
                        break;
                    case "login":
                        LoginUser();
                        break;
                    case "post":
                        CreatePost();
                        break;
                    case "follow":
                        FollowUser();
                        break;
                    case "reply":
                        ReplyToPost();
                        break;
                    case "upvote":
                        UpvotePost();
                        break;
                    case "downvote":
                        DownvotePost();
                        break;
                    case "shownewsfeed":
                        ShowNewsFeed();
                        break;
                    case "logout":
                        LogoutUser();
                        break;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
        }

        static void SignUpUser()
        {
            Console.Write("Enter your username: ");
            string username = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Please choose a different one.");
                return;
            }

            User newUser = new User(username, password);
            users.Add(newUser);
            Console.WriteLine("User registered successfully!");
        }

        static void LoginUser()
        {
            if (currentUser != null)
            {
                Console.WriteLine("You are already logged in. Please log out first to log in with another account.");
                return;
            }

            Console.Write("Enter your username: ");
            string username = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                currentUser = user;
                Console.WriteLine($"Logged in as {currentUser.Username}");
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

        static void CreatePost()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            Console.Write("Enter your post content: ");
            string content = Console.ReadLine();
            currentUser.CreatePost(content);
            Console.WriteLine("Post created successfully!");
        }

        static void FollowUser()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            Console.Write("Enter the username of the user you want to follow: ");
            string usernameToFollow = Console.ReadLine();

            User userToFollow = users.FirstOrDefault(u => u.Username == usernameToFollow);

            if (userToFollow == null)
            {
                Console.WriteLine("User not found. Please enter a valid username.");
                return;
            }

            if (userToFollow == currentUser)
            {
                Console.WriteLine("You cannot follow yourself.");
                return;
            }

            if (currentUser.IsFollowing(userToFollow))
            {
                Console.WriteLine("You are already following this user.");
                return;
            }

            currentUser.Follow(userToFollow);
            Console.WriteLine($"You are now following {userToFollow.Username}.");
        }

        static void ReplyToPost()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            Console.Write("Enter the ID of the post you want to reply to: ");
            if (!int.TryParse(Console.ReadLine(), out int postId))
            {
                Console.WriteLine("Invalid post ID. Please enter a valid number.");
                return;
            }

            Post post = currentUser.GetPostById(postId);

            if (post == null)
            {
                Console.WriteLine("Post not found. Please enter a valid post ID.");
                return;
            }

            Console.Write("Enter your reply: ");
            string replyContent = Console.ReadLine();
            currentUser.ReplyToPost(post, replyContent);
            Console.WriteLine("Reply added successfully!");
        }

        static void UpvotePost()
        {
            VotePost(true);
        }

        static void DownvotePost()
        {
            VotePost(false);
        }

        static void VotePost(bool upvote)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            Console.Write("Enter the ID of the post you want to vote: ");
            if (!int.TryParse(Console.ReadLine(), out int postId))
            {
                Console.WriteLine("Invalid post ID. Please enter a valid number.");
                return;
            }

            Post post = currentUser.GetPostById(postId);

            if (post == null)
            {
                Console.WriteLine("Post not found. Please enter a valid post ID.");
                return;
            }

            if (upvote)
            {
                currentUser.UpvotePost(post);
                Console.WriteLine("Post upvoted!");
            }
            else
            {
                currentUser.DownvotePost(post);
                Console.WriteLine("Post downvoted!");
            }
        }

        static void ShowNewsFeed()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Please log in first.");
                return;
            }

            List<Post> newsFeed = currentUser.GetNewsFeed();

            if (newsFeed.Count == 0)
            {
                Console.WriteLine("Your news feed is empty.");
                return;
            }

            Console.WriteLine("News Feed:");
            foreach (Post post in newsFeed)
            {
                Console.WriteLine($"Post ID: {post.PostID}");
                Console.WriteLine($"Author: {post.Author.Username}");
                Console.WriteLine($"Content: {post.Content}");
                Console.WriteLine($"Upvotes: {post.Upvotes} | Downvotes: {post.Downvotes}");
                Console.WriteLine("Comments:");
                foreach (Comment comment in post.Comments)
                {
                    Console.WriteLine($"- {comment.Author.Username}: {comment.Content}");
                    Console.WriteLine($"  Upvotes: {comment.Upvotes} | Downvotes: {comment.Downvotes}");
                }
                Console.WriteLine("---------------");
            }
        }

        static void LogoutUser()
        {
            if (currentUser == null)
            {
                Console.WriteLine("You are not logged in.");
                return;
            }

            currentUser = null;
            Console.WriteLine("Logged out successfully.");
        }
    }

    class User
    {
        private static int nextUserId = 1;

        public int UserID { get; }
        public string Username { get; }
        public string Password { get; }
        public List<User> Following { get; }
        public List<Post> Posts { get; }

        public User(string username, string password)
        {
            UserID = nextUserId++;
            Username = username;
            Password = password;
            Following = new List<User>();
            Posts = new List<Post>();
        }

        public void CreatePost(string content)
        {
            Post newPost = new Post(this, content);
            Posts.Add(newPost);
        }

        public void Follow(User userToFollow)
        {
            Following.Add(userToFollow);
        }

        public bool IsFollowing(User user)
        {
            return Following.Contains(user);
        }

        public void ReplyToPost(Post post, string content)
        {
            Comment newComment = new Comment(this, content);
            post.AddComment(newComment);
        }

        public void UpvotePost(Post post)
        {
            post.Upvote();
        }

        public void DownvotePost(Post post)
        {
            post.Downvote();
        }

        public List<Post> GetNewsFeed()
        {
            return Posts.Concat(Following.SelectMany(u => u.Posts))
                        .OrderByDescending(p => p.Timestamp)
                        .ToList();
        }

        public Post GetPostById(int postId)
        {
            return Posts.FirstOrDefault(p => p.PostID == postId);
        }
    }

    class Post
    {
        private static int nextPostId = 1;

        public int PostID { get; }
        public User Author { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
        public int Upvotes { get; private set; }
        public int Downvotes { get; private set; }
        public List<Comment> Comments { get; }

        public Post(User author, string content)
        {
            PostID = nextPostId++;
            Author = author;
            Content = content;
            Timestamp = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
            Comments = new List<Comment>();
        }

        public void AddComment(Comment comment)
        {
            Comments.Add(comment);
        }

        public void Upvote()
        {
            Upvotes++;
        }

        public void Downvote()
        {
            Downvotes++;
        }
    }

    class Comment
    {
        private static int nextCommentId = 1;

        public int CommentID { get; }
        public User Author { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
        public int Upvotes { get; private set; }
        public int Downvotes { get; private set; }

        public Comment(User author, string content)
        {
            CommentID = nextCommentId++;
            Author = author;
            Content = content;
            Timestamp = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }

        public void Upvote()
        {
            Upvotes++;
        }

        public void Downvote()
        {
            Downvotes++;
        }
    }
}
