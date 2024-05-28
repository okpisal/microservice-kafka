using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class PostCreateEvent : BaseEvent 
    {
        public PostCreateEvent():base(nameof(PostCreateEvent))
        {
            
        }
        public string Author { get; set; }

        public string Message { get; set; }

        public DateTime DatePosted { get; set; }
    }
}